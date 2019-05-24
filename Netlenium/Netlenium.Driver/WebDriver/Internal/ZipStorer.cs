// <copyright file="ZipStorer.cs" company="Jaime Olivares">
//
// ZipStorer, by Jaime Olivares
// Website: zipstorer.codeplex.com
// Version: 2.35 (March 14, 2010)
//
// Used under the provisions of the Microsoft Public License (Ms-PL).
// You may obtain a copy of the License at
//
//     https://zipstorer.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace System.IO.Compression
{
    /// <summary>
    /// Unique class for compression/decompression file. Represents a Zip file.
    /// </summary>
    internal sealed class ZipStorer : IDisposable
    {
        // Static CRC32 Table
        private static uint[] crcTable = GenerateCrc32Table();

        // Default filename encoder
        private static Encoding defaultEncoding = Encoding.GetEncoding(437);

        // List of files to store
        private List<ZipFileEntry> files = new List<ZipFileEntry>();

        // Stream object of storage file
        private Stream zipFileStream;

        // General comment
        private string comment = string.Empty;

        // Central dir image
        private byte[] centralDirectoryImage = null;

        // Existing files in zip
        private ushort existingFileCount = 0;

        // File access for Open method
        private FileAccess access;

        // True if UTF8 encoding for filename and comments, false if default (CP 437)
        private bool encodeUtf8 = false;

        // Force deflate algotithm even if it inflates the stored file. Off by default.
        private bool forceDeflating = false;

        /// <summary>
        /// Compression method enumeration.
        /// </summary>
        public enum CompressionMethod : ushort
        {
            /// <summary>Uncompressed storage.</summary>
            Store = 0,

            /// <summary>Deflate compression method.</summary>
            Deflate = 8
        }

        /// <summary>
        /// Gets a value indicating whether file names and comments should be encoded using UTF-8.
        /// </summary>
        public bool EncodeUtf8
        {
            get { return encodeUtf8; }
        }

        /// <summary>
        /// Gets a value indicating whether to force using the deflate algorithm,
        /// even if doing so inflates the stored file.
        /// </summary>
        public bool ForceDeflating
        {
            get { return forceDeflating; }
        }

        /// <summary>
        /// Create a new zip storage in a stream.
        /// </summary>
        /// <param name="zipStream">The stream to use to create the Zip file.</param>
        /// <param name="fileComment">General comment for Zip file.</param>
        /// <returns>A valid ZipStorer object.</returns>
        public static ZipStorer Create(Stream zipStream, string fileComment)
        {
            var zip = new ZipStorer();
            zip.comment = fileComment;
            zip.zipFileStream = zipStream;
            zip.access = FileAccess.Write;

            return zip;
        }

        /// <summary>
        /// Open the existing Zip storage in a stream.
        /// </summary>
        /// <param name="stream">Already opened stream with zip contents.</param>
        /// <param name="access">File access mode for stream operations.</param>
        /// <returns>A valid ZipStorer object.</returns>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method. Caller assumes ownership of returned object")]
        public static ZipStorer Open(Stream stream, FileAccess access)
        {
            if (!stream.CanSeek && access != FileAccess.Read)
            {
                throw new InvalidOperationException("Stream cannot seek");
            }

            var zip = new ZipStorer();
            zip.zipFileStream = stream;
            zip.access = access;

            if (zip.ReadFileInfo())
            {
                return zip;
            }

            throw new InvalidDataException();
        }

        /// <summary>
        /// Add full contents of a file into the Zip storage.
        /// </summary>
        /// <param name="compressionMethod">Compression method used to store the file.</param>
        /// <param name="sourceFile">Full path of file to add to Zip storage.</param>
        /// <param name="fileNameInZip">File name and path as desired in Zip directory.</param>
        /// <param name="fileEntryComment">Comment for stored file.</param>
        public void AddFile(CompressionMethod compressionMethod, string sourceFile, string fileNameInZip, string fileEntryComment)
        {
            if (access == FileAccess.Read)
            {
                throw new InvalidOperationException("Writing is not allowed");
            }

            using (var stream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            {
                AddStream(compressionMethod, stream, fileNameInZip, File.GetLastWriteTime(sourceFile), fileEntryComment);
            }
        }

        /// <summary>
        /// Add full contents of a stream into the Zip storage.
        /// </summary>
        /// <param name="compressionMethod">Compression method used to store the stream.</param>
        /// <param name="sourceStream">Stream object containing the data to store in Zip.</param>
        /// <param name="fileNameInZip">File name and path as desired in Zip directory.</param>
        /// <param name="modificationTimeStamp">Modification time of the data to store.</param>
        /// <param name="fileEntryComment">Comment for stored file.</param>
        public void AddStream(CompressionMethod compressionMethod, Stream sourceStream, string fileNameInZip, DateTime modificationTimeStamp, string fileEntryComment)
        {
            if (access == FileAccess.Read)
            {
                throw new InvalidOperationException("Writing is not allowed");
            }

            // Prepare the fileinfo
            var zipFileEntry = default(ZipFileEntry);
            zipFileEntry.Method = compressionMethod;
            zipFileEntry.EncodeUTF8 = EncodeUtf8;
            zipFileEntry.FilenameInZip = NormalizeFileName(fileNameInZip);
            zipFileEntry.Comment = fileEntryComment == null ? string.Empty : fileEntryComment;

            // Even though we write the header now, it will have to be rewritten, since we don't know compressed size or crc.
            zipFileEntry.Crc32 = 0;  // to be updated later
            zipFileEntry.HeaderOffset = (uint)zipFileStream.Position;  // offset within file of the start of this local record
            zipFileEntry.ModifyTime = modificationTimeStamp;

            // Write local header
            WriteLocalHeader(ref zipFileEntry);
            zipFileEntry.FileOffset = (uint)zipFileStream.Position;

            // Write file to zip (store)
            Store(ref zipFileEntry, sourceStream);
            sourceStream.Close();

            UpdateCrcAndSizes(ref zipFileEntry);

            files.Add(zipFileEntry);
        }

        /// <summary>
        /// Updates central directory (if needed) and close the Zip storage.
        /// </summary>
        /// <remarks>This is a required step, unless automatic dispose is used.</remarks>
        public void Close()
        {
            if (access != FileAccess.Read)
            {
                var centralOffset = (uint)zipFileStream.Position;
                uint centralSize = 0;

                if (centralDirectoryImage != null)
                {
                    zipFileStream.Write(centralDirectoryImage, 0, centralDirectoryImage.Length);
                }

                for (var i = 0; i < files.Count; i++)
                {
                    var pos = zipFileStream.Position;
                    WriteCentralDirRecord(files[i]);
                    centralSize += (uint)(zipFileStream.Position - pos);
                }

                if (centralDirectoryImage != null)
                {
                    WriteEndRecord(centralSize + (uint)centralDirectoryImage.Length, centralOffset);
                }
                else
                {
                    WriteEndRecord(centralSize, centralOffset);
                }
            }

            if (zipFileStream != null)
            {
                zipFileStream.Flush();
                zipFileStream.Dispose();
                zipFileStream = null;
            }
        }

        /// <summary>
        /// Read all the file records in the central directory.
        /// </summary>
        /// <returns>List of all entries in directory.</returns>
        public List<ZipFileEntry> ReadCentralDirectory()
        {
            if (centralDirectoryImage == null)
            {
                throw new InvalidOperationException("Central directory currently does not exist");
            }

            var result = new List<ZipFileEntry>();

            var pointer = 0;
            while (pointer < centralDirectoryImage.Length)
            {
                var signature = BitConverter.ToUInt32(centralDirectoryImage, pointer);
                if (signature != 0x02014b50)
                {
                    break;
                }

                var isUTF8Encoded = (BitConverter.ToUInt16(centralDirectoryImage, pointer + 8) & 0x0800) != 0;
                var method = BitConverter.ToUInt16(centralDirectoryImage, pointer + 10);
                var modifyTime = BitConverter.ToUInt32(centralDirectoryImage, pointer + 12);
                var crc32 = BitConverter.ToUInt32(centralDirectoryImage, pointer + 16);
                var comprSize = BitConverter.ToUInt32(centralDirectoryImage, pointer + 20);
                var fileSize = BitConverter.ToUInt32(centralDirectoryImage, pointer + 24);
                var filenameSize = BitConverter.ToUInt16(centralDirectoryImage, pointer + 28);
                var extraSize = BitConverter.ToUInt16(centralDirectoryImage, pointer + 30);
                var commentSize = BitConverter.ToUInt16(centralDirectoryImage, pointer + 32);
                var headerOffset = BitConverter.ToUInt32(centralDirectoryImage, pointer + 42);
                var headerSize = (uint)(46 + filenameSize + extraSize + commentSize);

                var encoder = isUTF8Encoded ? Encoding.UTF8 : defaultEncoding;

                var zfe = default(ZipFileEntry);
                zfe.Method = (CompressionMethod)method;
                zfe.FilenameInZip = encoder.GetString(centralDirectoryImage, pointer + 46, filenameSize);
                zfe.FileOffset = GetFileOffset(headerOffset);
                zfe.FileSize = fileSize;
                zfe.CompressedSize = comprSize;
                zfe.HeaderOffset = headerOffset;
                zfe.HeaderSize = headerSize;
                zfe.Crc32 = crc32;
                zfe.ModifyTime = DosTimeToDateTime(modifyTime);
                if (commentSize > 0)
                {
                    zfe.Comment = encoder.GetString(centralDirectoryImage, pointer + 46 + filenameSize + extraSize, commentSize);
                }

                result.Add(zfe);
                pointer += 46 + filenameSize + extraSize + commentSize;
            }

            return result;
        }

        /// <summary>
        /// Copy the contents of a stored file into a physical file.
        /// </summary>
        /// <param name="zipFileEntry">Entry information of file to extract.</param>
        /// <param name="destinationFileName">Name of file to store uncompressed data.</param>
        /// <returns><see langword="true"/> if the file is successfully extracted; otherwise, <see langword="false"/>.</returns>
        /// <remarks>Unique compression methods are Store and Deflate.</remarks>
        public bool ExtractFile(ZipFileEntry zipFileEntry, string destinationFileName)
        {
            // Make sure the parent directory exist
            var path = Path.GetDirectoryName(destinationFileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check it is directory. If so, do nothing
            if (Directory.Exists(destinationFileName))
            {
                return true;
            }

            var result = false;
            using (Stream output = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write))
            {
                result = ExtractFile(zipFileEntry, output);
            }

            File.SetCreationTime(destinationFileName, zipFileEntry.ModifyTime);
            File.SetLastWriteTime(destinationFileName, zipFileEntry.ModifyTime);

            return result;
        }

        /// <summary>
        /// Copy the contents of a stored file into an open stream.
        /// </summary>
        /// <param name="zipFileEntry">Entry information of file to extract.</param>
        /// <param name="destinationStream">Stream to store the uncompressed data.</param>
        /// <returns><see langword="true"/> if the file is successfully extracted; otherwise, <see langword="false"/>.</returns>
        /// <remarks>Unique compression methods are Store and Deflate.</remarks>
        public bool ExtractFile(ZipFileEntry zipFileEntry, Stream destinationStream)
        {
            if (!destinationStream.CanWrite)
            {
                throw new InvalidOperationException("Stream cannot be written");
            }

            // check signature
            var signature = new byte[4];
            zipFileStream.Seek(zipFileEntry.HeaderOffset, SeekOrigin.Begin);
            zipFileStream.Read(signature, 0, 4);
            if (BitConverter.ToUInt32(signature, 0) != 0x04034b50)
            {
                return false;
            }

            // Select input stream for inflating or just reading
            Stream inStream;
            if (zipFileEntry.Method == CompressionMethod.Store)
            {
                inStream = zipFileStream;
            }
            else if (zipFileEntry.Method == CompressionMethod.Deflate)
            {
                inStream = new DeflateStream(zipFileStream, CompressionMode.Decompress, true);
            }
            else
            {
                return false;
            }

            // Buffered copy
            var buffer = new byte[16384];
            zipFileStream.Seek(zipFileEntry.FileOffset, SeekOrigin.Begin);
            var bytesPending = zipFileEntry.FileSize;
            while (bytesPending > 0)
            {
                var bytesRead = inStream.Read(buffer, 0, (int)Math.Min(bytesPending, buffer.Length));
                destinationStream.Write(buffer, 0, bytesRead);
                bytesPending -= (uint)bytesRead;
            }

            destinationStream.Flush();

            if (zipFileEntry.Method == CompressionMethod.Deflate)
            {
                inStream.Dispose();
            }

            return true;
        }

        /// <summary>
        /// Closes the Zip file stream.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        private static uint[] GenerateCrc32Table()
        {
            // Generate CRC32 table
            var table = new uint[256];
            for (var i = 0; i < table.Length; i++)
            {
                var c = (uint)i;
                for (var j = 0; j < 8; j++)
                {
                    if ((c & 1) != 0)
                    {
                        c = 3988292384 ^ (c >> 1);
                    }
                    else
                    {
                        c >>= 1;
                    }
                }

                table[i] = c;
            }

            return table;
        }

        /* DOS Date and time:
            MS-DOS date. The date is a packed value with the following format. Bits Description
                0-4 Day of the month (1–31)
                5-8 Month (1 = January, 2 = February, and so on)
                9-15 Year offset from 1980 (add 1980 to get actual year)
            MS-DOS time. The time is a packed value with the following format. Bits Description
                0-4 Second divided by 2
                5-10 Minute (0–59)
                11-15 Hour (0–23 on a 24-hour clock)
        */
        private static uint DateTimeToDosTime(DateTime dateTime)
        {
            return (uint)(
                (dateTime.Second / 2) | (dateTime.Minute << 5) | (dateTime.Hour << 11) |
                (dateTime.Day << 16) | (dateTime.Month << 21) | ((dateTime.Year - 1980) << 25));
        }

        private static DateTime DosTimeToDateTime(uint dosTime)
        {
            return new DateTime(
                (int)(dosTime >> 25) + 1980,
                (int)(dosTime >> 21) & 15,
                (int)(dosTime >> 16) & 31,
                (int)(dosTime >> 11) & 31,
                (int)(dosTime >> 5) & 63,
                (int)(dosTime & 31) * 2);
        }

        // Replaces backslashes with slashes to store in zip header
        private static string NormalizeFileName(string fileNameToNormalize)
        {
            var normalizedFileName = fileNameToNormalize.Replace('\\', '/');

            var pos = normalizedFileName.IndexOf(':');
            if (pos >= 0)
            {
                normalizedFileName = normalizedFileName.Remove(0, pos + 1);
            }

            return normalizedFileName.Trim('/');
        }

        // Calculate the file offset by reading the corresponding local header
        private uint GetFileOffset(uint headerOffset)
        {
            var buffer = new byte[2];

            zipFileStream.Seek(headerOffset + 26, SeekOrigin.Begin);
            zipFileStream.Read(buffer, 0, 2);
            var filenameSize = BitConverter.ToUInt16(buffer, 0);
            zipFileStream.Read(buffer, 0, 2);
            var extraSize = BitConverter.ToUInt16(buffer, 0);

            return (uint)(30 + filenameSize + extraSize + headerOffset);
        }

        /* Local file header:
            local file header signature     4 bytes  (0x04034b50)
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes

            filename (variable size)
            extra field (variable size)
        */
        private void WriteLocalHeader(ref ZipFileEntry zipFileEntry)
        {
            var pos = zipFileStream.Position;
            var encoder = zipFileEntry.EncodeUTF8 ? Encoding.UTF8 : defaultEncoding;
            var encodedFilename = encoder.GetBytes(zipFileEntry.FilenameInZip);

            zipFileStream.Write(new byte[] { 80, 75, 3, 4, 20, 0 }, 0, 6); // No extra header
            zipFileStream.Write(BitConverter.GetBytes((ushort)(zipFileEntry.EncodeUTF8 ? 0x0800 : 0)), 0, 2); // filename and comment encoding
            zipFileStream.Write(BitConverter.GetBytes((ushort)zipFileEntry.Method), 0, 2);  // zipping method
            zipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(zipFileEntry.ModifyTime)), 0, 4); // zipping date and time
            zipFileStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 12); // unused CRC, un/compressed size, updated later
            zipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2); // filename length
            zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // extra length

            zipFileStream.Write(encodedFilename, 0, encodedFilename.Length);
            zipFileEntry.HeaderSize = (uint)(zipFileStream.Position - pos);
        }

        /* Central directory's File header:
            central file header signature   4 bytes  (0x02014b50)
            version made by                 2 bytes
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes
            file comment length             2 bytes
            disk number start               2 bytes
            internal file attributes        2 bytes
            external file attributes        4 bytes
            relative offset of local header 4 bytes

            filename (variable size)
            extra field (variable size)
            file comment (variable size)
        */
        private void WriteCentralDirRecord(ZipFileEntry zipFileEntry)
        {
            var encoder = zipFileEntry.EncodeUTF8 ? Encoding.UTF8 : defaultEncoding;
            var encodedFilename = encoder.GetBytes(zipFileEntry.FilenameInZip);
            var encodedComment = encoder.GetBytes(zipFileEntry.Comment);

            zipFileStream.Write(new byte[] { 80, 75, 1, 2, 23, 0xB, 20, 0 }, 0, 8);
            zipFileStream.Write(BitConverter.GetBytes((ushort)(zipFileEntry.EncodeUTF8 ? 0x0800 : 0)), 0, 2); // filename and comment encoding
            zipFileStream.Write(BitConverter.GetBytes((ushort)zipFileEntry.Method), 0, 2);  // zipping method
            zipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(zipFileEntry.ModifyTime)), 0, 4);  // zipping date and time
            zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.Crc32), 0, 4); // file CRC
            zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.CompressedSize), 0, 4); // compressed file size
            zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.FileSize), 0, 4); // uncompressed file size
            zipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2); // Filename in zip
            zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // extra length
            zipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);

            zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // disk=0
            zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // file type: binary
            zipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); // Internal file attributes
            zipFileStream.Write(BitConverter.GetBytes((ushort)0x8100), 0, 2); // External file attributes (normal/readable)
            zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.HeaderOffset), 0, 4);  // Offset of header

            zipFileStream.Write(encodedFilename, 0, encodedFilename.Length);
            zipFileStream.Write(encodedComment, 0, encodedComment.Length);
        }

        /* End of central dir record:
            end of central dir signature    4 bytes  (0x06054b50)
            number of this disk             2 bytes
            number of the disk with the
            start of the central directory  2 bytes
            total number of entries in
            the central dir on this disk    2 bytes
            total number of entries in
            the central dir                 2 bytes
            size of the central directory   4 bytes
            offset of start of central
            directory with respect to
            the starting disk number        4 bytes
            zipfile comment length          2 bytes
            zipfile comment (variable size)
        */
        private void WriteEndRecord(uint size, uint offset)
        {
            var encoder = EncodeUtf8 ? Encoding.UTF8 : defaultEncoding;
            var encodedComment = encoder.GetBytes(comment);

            zipFileStream.Write(new byte[] { 80, 75, 5, 6, 0, 0, 0, 0 }, 0, 8);
            zipFileStream.Write(BitConverter.GetBytes((ushort)files.Count + existingFileCount), 0, 2);
            zipFileStream.Write(BitConverter.GetBytes((ushort)files.Count + existingFileCount), 0, 2);
            zipFileStream.Write(BitConverter.GetBytes(size), 0, 4);
            zipFileStream.Write(BitConverter.GetBytes(offset), 0, 4);
            zipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);
            zipFileStream.Write(encodedComment, 0, encodedComment.Length);
        }

        // Copies all source file into storage file
        private void Store(ref ZipFileEntry zipFileEntry, Stream sourceStream)
        {
            var buffer = new byte[16384];
            int bytesRead;
            uint totalRead = 0;
            Stream outStream;

            var posStart = zipFileStream.Position;
            var sourceStart = sourceStream.Position;

            if (zipFileEntry.Method == CompressionMethod.Store)
            {
                outStream = zipFileStream;
            }
            else
            {
                outStream = new DeflateStream(zipFileStream, CompressionMode.Compress, true);
            }

            zipFileEntry.Crc32 = 0 ^ 0xffffffff;

            do
            {
                bytesRead = sourceStream.Read(buffer, 0, buffer.Length);
                totalRead += (uint)bytesRead;
                if (bytesRead > 0)
                {
                    outStream.Write(buffer, 0, bytesRead);

                    for (uint i = 0; i < bytesRead; i++)
                    {
                        zipFileEntry.Crc32 = crcTable[(zipFileEntry.Crc32 ^ buffer[i]) & 0xFF] ^ (zipFileEntry.Crc32 >> 8);
                    }
                }
            }
            while (bytesRead == buffer.Length);
            outStream.Flush();

            if (zipFileEntry.Method == CompressionMethod.Deflate)
            {
                outStream.Dispose();
            }

            zipFileEntry.Crc32 ^= 0xffffffff;
            zipFileEntry.FileSize = totalRead;
            zipFileEntry.CompressedSize = (uint)(zipFileStream.Position - posStart);

            // Verify for real compression
            if (zipFileEntry.Method == CompressionMethod.Deflate && !ForceDeflating && sourceStream.CanSeek && zipFileEntry.CompressedSize > zipFileEntry.FileSize)
            {
                // Start operation again with Store algorithm
                zipFileEntry.Method = CompressionMethod.Store;
                zipFileStream.Position = posStart;
                zipFileStream.SetLength(posStart);
                sourceStream.Position = sourceStart;
                Store(ref zipFileEntry, sourceStream);
            }
        }

        /* CRC32 algorithm
          The 'magic number' for the CRC is 0xdebb20e3.
          The proper CRC pre and post conditioning
          is used, meaning that the CRC register is
          pre-conditioned with all ones (a starting value
          of 0xffffffff) and the value is post-conditioned by
          taking the one's complement of the CRC residual.
          If bit 3 of the general purpose flag is set, this
          field is set to zero in the local header and the correct
          value is put in the data descriptor and in the central
          directory.
        */
        private void UpdateCrcAndSizes(ref ZipFileEntry zipFileEntry)
        {
            var lastPos = zipFileStream.Position;  // remember position

            zipFileStream.Position = zipFileEntry.HeaderOffset + 8;
            zipFileStream.Write(BitConverter.GetBytes((ushort)zipFileEntry.Method), 0, 2);  // zipping method

            zipFileStream.Position = zipFileEntry.HeaderOffset + 14;
            zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.Crc32), 0, 4);  // Update CRC
            zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.CompressedSize), 0, 4);  // Compressed size
            zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.FileSize), 0, 4);  // Uncompressed size

            zipFileStream.Position = lastPos;  // restore position
        }

        // Reads the end-of-central-directory record
        private bool ReadFileInfo()
        {
            if (zipFileStream.Length < 22)
            {
                return false;
            }

            try
            {
                zipFileStream.Seek(-17, SeekOrigin.End);
                var br = new BinaryReader(zipFileStream);
                do
                {
                    zipFileStream.Seek(-5, SeekOrigin.Current);
                    var sig = br.ReadUInt32();
                    if (sig == 0x06054b50)
                    {
                        zipFileStream.Seek(6, SeekOrigin.Current);

                        var entries = br.ReadUInt16();
                        var centralSize = br.ReadInt32();
                        var centralDirOffset = br.ReadUInt32();
                        var commentSize = br.ReadUInt16();

                        // check if comment field is the very last data in file
                        if (zipFileStream.Position + commentSize != zipFileStream.Length)
                        {
                            return false;
                        }

                        // Copy entire central directory to a memory buffer
                        existingFileCount = entries;
                        centralDirectoryImage = new byte[centralSize];
                        zipFileStream.Seek(centralDirOffset, SeekOrigin.Begin);
                        zipFileStream.Read(centralDirectoryImage, 0, centralSize);

                        // Leave the pointer at the begining of central dir, to append new files
                        zipFileStream.Seek(centralDirOffset, SeekOrigin.Begin);
                        return true;
                    }
                }
                while (zipFileStream.Position > 0);
            }
            catch (IOException)
            {
            }

            return false;
        }

        /// <summary>
        /// Represents an entry in Zip file directory
        /// </summary>
        public struct ZipFileEntry
        {
            /// <summary>Compression method</summary>
            public CompressionMethod Method;

            /// <summary>Full path and filename as stored in Zip</summary>
            public string FilenameInZip;

            /// <summary>Original file size</summary>
            public uint FileSize;

            /// <summary>Compressed file size</summary>
            public uint CompressedSize;

            /// <summary>Offset of header information inside Zip storage</summary>
            public uint HeaderOffset;

            /// <summary>Offset of file inside Zip storage</summary>
            public uint FileOffset;

            /// <summary>Size of header information</summary>
            public uint HeaderSize;

            /// <summary>32-bit checksum of entire file</summary>
            public uint Crc32;

            /// <summary>Last modification time of file</summary>
            public DateTime ModifyTime;

            /// <summary>User comment for file</summary>
            public string Comment;

            /// <summary>True if UTF8 encoding for filename and comments, false if default (CP 437)</summary>
            public bool EncodeUTF8;

            /// <summary>Overriden method</summary>
            /// <returns>Filename in Zip</returns>
            public override string ToString()
            {
                return FilenameInZip;
            }
        }
    }
}
