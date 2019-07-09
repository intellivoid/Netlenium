using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Intellivoid.HyperWS
{
    /// <inheritdoc />
    /// <summary>
    /// HttpClient Internal Class
    /// </summary>
    internal class HttpClient : IDisposable
    {

        /// <summary>
        /// Prolog Regex Pattern
        /// </summary>
        private static readonly Regex PrologRegex = new Regex("^([A-Z]+) ([^ ]+) (HTTP/[^ ]+)$", RegexOptions.Compiled);

        /// <summary>
        /// Indicates if this object was disposed or not
        /// </summary>
        private bool _disposed;
        
        /// <summary>
        /// The current write buffer for the client
        /// </summary>
        private readonly byte[] _writeBuffer;
        
        /// <summary>
        /// The incoming/outgoing network stream for this client
        /// </summary>
        private NetworkStream _stream;
        
        /// <summary>
        /// The current state of the client
        /// </summary>
        private ClientState _state;
        
        /// <summary>
        /// The current write stream
        /// </summary>
        private MemoryStream _writeStream;
        
        /// <summary>
        /// HTTP Request Parser
        /// </summary>
        private HttpRequestParser _parser;
        
        /// <summary>
        /// HTTP Context
        /// </summary>
        private HttpContext _context;
        
        /// <summary>
        /// If there is an ongoing error with the client
        /// </summary>
        private bool _errored;

        /// <summary>
        /// The server that this client is connected to
        /// </summary>
        public HttpServer Server { get; private set; }

        /// <summary>
        /// The TCP Client used the communicate with the client
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Request Method variable
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Protocol
        /// </summary>
        public string Protocol { get; private set; }

        /// <summary>
        /// Request
        /// </summary>
        public string Request { get; private set; }

        /// <summary>
        /// Headers
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Post Parameters
        /// </summary>
        public NameValueCollection PostParameters { get; set; }

        /// <summary>
        /// Multi Part Items
        /// </summary>
        public List<HttpMultiPartItem> MultiPartItems { get; set; }

        /// <summary>
        /// Read Buffer
        /// </summary>
        public HttpReadBuffer ReadBuffer { get; private set; }

        /// <summary>
        /// Input Stream
        /// </summary>
        public Stream InputStream { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        public HttpClient(HttpServer server, TcpClient client)
        {
            Server = server ?? throw new ArgumentNullException("server");
            TcpClient = client ?? throw new ArgumentNullException("client");

            ReadBuffer = new HttpReadBuffer(server.ReadBufferSize);
            _writeBuffer = new byte[server.WriteBufferSize];

            _stream = client.GetStream();
        }

        /// <summary>
        /// Resets the client
        /// </summary>
        private void Reset()
        {
            _state = ClientState.ReadingProlog;
            _context = null;

            if (_parser != null)
            {
                _parser.Dispose();
                _parser = null;
            }

            if (_writeStream != null)
            {
                _writeStream.Dispose();
                _writeStream = null;
            }

            if (InputStream != null)
            {
                InputStream.Dispose();
                InputStream = null;
            }

            ReadBuffer.Reset();

            Method = null;
            Protocol = null;
            Request = null;
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            PostParameters = new NameValueCollection();

            if (MultiPartItems != null)
            {
                foreach (var item in MultiPartItems)
                {
                    if (item.Stream != null)
                        item.Stream.Dispose();
                }

                MultiPartItems = null;
            }
        }

        /// <summary>
        /// Starts the reading the request
        /// </summary>
        public void BeginRequest()
        {
            Reset();

            BeginRead();
        }

        /// <summary>
        /// Begins reading the incoming stream
        /// </summary>
        private void BeginRead()
        {
            if (_disposed)
                return;

            try
            {
                Server.TimeoutManager.ReadQueue.Add(
                    ReadBuffer.BeginRead(_stream, ReadCallback, null),
                    this
                );
            }
            catch (Exception ex)
            {
                //Logging.WriteEntry(LogType.Information, "HyperWS", $"BeginRead failed {ex}");

                Dispose();
            }
        }

        /// <summary>
        /// Reads as a callback function
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReadCallback(IAsyncResult asyncResult)
        {
            if (_disposed)
                return;

            // The below state matches the RequestClose state. Dispose immediately
            // when this occurs.

            if (
                _state == ClientState.ReadingProlog &&
                Server.State != HttpServerState.Started
            )
            {
                Dispose();
                return;
            }

            try
            {
                if (ReadBuffer == null)
                {
                    Dispose();
                }
                try
                {
                    ReadBuffer.EndRead(_stream, asyncResult);
                }
                catch
                {
                    // This happens when the bugger stream remains null, this can either
                    // be a slow-loris attack or malformed request.
                }
                

                if (ReadBuffer.DataAvailable)
                {
                    ProcessReadBuffer();
                }
                else
                {
                    Dispose();
                }
                    
            }
            catch (ObjectDisposedException ex)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "HyperWS", $"Failed to read {ex}");
                Dispose();
            }
            catch (Exception ex)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "HyperWS", $"Failed to read from the HTTP connection {ex}");
                ProcessException(ex);
            }
        }

        /// <summary>
        /// Processes the read buffer
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ProcessReadBuffer()
        {
            while (_writeStream == null && ReadBuffer.DataAvailable)
            {
                switch (_state)
                {
                    case ClientState.ReadingProlog:
                        ProcessProlog();
                        break;

                    case ClientState.ReadingHeaders:
                        ProcessHeaders();
                        break;

                    case ClientState.ReadingContent:
                        ProcessContent();
                        break;

                    default:
                        throw new InvalidOperationException("Invalid state");
                }
            }

            if (_writeStream == null)
                BeginRead();
        }

        /// <summary>
        /// Processes the Prolog
        /// </summary>
        /// <exception cref="ProtocolException"></exception>
        private void ProcessProlog()
        {
            string line = ReadBuffer.ReadLine();

            if (line == null)
                return;

            var match = PrologRegex.Match(line);

            if (!match.Success)
                throw new ProtocolException($"Could not parse prolog '{line}'");

            Method = match.Groups[1].Value;
            Request = match.Groups[2].Value;
            Protocol = match.Groups[3].Value;

            // Continue reading the headers.

            _state = ClientState.ReadingHeaders;

            ProcessHeaders();
        }

        /// <summary>
        /// Processes the headers
        /// </summary>
        /// <exception cref="ProtocolException"></exception>
        private void ProcessHeaders()
        {
            string line;

            while ((line = ReadBuffer.ReadLine()) != null)
            {
                // Have we completed receiving the headers?

                if (line.Length == 0)
                {
                    // Reset the read buffer which resets the bytes read.

                    ReadBuffer.Reset();

                    // Start processing the body of the request.

                    _state = ClientState.ReadingContent;

                    ProcessContent();

                    return;
                }

                var parts = line.Split(new[] { ':' }, 2);

                if (parts.Length != 2)
                    throw new ProtocolException("Received header without colon");

                Headers[parts[0].Trim()] = parts[1].Trim();
            }
        }

        /// <summary>
        /// Processes content
        /// </summary>
        private void ProcessContent()
        {
            if (_parser != null)
            {
                _parser.Parse();
                return;
            }

            if (ProcessExpectHeader())
                return;

            if (ProcessContentLengthHeader())
                return;

            // The request has been completely parsed now.

            ExecuteRequest();
        }

        /// <summary>
        /// Processes a expected header
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ProtocolException"></exception>
        private bool ProcessExpectHeader()
        {
            if (!Headers.TryGetValue("Expect", out string expectHeader)) return false;
            // Remove the expect header for the next run.

            Headers.Remove("Expect");

            var pos = expectHeader.IndexOf(';');

            if (pos != -1)
                expectHeader = expectHeader.Substring(0, pos).Trim();

            if (!string.Equals("100-continue", expectHeader, StringComparison.OrdinalIgnoreCase))
                throw new ProtocolException($"Could not process Expect header '{expectHeader}'");

            SendContinueResponse();
            return true;

        }
        
        /// <summary>
        /// Processes the content length and starts reading the content
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ProtocolException"></exception>
        private bool ProcessContentLengthHeader()
        {
            // Read the content.
            if (!Headers.TryGetValue("Content-Length", out var contentLengthHeader)) return false;
            
            if (!int.TryParse(contentLengthHeader, out var contentLength))
                throw new ProtocolException($"Could not parse Content-Length header '{contentLengthHeader}'");

            string contentType = null;
            string contentTypeExtra = null;

            if (Headers.TryGetValue("Content-Type", out var contentTypeHeader))
            {
                var parts = contentTypeHeader.Split(new[] { ';' }, 2);

                contentType = parts[0].Trim().ToLowerInvariant();
                contentTypeExtra = parts.Length == 2 ? parts[1].Trim() : null;
            }

            if (_parser != null)
            {
                _parser.Dispose();
                _parser = null;
            }

            switch (contentType)
            {
                case "application/x-www-form-urlencoded":
                    _parser = new HttpUrlEncodedRequestParser(this, contentLength);
                    break;

                case "multipart/form-data":
                    string boundary = null;

                    if (contentTypeExtra != null)
                    {
                        var parts = contentTypeExtra.Split(new[] { '=' }, 2);

                        if (parts.Length == 2 && string.Equals(parts[0], "boundary", StringComparison.OrdinalIgnoreCase))
                        {
                            boundary = parts[1];
                        }
                    }

                    if (boundary == null)
                    {
                        throw new ProtocolException("Expected boundary with multipart content type");
                    }

                    _parser = new HttpMultiPartRequestParser(this, contentLength, boundary);
                    break;

                default:
                    _parser = new HttpUnknownRequestParser(this, contentLength);
                    break;
            }

            // We've made a parser available. Recurs back to start processing
            // with the parser.

            ProcessContent();
            return true;

        }

        /// <summary>
        /// Sends a continue request to expect more data
        /// </summary>
        private void SendContinueResponse()
        {
            var sb = new StringBuilder();

            sb.Append(Protocol);
            sb.Append(" 100 Continue\r\nServer: ");
            sb.Append(Server.ServerBanner);
            sb.Append("\r\nDate: ");
            sb.Append(DateTime.UtcNow.ToString("R"));
            sb.Append("\r\n\r\n");

            var bytes = Encoding.ASCII.GetBytes(sb.ToString());

            _writeStream?.Dispose();

            _writeStream = new MemoryStream();
            _writeStream.Write(bytes, 0, bytes.Length);
            _writeStream.Position = 0;

            BeginWrite();
        }

        /// <summary>
        /// Begins writing to the Write Stream
        /// </summary>
        private void BeginWrite()
        {
            try
            {
                // Copy the next part from the write stream.

                var read = _writeStream.Read(_writeBuffer, 0, _writeBuffer.Length);

                Server.TimeoutManager.WriteQueue.Add(
                    _stream.BeginWrite(_writeBuffer, 0, read, WriteCallback, null),
                    this
                );
            
            }
            catch (IOException)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Warning, "HyperWS", $"The HTTP server wasn't able to write to the stream because the stream was terminated by the client/host machine. Priority=NORMAL");
                
                Dispose();
            }
            catch (Exception ex)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "HyperWS", $"BeginWrite failed {ex}");

                Dispose();
            }
        }

        /// <summary>
        /// Writes to the Write Stream, but executes the callback function
        /// </summary>
        /// <param name="asyncResult"></param>
        private void WriteCallback(IAsyncResult asyncResult)
        {
            if (_disposed)
                return;

            try
            {
                _stream.EndWrite(asyncResult);

                if (_writeStream != null && _writeStream.Length != _writeStream.Position)
                {
                    // Continue writing from the write stream.

                    BeginWrite();
                }
                else
                {
                    if (_writeStream != null)
                    {
                        _writeStream.Dispose();
                        _writeStream = null;
                    }

                    switch (_state)
                    {
                        case ClientState.WritingHeaders:
                            WriteResponseContent();
                            break;

                        case ClientState.WritingContent:
                            ProcessRequestCompleted();
                            break;

                        default:
                            Debug.Assert(_state != ClientState.Closed);

                            if (ReadBuffer.DataAvailable)
                            {
                                try
                                {
                                    ProcessReadBuffer();
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(ex);
                                }
                            }
                            else
                            {
                                BeginRead();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Warning, "HyperWS", $"Failed to write {ex}");

                Dispose();
            }
        }

        /// <summary>
        /// Executes the request
        /// </summary>
        public void ExecuteRequest()
        {
            _context = new HttpContext(this);

            NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Verbose, "HyperWS", $"Accepted request {_context.Request.RawUrl.Replace(Environment.NewLine, string.Empty)}");

            Server.RaiseRequest(_context);

            WriteResponseHeaders();
        }

        /// <summary>
        /// Writes the response headers
        /// </summary>
        private void WriteResponseHeaders()
        {
            var headers = BuildResponseHeaders();

            _writeStream?.Dispose();

            _writeStream = new MemoryStream(headers);

            _state = ClientState.WritingHeaders;

            BeginWrite();
        }

        /// <summary>
        /// Builds Response Headers
        /// </summary>
        /// <returns></returns>
        private byte[] BuildResponseHeaders()
        {
            var response = _context.Response;
            var sb = new StringBuilder();

            // Write the prolog.
            sb.Append(Protocol);
            sb.Append(' ');
            sb.Append(response.StatusCode);

            if (!string.IsNullOrEmpty(response.StatusDescription))
            {
                sb.Append(' ');
                sb.Append(response.StatusDescription);
            }

            sb.Append("\r\n");

            // Write all headers provided by Response.
            if (!string.IsNullOrEmpty(response.CacheControl))
                WriteHeader(sb, "Cache-Control", response.CacheControl);

            if (!string.IsNullOrEmpty(response.ContentType))
            {
                var contentType = response.ContentType;

                if (!string.IsNullOrEmpty(response.CharSet))
                    contentType += "; charset=" + response.CharSet;

                WriteHeader(sb, "Content-Type", contentType);
            }

            WriteHeader(sb, "Expires", response.ExpiresAbsolute.ToString("R"));

            if (!string.IsNullOrEmpty(response.RedirectLocation))
                WriteHeader(sb, "Location", response.RedirectLocation);

            // Write the remainder of the headers.
            foreach (var key in response.Headers.AllKeys)
            {
                WriteHeader(sb, key, response.Headers[key]);
            }

            // Write the content length (we override custom headers for this).
            WriteHeader(sb, "Content-Length", response.OutputStream.BaseStream.Length.ToString(CultureInfo.InvariantCulture));

            for (var i = 0; i < response.Cookies.Count; i++)
            {
                WriteHeader(sb, "Set-Cookie", response.Cookies[i].GetHeaderValue());
            }

            sb.Append("\r\n");

            return response.HeadersEncoding.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Writes to the given header
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private static void WriteHeader(StringBuilder sb, string key, string value)
        {
            sb.Append(key);
            sb.Append(": ");
            sb.Append(value);
            sb.Append("\r\n");
        }

        /// <summary>
        /// Writes Response Content
        /// </summary>
        private void WriteResponseContent()
        {
            _writeStream?.Dispose();

            _writeStream = _context.Response.OutputStream.BaseStream;
            _writeStream.Position = 0;

            _state = ClientState.WritingContent;

            BeginWrite();
        }

        /// <summary>
        /// Process execution when the request has been completed
        /// </summary>
        private void ProcessRequestCompleted()
        {

            // Do not accept new requests when the server is stopping.

            if (
                !_errored &&
                Server.State == HttpServerState.Started &&
                Headers.TryGetValue("Connection", out string connectionHeader) &&
                string.Equals(connectionHeader, "keep-alive", StringComparison.OrdinalIgnoreCase)
            )
                BeginRequest();
            else
                Dispose();
        }

        /// <summary>
        /// Closes the request
        /// </summary>
        public void RequestClose()
        {
            if (_state != ClientState.ReadingProlog) return;
            var stream = _stream;

            stream?.Dispose();
        }

        /// <summary>
        /// Forces the close
        /// </summary>
        public void ForceClose()
        {
            var stream = _stream;

            stream?.Dispose();
        }

        /// <summary>
        /// Unsets the parser
        /// </summary>
        public void UnsetParser()
        {
            Debug.Assert(_parser != null);

            _parser = null;
        }

        /// <summary>
        /// Processes the given exception
        /// </summary>
        /// <param name="exception"></param>
        private void ProcessException(Exception exception)
        {
            if (_disposed)
                return;

            _errored = true;

            // If there is no request available, the error didn't occur as part
            // of a request (e.g. the client closed the connection). Just close
            // the channel in that case.

            if (Request == null)
            {
                Dispose();
                return;
            }

            try
            {
                if (_context == null)
                    _context = new HttpContext(this);

                _context.Response.Status = "500 Internal Server Error";

                bool handled;

                try
                {
                    handled = Server.RaiseUnhandledException(_context, exception);
                }
                catch
                {
                    handled = false;
                }

                if (!handled && _context.Response.OutputStream.CanWrite)
                {
                    var resourceName = GetType().Namespace + ".Resources.InternalServerError.html";

                    using (var stream = GetType().Assembly.GetManifestResourceStream(resourceName))
                    {
                        var buffer = new byte[4096];
                        int read;

                        while (stream != null && (read = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            _context.Response.OutputStream.Write(buffer, 0, read);
                        }
                    }
                }

                WriteResponseHeaders();
            }
            catch (Exception ex)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Warning, "HyperWS", $"Failed to process internal server error response {ex}");

                Dispose();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Disposes this HttpClient Object
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            Server.UnregisterClient(this);

            _state = ClientState.Closed;

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            if (TcpClient != null)
            {
                TcpClient.Close();
                TcpClient = null;
            }

            Reset();

            _disposed = true;
        }

        /// <summary>
        /// The current state of the client
        /// </summary>
        private enum ClientState
        {
            ReadingProlog,
            ReadingHeaders,
            ReadingContent,
            WritingHeaders,
            WritingContent,
            Closed
        }
    }
}
