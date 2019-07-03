using System;
using System.IO;

namespace Netlenium.Driver
{
    /// <summary>
    /// Application Directory Paths that are available
    /// </summary>
    public static class ApplicationPaths
    {
        public static string AppData
        {
            get
            {
                var directoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Netlenium";

                if (Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return directoryPath;
            }
        }

        /// <summary>
        /// Driver installations in the Netlenium Application Directory
        /// </summary>
        public static string DriverDirectory
        {
            get
            {
                var directoryPath = $"{AppData}{Path.DirectorySeparatorChar}Drivers";

                if (Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return directoryPath;
            }
        }

        /// <summary>
        /// Gets the temporary directory from the Netlenium Application directory
        /// </summary>
        public static string TemporaryDirectory
        {
            get
            {
                var directoryPath = $"{AppData}{Path.DirectorySeparatorChar}tmp";

                if (Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return directoryPath;
            }
        }

        /// <summary>
        /// Gets the directory dedicated for storing log files
        /// </summary>
        public static string LoggingDirectory
        {
            get
            {
                var directoryPath = $"{AppData}{Path.DirectorySeparatorChar}logs";

                if (Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return directoryPath;
            }
        }
    }
}
