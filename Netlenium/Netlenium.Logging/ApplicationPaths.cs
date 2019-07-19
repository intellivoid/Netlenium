using System;
using System.IO;

namespace Netlenium.Logging
{
    /// <summary>
    /// Application Directory Paths that are available
    /// </summary>
    public static class ApplicationPaths
    {
        private static string AppData
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
