using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

namespace Netlenium.Driver
{
    /// <summary>
    ///     Manages File Archives
    /// </summary>
    public static class FileArchive
    {
        /// <summary>
        ///     Auto-detects the archive type and extracts the contents to a destination path
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destinationPath"></param>
        public static void ExtractArchive(string filename, string destinationPath)
        {
            switch (Path.GetExtension(filename))
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
                    throw new UnsupportedArchiveException(
                        $"The archive '{Path.GetExtension(filename)}' is not supported.");
            }
        }

        /// <summary>
        ///     Extracts all contents from a Zip file to a destination path
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destinationPath"></param>
        private static void ExtractZip(string filename, string destinationPath)
        {
			ZipFile zf = null;
           	try
            {
           		var fs = File.OpenRead(filename);
           		zf = new ZipFile(fs);
                
           		foreach (ZipEntry zipEntry in zf)
                {
           			if (!zipEntry.IsFile) { continue; }
                    
           			var buffer = new byte[4096];
           			var zipStream = zf.GetInputStream(zipEntry);
           			var fullZipToPath = Path.Combine(destinationPath, Path.GetFileName(zipEntry.Name) ?? throw new Exception("Cannot resolve path name"));

           			using (var streamWriter = File.Create(fullZipToPath))
                    {
           				StreamUtils.Copy(zipStream, streamWriter, buffer);
           			}
           		}
           	}
            finally
            {
           		if (zf != null)
                {
           			zf.IsStreamOwner = true; // Makes close also shut the underlying stream
           			zf.Close(); // Ensure we release resources
           		}
           	}
        }

        /// <summary>
        ///     Extracts a <i>.tar.gz</i> archive to the specified directory.
        /// </summary>
        /// <param name="filename">The <i>.tar.gz</i> to decompress and extract.</param>
        /// <param name="destinationPath">Output directory to write the files.</param>
        private static void ExtractTarGz(string filename, string destinationPath)
        {
            Stream inStream = File.OpenRead(filename);
            Stream gzipStream = new GZipInputStream(inStream);

            var tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destinationPath);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }

        /// <summary>
        ///     Extracts a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="filename">The <i>.tar</i> to extract.</param>
        /// <param name="destinationPath">Output directory to write the files.</param>
        private static void ExtractTar(string filename, string destinationPath)
        {
            using (var fsIn = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var tarIn = new TarInputStream(fsIn);
                TarEntry tarEntry;
                while ((tarEntry = tarIn.GetNextEntry()) != null)
                {
                    if (tarEntry.IsDirectory) continue;

                    // Converts the unix forward slashes in the filenames to windows backslashes
                    var name = tarEntry.Name.Replace('/', Path.DirectorySeparatorChar);

                    // Remove any root e.g. '\' because a PathRooted filename defeats Path.Combine
                    if (Path.IsPathRooted(name)) name = name.Substring(Path.GetPathRoot(name).Length);

                    // Apply further name transformations here as necessary
                    var outName = Path.Combine(destinationPath, Path.GetFileName(name));

                    var outStr = new FileStream(outName, FileMode.Create);
                    tarIn.CopyEntryContents(outStr);
                    outStr.Close();

                    // Set the modification date/time. This approach seems to solve timezone issues.
                    var myDt = DateTime.SpecifyKind(tarEntry.ModTime, DateTimeKind.Utc);
                    File.SetLastWriteTime(outName, myDt);
                }

                tarIn.Close();
            }
        }
    }
}