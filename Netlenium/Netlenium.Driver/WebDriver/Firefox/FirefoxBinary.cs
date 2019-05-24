using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Netlenium.Driver.WebDriver.Firefox.Internal;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Represents the binary associated with Firefox.
    /// </summary>
    /// <remarks>The <see cref="FirefoxBinary"/> class is responsible for instantiating the
    /// Firefox process, and the operating system environment in which it runs.</remarks>
    public class FirefoxBinary : IDisposable
    {
        private const string NoFocusLibraryName = "x_ignore_nofocus.so";
        private Dictionary<string, string> extraEnv = new Dictionary<string, string>();
        private Executable executable;
        private Process process;
        private TimeSpan timeout = TimeSpan.FromSeconds(45);
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxBinary"/> class.
        /// </summary>
        public FirefoxBinary()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxBinary"/> class located at a specific file location.
        /// </summary>
        /// <param name="pathToFirefoxBinary">Full path and file name to the Firefox executable.</param>
        public FirefoxBinary(string pathToFirefoxBinary)
        {
            executable = new Executable(pathToFirefoxBinary);
        }

        /// <summary>
        /// Gets or sets the timeout to wait for Firefox to be available for command execution.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Gets the <see cref="Executable"/> associated with this <see cref="FirefoxBinary"/>.
        /// </summary>
        internal Executable BinaryExecutable
        {
            get { return executable; }
        }

        /// <summary>
        /// Gets a value indicating whether the current operating system is Linux.
        /// </summary>
        protected static bool IsOnLinux
        {
            get { return Platform.CurrentPlatform.IsPlatformType(PlatformType.Linux); }
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{K, V}"/> containing string key-value pairs
        /// representing any operating system environment variables beyond the defaults.
        /// </summary>
        protected Dictionary<string, string> ExtraEnvironmentVariables
        {
            get { return extraEnv; }
        }

        /// <summary>
        /// Starts Firefox using the specified profile and command-line arguments.
        /// </summary>
        /// <param name="profile">The <see cref="FirefoxProfile"/> to use with this instance of Firefox.</param>
        /// <param name="commandLineArguments">The command-line arguments to use in starting Firefox.</param>
        [SecurityPermission(SecurityAction.Demand)]
        public void StartProfile(FirefoxProfile profile, params string[] commandLineArguments)
        {
            if (profile == null)
            {
                throw new ArgumentNullException("profile", "profile cannot be null");
            }

            if (commandLineArguments == null)
            {
                commandLineArguments = new string[] { };
            }

            var profileAbsPath = profile.ProfileDirectory;
            SetEnvironmentProperty("XRE_PROFILE_PATH", profileAbsPath);
            SetEnvironmentProperty("MOZ_NO_REMOTE", "1");
            SetEnvironmentProperty("MOZ_CRASHREPORTER_DISABLE", "1"); // Disable Breakpad
            SetEnvironmentProperty("NO_EM_RESTART", "1"); // Prevent the binary from detaching from the console

            if (IsOnLinux && (profile.EnableNativeEvents || profile.AlwaysLoadNoFocusLibrary))
            {
                ModifyLinkLibraryPath(profile);
            }

            var commandLineArgs = new StringBuilder();
            foreach (var commandLineArg in commandLineArguments)
            {
                commandLineArgs.Append(" ").Append(commandLineArg);
            }

            process = new Process();
            process.StartInfo.FileName = BinaryExecutable.ExecutablePath;
            process.StartInfo.Arguments = commandLineArgs.ToString();
            process.StartInfo.UseShellExecute = false;

            foreach (var environmentVar in extraEnv.Keys)
            {
                if (process.StartInfo.EnvironmentVariables.ContainsKey(environmentVar))
                {
                    process.StartInfo.EnvironmentVariables[environmentVar] = extraEnv[environmentVar];
                }
                else
                {
                    process.StartInfo.EnvironmentVariables.Add(environmentVar, extraEnv[environmentVar]);
                }
            }

            BinaryExecutable.SetLibraryPath(process);

            StartFirefoxProcess();

            CopeWithTheStrangenessOfTheMac();
        }

        /// <summary>
        /// Sets a variable to be used in the Firefox execution environment.
        /// </summary>
        /// <param name="propertyName">The name of the environment variable to set.</param>
        /// <param name="value">The value of the environment variable to set.</param>
        public void SetEnvironmentProperty(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(propertyName) || value == null)
            {
                throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "You must set both the property name and value: {0}, {1}", propertyName, value));
            }

            if (extraEnv.ContainsKey(propertyName))
            {
                extraEnv[propertyName] = value;
            }
            else
            {
                extraEnv.Add(propertyName, value);
            }
        }

        /// <summary>
        /// Waits for the process to complete execution.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand)]
        public void WaitForProcessExit()
        {
            process.WaitForExit();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="FirefoxBinary"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a <see cref="string">String</see> that represents the current <see cref="object">Object</see>.
        /// </summary>
        /// <returns>A <see cref="string">String</see> that represents the current <see cref="object">Object</see>.</returns>
        public override string ToString()
        {
            return "FirefoxBinary(" + executable.ExecutablePath + ")";
        }

        /// <summary>
        /// Starts the Firefox process.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand)]
        protected void StartFirefoxProcess()
        {
            process.Start();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FirefoxBinary"/> and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release managed and resources;
        /// <see langword="false"/> to only release unmanaged resources.</param>
        [SecurityPermission(SecurityAction.Demand)]
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // Suicide watch: First,  a second to see if the process will die on
                    // it's own (we will likely have asked the process to kill itself just
                    // before calling this method).
                    if (process != null)
                    {
                        if (!process.HasExited)
                        {
                            Thread.Sleep(1000);
                        }

                        // Murder option: The process is still alive, so kill it.
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }

                        process.Dispose();
                        process = null;
                    }
                }

                isDisposed = true;
            }
        }

        private static void Sleep(int timeInMilliseconds)
        {
            try
            {
                Thread.Sleep(timeInMilliseconds);
            }
            catch (ThreadInterruptedException e)
            {
                throw new WebDriverException("Thread was interrupted", e);
            }
        }

        private static string ExtractAndCheck(FirefoxProfile profile, string noFocusSoName, string libraryPath32Bit, string libraryPath64Bit)
        {
            //// 1. Extract x86/x_ignore_nofocus.so to profile.getLibsDir32bit
            //// 2. Extract amd64/x_ignore_nofocus.so to profile.getLibsDir64bit
            //// 3. Create a new LD_LIB_PATH string to contain:
            ////    profile.getLibsDir32bit + ":" + profile.getLibsDir64bit
            var pathsSet = new List<string>();
            pathsSet.Add(libraryPath32Bit);
            pathsSet.Add(libraryPath64Bit);

            var builtPath = new StringBuilder();

            foreach (var path in pathsSet)
            {
                var outSoPath = Path.Combine(profile.ProfileDirectory, path);
                var file = Path.Combine(outSoPath, noFocusSoName);
                var resourceName = string.Format(CultureInfo.InvariantCulture, "WebDriver.FirefoxNoFocus.{0}.dll", path);
                if (ResourceUtilities.IsValidResourceName(resourceName))
                {
                    using (var libraryStream = ResourceUtilities.GetResourceStream(noFocusSoName, resourceName))
                    {
                        Directory.CreateDirectory(outSoPath);
                        using (var outputStream = File.Create(file))
                        {
                            var buffer = new byte[1000];
                            var bytesRead = libraryStream.Read(buffer, 0, buffer.Length);
                            while (bytesRead > 0)
                            {
                                outputStream.Write(buffer, 0, bytesRead);
                                bytesRead = libraryStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }

                if (!File.Exists(file))
                {
                    throw new WebDriverException("Could not locate " + path + ": "
                                                 + "native events will not work.");
                }

                builtPath.Append(outSoPath).Append(Path.PathSeparator);
            }

            return builtPath.ToString();
        }

        private void ModifyLinkLibraryPath(FirefoxProfile profile)
        {
            // Extract x_ignore_nofocus.so from x86, amd64 directories inside
            // the jar into a real place in the filesystem and change LD_LIBRARY_PATH
            // to reflect that.
            var existingLdLibPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");

            // The returned new ld lib path is terminated with ':'
            var newLdLibPath = ExtractAndCheck(profile, NoFocusLibraryName, "x86", "x64");
            if (!string.IsNullOrEmpty(existingLdLibPath))
            {
                newLdLibPath += existingLdLibPath;
            }

            SetEnvironmentProperty("LD_LIBRARY_PATH", newLdLibPath);

            // Set LD_PRELOAD to x_ignore_nofocus.so - this will be taken automagically
            // from the LD_LIBRARY_PATH
            SetEnvironmentProperty("LD_PRELOAD", NoFocusLibraryName);
        }

        [SecurityPermission(SecurityAction.Demand)]
        private void CopeWithTheStrangenessOfTheMac()
        {
            if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac))
            {
                // On the Mac, this process sometimes dies. Check for this, put in a decent sleep
                // and then attempt to restart it. If this doesn't work, then give up

                // TODO(simon): Why is this happening? Firefox 2 never seemed to suffer this
                try
                {
                    Thread.Sleep(300);
                    if (process.ExitCode == 0)
                    {
                        return;
                    }

                    // Looks like it's gone wrong.
                    // TODO(simon): This is utterly bogus. We should do something far smarter
                    Thread.Sleep(10000);

                    StartFirefoxProcess();
                }
                catch (ThreadStateException)
                {
                    // Excellent, we've not creashed.
                }

                // Ensure we're okay
                try
                {
                    Sleep(300);

                    if (process.ExitCode == 0)
                    {
                        return;
                    }

                    var message = new StringBuilder("Unable to start firefox cleanly.\n");
                    message.Append("Exit value: ").Append(process.ExitCode.ToString(CultureInfo.InvariantCulture)).Append("\n");
                    message.Append("Ran from: ").Append(process.StartInfo.FileName).Append("\n");
                    throw new WebDriverException(message.ToString());
                }
                catch (ThreadStateException)
                {
                    // Woot!
                }
            }
        }
    }
}
