using System;
using System.Diagnostics;

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

            return Platform.Windows;
        }

        /// <summary>
        /// Gives executable permissions to the given file on Linux based operating systems
        /// </summary>
        /// <param name="filePath"></param>
        public static void GiveExecutablePermissions(string filePath)
        {
            Process.Start("chmod", $"+x \"{filePath}\"");
        }
    }
}
