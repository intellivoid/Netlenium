using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Netlenium.Driver
{
    /// <summary>
    /// Manages File Archives
    /// </summary>
    public static class FileArchive
    {
        /// <summary>
        /// Auto-detects the archive type and extracts the contents to a destination path
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destinationPath"></param>
        public static void ExtractArchive(string filename, string destinationPath)
        {
            switch(Path.GetExtension(filename))
            {
                case ".zip":
                    ExtractZip(filename, destinationPath);
                    break;

                case ".tar":
                    ExtractTar(filename, destinationPath);
                    break;

                case ".tgz":
                    ExtractTarGz(filename, destinationPath);
                    break;

                case ".gz":
                    ExtractTarGz(filename, destinationPath);
                    break;

                default:
                    throw new UnsupportedArchiveException($"The archive '{Path.GetExtension(filename)}' is not supported.");
            }
        }
        
        /// <summary>
        /// Extracts all contents from a Zip file to a destination path
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destinationPath"></param>
        public static void ExtractZip(string filename, string destinationPath)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(filename, destinationPath, null);
        }

        /// <summary>
		/// Extracts a <i>.tar.gz</i> archive to the specified directory.
		/// </summary>
		/// <param name="filename">The <i>.tar.gz</i> to decompress and extract.</param>
		/// <param name="destinationPath">Output directory to write the files.</param>
		public static void ExtractTarGz(string filename, string destinationPath)
        {
            Stream inStream = File.OpenRead(filename);
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destinationPath);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }

        /// <summary>
        /// Extractes a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="filename">The <i>.tar</i> to extract.</param>
        /// <param name="destinationPath">Output directory to write the files.</param>
        public static void ExtractTar(string filename, string destinationPath)
        {
            Stream inStream = File.OpenRead(filename);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream);
            tarArchive.ExtractContents(destinationPath);
            tarArchive.Close();

            inStream.Close();
        }

     
    }
}
