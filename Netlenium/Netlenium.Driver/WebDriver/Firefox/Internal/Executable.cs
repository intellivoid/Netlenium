using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Firefox.Internal
{
    /// <summary>
    /// Represents the executable file for Firefox.
    /// </summary>
    internal class Executable
    {
        private readonly string binaryInDefaultLocationForPlatform;
        private string binaryLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Executable"/> class.
        /// </summary>
        /// <param name="userSpecifiedBinaryPath">The path and file name to the Firefox executable.</param>
        public Executable(string userSpecifiedBinaryPath)
        {
            if (!string.IsNullOrEmpty(userSpecifiedBinaryPath))
            {
                // It should exist and be a file.
                if (File.Exists(userSpecifiedBinaryPath))
                {
                    binaryLocation = userSpecifiedBinaryPath;
                    return;
                }

                throw new WebDriverException(
                    "Specified firefox binary location does not exist or is not a real file: " +
                    userSpecifiedBinaryPath);
            }
            else
            {
                binaryInDefaultLocationForPlatform = LocateFirefoxBinaryFromPlatform();
            }

            if (binaryInDefaultLocationForPlatform != null && File.Exists(binaryInDefaultLocationForPlatform))
            {
                binaryLocation = binaryInDefaultLocationForPlatform;
                return;
            }

            throw new WebDriverException("Cannot find Firefox binary in PATH or default install locations. " +
                "Make sure Firefox is installed. OS appears to be: " + Platform.CurrentPlatform.ToString());
        }

        /// <summary>
        /// Gets the full path to the executable.
        /// </summary>
        public string ExecutablePath
        {
            get { return binaryLocation; }
        }

        /// <summary>
        /// Sets the library path for the Firefox executable environment.
        /// </summary>
        /// <param name="builder">The <see cref="Process"/> used to execute the binary.</param>
        [SecurityPermission(SecurityAction.Demand)]
        public void SetLibraryPath(Process builder)
        {
            var propertyName = GetLibraryPathPropertyName();
            var libraryPath = new StringBuilder();

            // If we have an env var set for the path, use it.
            var env = GetEnvironmentVariable(propertyName, null);
            if (env != null)
            {
                libraryPath.Append(env).Append(Path.PathSeparator);
            }

            // Check our extra env vars for the same var, and use it too.
            if (builder.StartInfo.EnvironmentVariables.ContainsKey(propertyName))
            {
                libraryPath.Append(builder.StartInfo.EnvironmentVariables[propertyName]).Append(Path.PathSeparator);
            }

            // Last, add the contents of the specified system property, defaulting to the binary's path.
            // On Snow Leopard, beware of problems the sqlite library
            var firefoxLibraryPath = Path.GetFullPath(binaryLocation);
            if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac) && Platform.CurrentPlatform.MinorVersion > 5)
            {
                libraryPath.Append(Path.PathSeparator);
            }
            else
            {
                // Insert the Firefox library path and the path separator at the beginning
                // of the path.
                libraryPath.Insert(0, Path.PathSeparator).Insert(0, firefoxLibraryPath);
            }

            // Add the library path to the builder.
            if (builder.StartInfo.EnvironmentVariables.ContainsKey(propertyName))
            {
                builder.StartInfo.EnvironmentVariables[propertyName] = libraryPath.ToString();
            }
            else
            {
                builder.StartInfo.EnvironmentVariables.Add(propertyName, libraryPath.ToString());
            }
        }

        /// <summary>
        /// Locates the Firefox binary by platform.
        /// </summary>
        /// <returns>The full path to the binary.</returns>
        [SecurityPermission(SecurityAction.Demand)]
        private static string LocateFirefoxBinaryFromPlatform()
        {
            var binary = string.Empty;
            if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
            {
#if !NETCOREAPP2_0 && !NETSTANDARD2_0
                // NOTE: This code is legacy, and will be removed. It will not be
                // fixed for the .NET Core case.
                // Look first in HKEY_LOCAL_MACHINE, then in HKEY_CURRENT_USER
                // if it's not found there. If it's still not found, look in
                // the default install location (C:\Program Files\Mozilla Firefox).
                var firefoxRegistryKey = @"SOFTWARE\Mozilla\Mozilla Firefox";
                var mozillaKey = Registry.LocalMachine.OpenSubKey(firefoxRegistryKey);
                if (mozillaKey == null)
                {
                    mozillaKey = Registry.CurrentUser.OpenSubKey(firefoxRegistryKey);
                }

                if (mozillaKey != null)
                {
                    binary = GetExecutablePathUsingRegistry(mozillaKey);
                }
                else
                {
#endif
                    // NOTE: Can't use Environment.SpecialFolder.ProgramFilesX86, because .NET 3.5
                    // doesn't have that member of the enum.
                    var windowsDefaultInstallLocations = new string[]
                    {
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Mozilla Firefox"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + " (x86)", "Mozilla Firefox")
                    };

                    binary = GetExecutablePathUsingDefaultInstallLocations(windowsDefaultInstallLocations, "Firefox.exe");
#if !NETCOREAPP2_0 && !NETSTANDARD2_0
                }
#endif
            }
            else
            {
                var macDefaultInstallLocations = new string[]
                {
                    "/Applications/Firefox.app/Contents/MacOS",
                    string.Format(CultureInfo.InvariantCulture, "/Users/{0}/Applications/Firefox.app/Contents/MacOS", Environment.UserName)
                };

                binary = GetExecutablePathUsingDefaultInstallLocations(macDefaultInstallLocations, "firefox-bin");

                if (string.IsNullOrEmpty(binary))
                {
                    // Use "which firefox" for non-Windows OS, and non-Mac OS where
                    // Firefox is installed in a non-default location.
                    using (var proc = new Process())
                    {
                        proc.StartInfo.FileName = "which";
                        proc.StartInfo.Arguments = "firefox";
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.Start();
                        proc.WaitForExit();
                        binary = proc.StandardOutput.ReadToEnd().Trim();
                    }
                }
            }

            if (binary != null && File.Exists(binary))
            {
                return binary;
            }

            // Didn't find binary in any of the default install locations, so look
            // at directories on the user's PATH environment variable.
            return FindBinary(new string[] { "firefox3", "firefox" });
        }

#if !NETCOREAPP2_0 && !NETSTANDARD2_0
        private static string GetExecutablePathUsingRegistry(RegistryKey mozillaKey)
        {
            // NOTE: This code is legacy, and will be removed. It will not be
            // fixed for the .NET Core case.
            var currentVersion = (string)mozillaKey.GetValue("CurrentVersion");
            if (string.IsNullOrEmpty(currentVersion))
            {
                throw new WebDriverException("Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox correctly");
            }

            var currentMain = mozillaKey.OpenSubKey(string.Format(CultureInfo.InvariantCulture, @"{0}\Main", currentVersion));
            if (currentMain == null)
            {
                throw new WebDriverException(
                    "Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox correctly");
            }

            var path = (string)currentMain.GetValue("PathToExe");
            if (!File.Exists(path))
            {
                throw new WebDriverException(
                    "FireFox executable listed in the registry does not exist, please make sure you have installed FireFox correctly");
            }

            return path;
        }
#endif

        private static string GetExecutablePathUsingDefaultInstallLocations(string[] defaultInstallLocations, string exeName)
        {
            foreach (var defaultInstallLocation in defaultInstallLocations)
            {
                var fullPath = Path.Combine(defaultInstallLocation, exeName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves an environment variable
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="defaultValue">Default value of the variable.</param>
        /// <returns>The value of the variable. If no variable with that name is set, returns the default.</returns>
        private static string GetEnvironmentVariable(string name, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Retrieves the platform specific environment property name which contains the library path.
        /// </summary>
        /// <returns>The platform specific environment property name which contains the library path.</returns>
        private static string GetLibraryPathPropertyName()
        {
            var libraryPropertyPathName = "LD_LIBRARY_PATH";
            if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
            {
                libraryPropertyPathName = "PATH";
            }
            else if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac))
            {
                libraryPropertyPathName = "DYLD_LIBRARY_PATH";
            }

            return libraryPropertyPathName;
        }

        /// <summary>
        /// Walk a PATH to locate binaries with a specified name. Binaries will be searched for in the
        /// order they are provided.
        /// </summary>
        /// <param name="binaryNames">The binary names to search for.</param>
        /// <returns>The first binary found matching that name.</returns>
        private static string FindBinary(string[] binaryNames)
        {
            foreach (var binaryName in binaryNames)
            {
                var exe = binaryName;
                if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
                {
                    exe += ".exe";
                }

                var path = FileUtilities.FindFile(exe);
                if (!string.IsNullOrEmpty(path))
                {
                    return Path.Combine(path, exe);
                }
            }

            return null;
        }
    }
}
