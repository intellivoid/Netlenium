using System.IO;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Represents a file detector for determining whether a file
    /// must be uploaded to a remote server.
    /// </summary>
    public class LocalFileDetector : IFileDetector
    {
        /// <summary>
        /// Returns a value indicating whether a specified key sequence represents
        /// a file name and path.
        /// </summary>
        /// <param name="keySequence">The sequence to test for file existence.</param>
        /// <returns><see langword="true"/> if the key sequence represents a file; otherwise, <see langword="false"/>.</returns>
        public bool IsFile(string keySequence)
        {
            return File.Exists(keySequence);
        }
    }
}
