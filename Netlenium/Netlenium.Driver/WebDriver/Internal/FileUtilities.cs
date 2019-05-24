using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Encapsulates methods for working with files.
    /// </summary>
    internal static class FileUtilities
    {
        /// <summary>
        /// Recursively copies a directory.
        /// </summary>
        /// <param name="sourceDirectory">The source directory to copy.</param>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <returns><see langword="true"/> if the copy is completed; otherwise <see langword="false"/>.</returns>
        public static bool CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            var copyComplete = false;
            var sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);
            var destinationDirectoryInfo = new DirectoryInfo(destinationDirectory);

            if (sourceDirectoryInfo.Exists)
            {
                if (!destinationDirectoryInfo.Exists)
                {
                    destinationDirectoryInfo.Create();
                }

                foreach (var fileEntry in sourceDirectoryInfo.GetFiles())
                {
                    fileEntry.CopyTo(Path.Combine(destinationDirectoryInfo.FullName, fileEntry.Name));
                }

                foreach (var directoryEntry in sourceDirectoryInfo.GetDirectories())
                {
                    if (!CopyDirectory(directoryEntry.FullName, Path.Combine(destinationDirectoryInfo.FullName, directoryEntry.Name)))
                    {
                        copyComplete = false;
                    }
                }
            }

            copyComplete = true;
            return copyComplete;
        }

        /// <summary>
        /// Recursively deletes a directory, retrying on error until a timeout.
        /// </summary>
        /// <param name="directoryToDelete">The directory to delete.</param>
        /// <remarks>This method does not throw an exception if the delete fails.</remarks>
        public static void DeleteDirectory(string directoryToDelete)
        {
            var numberOfRetries = 0;
            while (Directory.Exists(directoryToDelete) && numberOfRetries < 10)
            {
                try
                {
                    Directory.Delete(directoryToDelete, true);
                }
                catch (IOException)
                {
                    // If we hit an exception (like file still in use), wait a half second
                    // and try again. If we still hit an exception, go ahead and let it through.
                    System.Threading.Thread.Sleep(500);
                }
                catch (UnauthorizedAccessException)
                {
                    // If we hit an exception (like file still in use), wait a half second
                    // and try again. If we still hit an exception, go ahead and let it through.
                    System.Threading.Thread.Sleep(500);
                }
                finally
                {
                    numberOfRetries++;
                }
            }

            if (Directory.Exists(directoryToDelete))
            {
                Console.WriteLine("Unable to delete directory '{0}'", directoryToDelete);
            }
        }

        /// <summary>
        /// Searches for a file with the specified name.
        /// </summary>
        /// <param name="fileName">The name of the file to find.</param>
        /// <returns>The full path to the directory where the file can be found,
        /// or an empty string if the file does not exist in the locations searched.</returns>
        /// <remarks>
        /// This method looks first in the directory of the currently executing
        /// assembly. If the specified file is not there, the method then looks in
        /// each directory on the PATH environment variable, in order.
        /// </remarks>
        public static string FindFile(string fileName)
        {
            // Look first in the same directory as the executing assembly
            var currentDirectory = GetCurrentDirectory();
            if (File.Exists(Path.Combine(currentDirectory, fileName)))
            {
                return currentDirectory;
            }

            // If it's not in the same directory as the executing assembly,
            // try looking in the system path.
            var systemPath = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(systemPath))
            {
                var expandedPath = Environment.ExpandEnvironmentVariables(systemPath);
                var directories = expandedPath.Split(Path.PathSeparator);
                foreach (var directory in directories)
                {
                    // N.B., if the directory in the path contains an invalid character,
                    // we will skip that directory, meaning no error will be thrown. This
                    // may be confusing to the user, so we might want to revisit this.
                    if (directory.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                    {
                        if (File.Exists(Path.Combine(directory, fileName)))
                        {
                            currentDirectory = directory;
                            return currentDirectory;
                        }
                    }
                }
            }

            // Note that if it wasn't found on the system path, currentDirectory is still
            // set to the same directory as the executing assembly.
            return string.Empty;
        }

        /// <summary>
        /// Gets the directory of the currently executing assembly.
        /// </summary>
        /// <returns>The directory of the currently executing assembly.</returns>
        public static string GetCurrentDirectory()
        {
            var executingAssembly = typeof(FileUtilities).Assembly;
            var location =  Path.GetDirectoryName(executingAssembly.Location);
            if (string.IsNullOrEmpty(location))
            {
                // If there is no location information from the executing
                // assembly, we will bail by using the current directory.
                // Note this is inaccurate, because the working directory
                // may not actually be the directory of the current assembly,
                // especially if the WebDriver assembly was embedded as a
                // resource.
                location = Directory.GetCurrentDirectory();
            }

            var currentDirectory = location;

            // If we're shadow copying, get the directory from the codebase instead
            if (AppDomain.CurrentDomain.ShadowCopyFiles)
            {
                var uri = new Uri(executingAssembly.CodeBase);
                currentDirectory = Path.GetDirectoryName(uri.LocalPath);
            }

            return currentDirectory;
        }

        /// <summary>
        /// Generates the full path to a random directory name in the temporary directory, following a naming pattern..
        /// </summary>
        /// <param name="directoryPattern">The pattern to use in creating the directory name, following standard
        /// .NET string replacement tokens.</param>
        /// <returns>The full path to the random directory name in the temporary directory.</returns>
        public static string GenerateRandomTempDirectoryName(string directoryPattern)
        {
            var directoryName = string.Format(CultureInfo.InvariantCulture, directoryPattern, Guid.NewGuid().ToString("N"));
            return Path.Combine(Path.GetTempPath(), directoryName);
        }
    }
}
