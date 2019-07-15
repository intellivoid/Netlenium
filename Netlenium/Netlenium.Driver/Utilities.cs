using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Netlenium.Driver

{
    /// <summary>
    /// Netlenium Utilities
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Detects the current platform of this machine
        /// </summary>
        /// <returns></returns>
        public static Platform DetectPlatform()
        {
            var p = (int)Environment.OSVersion.Platform;

            if (p == 4 || p == 6 || p == 128)
            {
                return Environment.Is64BitOperatingSystem ? Platform.Linux64 : Platform.Linux32;
            }

            return Environment.Is64BitOperatingSystem ? Platform.Windows64 : Platform.Windows32;
        }

        /// <summary>
        /// The directory of this current executable
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(
                    File.Exists($"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}Netlenium.so") 
                    ?
                    path : Process.GetCurrentProcess().MainModule.FileName
                );
            }
        }

        /// <summary>
        /// Gives executable permissions to the given file on Linux based operating systems
        /// </summary>
        /// <param name="filePath"></param>
        public static void GiveExecutablePermissions(string filePath)
        {
            Process.Start("chmod", $"+x \"{filePath}\"");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="browser"></param>
        /// <returns></returns>
        public static string GetDriverDirectoryName(Platform platform, Browser browser)
        {
            if(platform == Platform.AutoDetect)
            {
                platform = DetectPlatform();
            }

            var Results = string.Empty;

            switch(browser)
            {
                case Browser.Chrome:
                    Results = "chrome";
                    break;

                case Browser.Firefox:
                    Results = "firefox";
                    break;

                default:
                    Results = "unknown";
                    break;
            }

            switch(platform)
            {
                case Platform.Windows32:
                    Results = string.Format("{0}_{1}", Results, "windows_x86");
                    break;

                case Platform.Windows64:
                    Results = string.Format("{0}_{1}", Results, "windows_x64");
                    break;

                case Platform.Linux32:
                    Results = string.Format("{0}_{1}", Results, "linux_x86");
                    break;

                case Platform.Linux64:
                    Results = string.Format("{0}_{1}", Results, "linux_x64");
                    break;

                default:
                    Results = string.Format("{0}_{1}", Results, "unknown");
                    break;
            }

            return Results;
        }

        /// <summary>
        /// Detects the target platfrom from the Archive Name
        /// </summary>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        public static Platform DetectTargetPlatformFromArchive(string archiveName)
        {
            if (archiveName.ToLower().Contains("linux32"))
            {
                return Platform.Linux32;
            }

            if (archiveName.ToLower().Contains("linux64"))
            {
                return Platform.Linux64;
            }

            if (archiveName.ToLower().Contains("win32"))
            {
                return Platform.Windows32;
            }

            if (archiveName.ToLower().Contains("win64"))
            {
                return Platform.Windows64;
            }

            return Platform.None;
        }
    }
}
