using Ionic.Zip;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

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
            var zip = ZipFile.Read(filename);
            zip.ExtractAll(destinationPath, ExtractExistingFileAction.OverwriteSilently);
            zip.Dispose();
        }

        /// <summary>
		/// Extracts a <i>.tar.gz</i> archive to the specified directory.
		/// </summary>
		/// <param name="filename">The <i>.tar.gz</i> to decompress and extract.</param>
		/// <param name="destinationPath">Output directory to write the files.</param>
		public static void ExtractTarGz(string filename, string destinationPath)
        {
            using (var stream = File.OpenRead(filename))
                ExtractTarGz(stream, destinationPath);
        }

        /// <summary>
        /// Extracts a <i>.tar.gz</i> archive stream to the specified directory.
        /// </summary>
        /// <param name="stream">The <i>.tar.gz</i> to decompress and extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        private static void ExtractTarGz(Stream stream, string outputDir)
        {
            // A GZipStream is not seekable, so copy it first to a MemoryStream
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                const int chunk = 4096;
                using (var memStr = new MemoryStream())
                {
                    int read;
                    var buffer = new byte[chunk];
                    do
                    {
                        read = gzip.Read(buffer, 0, chunk);
                        memStr.Write(buffer, 0, read);
                    } while (read == chunk);

                    memStr.Seek(0, SeekOrigin.Begin);
                    ExtractTar(memStr, outputDir);
                }
            }
        }

        /// <summary>
        /// Extractes a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="filename">The <i>.tar</i> to extract.</param>
        /// <param name="destinationPath">Output directory to write the files.</param>
        public static void ExtractTar(string filename, string destinationPath)
        {
            using (var stream = File.OpenRead(filename))
                ExtractTar(stream, destinationPath);
        }

        /// <summary>
        /// Extractes a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="stream">The <i>.tar</i> to extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        private static void ExtractTar(Stream stream, string outputDir)
        {
            var buffer = new byte[100];
            while (true)
            {
                stream.Read(buffer, 0, 100);
                var name = Encoding.ASCII.GetString(buffer).Trim('\0');
                if (String.IsNullOrWhiteSpace(name))
                    break;
                stream.Seek(24, SeekOrigin.Current);
                stream.Read(buffer, 0, 12);
                var size = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 0, 12).Trim(), 8);

                stream.Seek(376L, SeekOrigin.Current);

                var output = Path.Combine(outputDir, name);
                if (!Directory.Exists(Path.GetDirectoryName(output)))
                    Directory.CreateDirectory(Path.GetDirectoryName(output));
                using (var str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    var buf = new byte[size];
                    stream.Read(buf, 0, buf.Length);
                    str.Write(buf, 0, buf.Length);
                }

                var pos = stream.Position;

                var offset = 512 - (pos % 512);
                if (offset == 512)
                    offset = 0;

                stream.Seek(offset, SeekOrigin.Current);
            }
        }
    }
}
