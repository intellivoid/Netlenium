using System;
using System.IO;
using System.Text;

namespace Netlenium.Intellivoid
{
    internal class HttpReadBuffer
    {
        private readonly int bufferSize;
        private StringBuilder lineBuffer;
        private int totalRead;
        private int offset;
        private byte[] buffer;
        private int available;
        private bool forceNewRead;

        public bool DataAvailable => !forceNewRead && offset < available;

        public HttpReadBuffer(int size)
        {
            bufferSize = size;

            buffer = new byte[size];
        }

        public string ReadLine()
        {
            if (lineBuffer == null)
                lineBuffer = new StringBuilder();

            while (offset < available)
            {
                int c = buffer[offset++];

                totalRead++;

                if (c == '\n')
                {
                    var line = lineBuffer.ToString();

                    lineBuffer = new StringBuilder();

                    return line;
                }

                if (c != '\r')
                {
                    lineBuffer.Append((char)c);
                }
            }

            return null;
        }

        public void Reset()
        {
            lineBuffer = null;
            totalRead = 0;
        }

        public void CopyToStream(Stream stream, int maximum)
        {
            CopyToStream(stream, maximum, null);
        }

        public bool CopyToStream(Stream stream, int maximum, byte[] boundary)
        {
            var toRead = Math.Min(
                available - offset,
                maximum - totalRead
            );

            var atBoundary = false;
            var partialBoundaryMatch = false;

            if (boundary != null)
            {
                var boundaryOffset = -1;

                for (var i = 0; i < toRead; i++)
                {
                    var boundaryMatched = true;

                    for (var j = 0; j < boundary.Length; j++)
                    {
                        if (i + j >= toRead)
                        {
                            partialBoundaryMatch = true;
                            break;
                        }

                        if (buffer[i + offset + j] != boundary[j])
                        {
                            boundaryMatched = false;
                            break;
                        }
                    }

                    if (boundaryMatched)
                    {
                        boundaryOffset = i;
                        break;
                    }
                }

                if (boundaryOffset != -1)
                {
                    // If we have a boundary, we can read up until the boundary offset.

                    toRead = boundaryOffset;
                    atBoundary = !partialBoundaryMatch;

                    // When we have a partial boundary match, we need more data.

                    if (partialBoundaryMatch)
                        forceNewRead = true;
                }
            }

            stream.Write(buffer, offset, toRead);

            offset += toRead;
            totalRead += toRead;

            // If we found the boundary, we also skip it.

            if (atBoundary)
            {
                offset += boundary.Length;
                totalRead += boundary.Length;
            }
            else if (boundary != null && maximum - totalRead < boundary.Length)
            {
                throw new ProtocolException("Not enough data available for multipart boundary to match");
            }

            return atBoundary;
        }

        public bool? AtBoundary(byte[] boundary, int maximum)
        {
            if (boundary == null)
                throw new ArgumentNullException("boundary");

            if (maximum - totalRead < boundary.Length)
                throw new ProtocolException("Not enough data available for multipart boundary to match");

            if (available - offset < boundary.Length)
                return null;

            for (var i = 0; i < boundary.Length; i++)
            {
                if (boundary[i] != buffer[i + offset])
                    return false;
            }

            offset += boundary.Length;
            totalRead += boundary.Length;

            return true;
        }

        public IAsyncResult BeginRead(Stream stream, AsyncCallback callback, object state)
        {
            // A new read was requested. Reset the flag.

            forceNewRead = false;

            if (offset == available)
            {
                // If the offset is at the end, we can just reset the
                // positions.

                offset = 0;
                available = 0;
            }
            else if (buffer.Length - available < bufferSize)
            {
                // If there is less than the initial buffer size room left,
                // we need to move some data.

                if (buffer.Length - (available - offset) < bufferSize)
                {
                    // If the available size is less than the initial buffer size,
                    // enlarge the buffer.

                    var buffer = new byte[this.buffer.Length * 2];

                    // Copy the unprocessed bytes to the start of the new buffer.

                    Array.Copy(this.buffer, offset, buffer, 0, available - offset);

                    this.buffer = buffer;
                }
                else
                {
                    // Else, just move the unprocessed bytes to the beginning.

                    Array.Copy(buffer, offset, buffer, 0, available - offset);
                }

                // Reset the position and available to reflect the moved
                // bytes.

                available -= offset;
                offset = 0;
            }

            // We don't use the whole buffer, only what we were assigned in
            // the beginning. This is to prevent the read buffer from
            // resizing too much.

            var bufferAvailable = Math.Min(buffer.Length - available, bufferSize);

            return stream.BeginRead(buffer, available, bufferAvailable, callback, state);
        }

        public void EndRead(Stream stream, IAsyncResult asyncResult)
        {
            try
            {
                if (stream == null)
                {

                    Reset();
                }
                var read = stream.EndRead(asyncResult);

                available += read;
            }
            catch (ObjectDisposedException)
            {
                Reset();
            }
        }
    }
}
