using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Netlenium.Intellivoid
{
    internal class HttpMultiPartRequestParser : HttpRequestParser
    {
        private static readonly byte[] MoreBoundary = Encoding.ASCII.GetBytes("\r\n");
        private static readonly byte[] EndBoundary = Encoding.ASCII.GetBytes("--");

        private Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ParserState state = ParserState.BeforeFirstHeaders;
        private readonly byte[] firstBoundary;
        private readonly byte[] separatorBoundary;
        private bool readingFile;
        private MemoryStream fieldStream;
        private Stream fileStream;
        private string fileName;
        private bool disposed;

        public HttpMultiPartRequestParser(HttpClient client, int contentLength, string boundary)
            : base(client, contentLength)
        {
            if (boundary == null)
                throw new ArgumentNullException("boundary");

            firstBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            separatorBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary);

            Client.MultiPartItems = new List<HttpMultiPartItem>();
        }

        public override void Parse()
        {
            switch (state)
            {
                case ParserState.BeforeFirstHeaders:
                    ParseFirstHeader();
                    break;

                case ParserState.ReadingHeaders:
                    ParseHeaders();
                    break;

                case ParserState.ReadingContent:
                    ParseContent();
                    break;

                case ParserState.ReadingBoundary:
                    ParseBoundary();
                    break;
            }
        }

        private void ParseFirstHeader()
        {
            var atBoundary = Client.ReadBuffer.AtBoundary(firstBoundary, ContentLength);

            if (atBoundary.HasValue)
            {
                if (!atBoundary.Value)
                    throw new ProtocolException("Expected multipart content to start with the boundary");

                state = ParserState.ReadingHeaders;

                ParseHeaders();
            }
        }

        private void ParseHeaders()
        {
            string line;

            while ((line = Client.ReadBuffer.ReadLine()) != null)
            {
                string[] parts;

                if (line.Length == 0)
                {
                    // Test whether we're reading a file or a field.

                    string contentDispositionHeader;

                    if (!headers.TryGetValue("Content-Disposition", out contentDispositionHeader))
                        throw new ProtocolException("Expected Content-Disposition header with multipart");

                    parts = contentDispositionHeader.Split(';');

                    readingFile = false;

                    for (var i = 0; i < parts.Length; i++)
                    {
                        var part = parts[i].Trim();

                        if (part.StartsWith("filename=", StringComparison.OrdinalIgnoreCase))
                        {
                            readingFile = true;
                            break;
                        }
                    }

                    // Prepare our state for whether we're reading a file
                    // or a field.

                    if (readingFile)
                    {
                        fileName = Path.GetTempFileName();
                        fileStream = File.Create(fileName, 4096, FileOptions.DeleteOnClose);
                    }
                    else
                    {
                        if (fieldStream == null)
                        {
                            fieldStream = new MemoryStream();
                        }
                        else
                        {
                            fieldStream.Position = 0;
                            fieldStream.SetLength(0);
                        }
                    }

                    state = ParserState.ReadingContent;

                    ParseContent();
                    return;
                }

                parts = line.Split(new[] { ':' }, 2);

                if (parts.Length != 2)
                    throw new ProtocolException("Received header without colon");

                headers[parts[0].Trim()] = parts[1].Trim();
            }
        }

        private void ParseContent()
        {
            var result = Client.ReadBuffer.CopyToStream(
                readingFile ? fileStream : fieldStream,
                ContentLength,
                separatorBoundary
            );

            if (result)
            {
                string value = null;
                Stream stream = null;

                if (!readingFile)
                {
                    value = Encoding.ASCII.GetString(fieldStream.ToArray());
                }
                else
                {
                    stream = fileStream;
                    fileStream = null;

                    stream.Position = 0;
                }

                Client.MultiPartItems.Add(new HttpMultiPartItem(headers, value, stream));

                headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                state = ParserState.ReadingBoundary;

                ParseBoundary();
            }
        }

        private void ParseBoundary()
        {
            var atMore = Client.ReadBuffer.AtBoundary(MoreBoundary, ContentLength);

            if (atMore.HasValue)
            {
                if (atMore.Value)
                {
                    state = ParserState.ReadingHeaders;

                    ParseHeaders();
                }
                else
                {
                    var atEnd = Client.ReadBuffer.AtBoundary(EndBoundary, ContentLength);

                    // The more and end boundaries have the same length.

                    Debug.Assert(atEnd.HasValue);

                    if (!atEnd.Value)
                        throw new ProtocolException("Unexpected content after boundary");

                    EndParsing();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (fieldStream != null)
                {
                    fieldStream.Dispose();
                    fieldStream = null;
                }

                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }

                disposed = true;
            }

            base.Dispose(disposing);
        }

        private enum ParserState
        {
            BeforeFirstHeaders,
            ReadingHeaders,
            ReadingContent,
            ReadingBoundary
        }
    }
}
