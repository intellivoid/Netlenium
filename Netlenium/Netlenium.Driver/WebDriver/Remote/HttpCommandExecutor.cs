using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a way of executing Commands over HTTP
    /// </summary>
    public class HttpCommandExecutor : ICommandExecutor
    {
        private const string JsonMimeType = "application/json";
        private const string PngMimeType = "image/png";
        private const string CharsetType = "charset=utf-8";
        private const string ContentTypeHeader = JsonMimeType + ";" + CharsetType;
        private const string RequestAcceptHeader = JsonMimeType + ", " + PngMimeType;
        private const string UserAgentHeaderTemplate = "selenium/{0} (.net {1})";
        private Uri remoteServerUri;
        private TimeSpan serverResponseTimeout;
        private bool enableKeepAlive;
        private bool isDisposed;
        private IWebProxy proxy;
        private CommandInfoRepository commandInfoRepository = new WebDriverWireProtocolCommandInfoRepository();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCommandExecutor"/> class
        /// </summary>
        /// <param name="addressOfRemoteServer">Address of the WebDriver Server</param>
        /// <param name="timeout">The timeout within which the server must respond.</param>
        public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout)
            : this(addressOfRemoteServer, timeout, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCommandExecutor"/> class
        /// </summary>
        /// <param name="addressOfRemoteServer">Address of the WebDriver Server</param>
        /// <param name="timeout">The timeout within which the server must respond.</param>
        /// <param name="enableKeepAlive"><see langword="true"/> if the KeepAlive header should be sent
        /// with HTTP requests; otherwise, <see langword="false"/>.</param>
        public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout, bool enableKeepAlive)
        {
            if (addressOfRemoteServer == null)
            {
                throw new ArgumentNullException("addressOfRemoteServer", "You must specify a remote address to connect to");
            }

            if (!addressOfRemoteServer.AbsoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                addressOfRemoteServer = new Uri(addressOfRemoteServer.ToString() + "/");
            }

            remoteServerUri = addressOfRemoteServer;
            serverResponseTimeout = timeout;
            this.enableKeepAlive = enableKeepAlive;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 2000;

            // In the .NET Framework, HttpWebRequest responses with an error code are limited
            // to 64k by default. Since the remote server error responses include a screenshot,
            // they can frequently exceed this size. This only applies to the .NET Framework;
            // Mono does not implement the property.
            if (Type.GetType("Mono.Runtime", false, true) == null)
            {
                HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            }


        }

        public event EventHandler<SendingRemoteHttpRequestEventArgs> SendingRemoteHttpRequest;

        /// <summary>
        /// Gets the repository of objects containin information about commands.
        /// </summary>
        public CommandInfoRepository CommandInfoRepository
        {
            get { return commandInfoRepository; }
        }

        /// <summary>
        /// Gets or sets an <see cref="IWebProxy"/> object to be used to proxy requests
        /// between this <see cref="HttpCommandExecutor"/> and the remote end WebDriver
        /// implementation.
        /// </summary>
        public IWebProxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether keep-alive is enabled for HTTP
        /// communication between this <see cref="HttpCommandExecutor"/> and the
        /// remote end WebDriver implementation.
        /// </summary>
        public bool IsKeepAliveEnabled
        {
            get { return enableKeepAlive; }
            set { enableKeepAlive = value; }
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="commandToExecute">The command you wish to execute</param>
        /// <returns>A response from the browser</returns>
        public virtual Response Execute(Command commandToExecute)
        {
            if (commandToExecute == null)
            {
                throw new ArgumentNullException("commandToExecute", "commandToExecute cannot be null");
            }

            var info = commandInfoRepository.GetCommandInfo(commandToExecute.Name);
            var requestInfo = new HttpRequestInfo(remoteServerUri, commandToExecute, info);

            var responseInfo = MakeHttpRequest(requestInfo);

            var toReturn = CreateResponse(responseInfo);
            if (commandToExecute.Name == DriverCommand.NewSession && toReturn.IsSpecificationCompliant)
            {
                // If we are creating a new session, sniff the response to determine
                // what protocol level we are using. If the response contains a
                // field called "status", it's not a spec-compliant response.
                // Each response is polled for this, and sets a property describing
                // whether it's using the W3C protocol dialect.
                // TODO(jimevans): Reverse this test to make it the default path when
                // most remote ends speak W3C, then remove it entirely when legacy
                // protocol is phased out.
                commandInfoRepository = new W3CWireProtocolCommandInfoRepository();
            }

            return toReturn;
        }

        /// <summary>
        /// Raises the <see cref="SendingRemoteHttpRequest"/> event.
        /// </summary>
        /// <param name="eventArgs">A <see cref="SendingRemoteHttpRequestEventArgs"/> that contains the event data.</param>
        protected virtual void OnSendingRemoteHttpRequest(SendingRemoteHttpRequestEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs", "eventArgs must not be null");
            }

            if (SendingRemoteHttpRequest != null)
            {
                SendingRemoteHttpRequest(this, eventArgs);
            }
        }

        private static string GetTextOfWebResponse(HttpWebResponse webResponse)
        {
            // StreamReader.Close also closes the underlying stream.
            var responseStream = webResponse.GetResponseStream();
            var responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);
            var responseString = responseStreamReader.ReadToEnd();
            responseStreamReader.Close();

            // The response string from the Java remote server has trailing null
            // characters. This is due to the fix for issue 288.
            if (responseString.IndexOf('\0') >= 0)
            {
                responseString = responseString.Substring(0, responseString.IndexOf('\0'));
            }

            return responseString;
        }

        private HttpResponseInfo MakeHttpRequest(HttpRequestInfo requestInfo)
        {
            var request = WebRequest.Create(requestInfo.FullUri) as HttpWebRequest;
            if (!string.IsNullOrEmpty(requestInfo.FullUri.UserInfo) && requestInfo.FullUri.UserInfo.Contains(":"))
            {
                var userInfo = remoteServerUri.UserInfo.Split(new char[] { ':' }, 2);
                request.Credentials = new NetworkCredential(userInfo[0], userInfo[1]);
                request.PreAuthenticate = true;
            }

            var userAgentString = string.Format(CultureInfo.InvariantCulture, UserAgentHeaderTemplate, ResourceUtilities.AssemblyVersion, ResourceUtilities.PlatformFamily);
            request.UserAgent = userAgentString;
            request.Method = requestInfo.HttpMethod;
            request.Timeout = (int)serverResponseTimeout.TotalMilliseconds;
            request.Accept = RequestAcceptHeader;
            request.KeepAlive = enableKeepAlive;
            request.Proxy = proxy;
            request.ServicePoint.ConnectionLimit = 2000;
            if (request.Method == CommandInfo.GetCommand)
            {
                request.Headers.Add("Cache-Control", "no-cache");
            }

            var eventArgs = new SendingRemoteHttpRequestEventArgs(request, requestInfo.RequestBody);
            OnSendingRemoteHttpRequest(eventArgs);

            if (request.Method == CommandInfo.PostCommand)
            {
                var payload = eventArgs.RequestBody;
                var data = Encoding.UTF8.GetBytes(payload);
                request.ContentType = ContentTypeHeader;
                var requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            var responseInfo = new HttpResponseInfo();
            HttpWebResponse webResponse = null;
            try
            {
                webResponse = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                webResponse = ex.Response as HttpWebResponse;
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    var timeoutMessage = "The HTTP request to the remote WebDriver server for URL {0} timed out after {1} seconds.";
                    throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, timeoutMessage, request.RequestUri.AbsoluteUri, serverResponseTimeout.TotalSeconds), ex);
                }
                else if (ex.Response == null)
                {
                    var nullResponseMessage = "A exception with a null response was thrown sending an HTTP request to the remote WebDriver server for URL {0}. The status of the exception was {1}, and the message was: {2}";
                    throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, nullResponseMessage, request.RequestUri.AbsoluteUri, ex.Status, ex.Message), ex);
                }
            }

            if (webResponse == null)
            {
                throw new WebDriverException("No response from server for url " + request.RequestUri.AbsoluteUri);
            }
            else
            {
                responseInfo.Body = GetTextOfWebResponse(webResponse);
                responseInfo.ContentType = webResponse.ContentType;
                responseInfo.StatusCode = webResponse.StatusCode;
            }

            return responseInfo;
        }

        private Response CreateResponse(HttpResponseInfo stuff)
        {
            var commandResponse = new Response();
            var responseString = stuff.Body;
            if (stuff.ContentType != null && stuff.ContentType.StartsWith(JsonMimeType, StringComparison.OrdinalIgnoreCase))
            {
                commandResponse = Response.FromJson(responseString);
            }
            else
            {
                commandResponse.Value = responseString;
            }

            if (commandInfoRepository.SpecificationLevel < 1 && (stuff.StatusCode < HttpStatusCode.OK || stuff.StatusCode >= HttpStatusCode.BadRequest))
            {
                // 4xx represents an unknown command or a bad request.
                if (stuff.StatusCode >= HttpStatusCode.BadRequest && stuff.StatusCode < HttpStatusCode.InternalServerError)
                {
                    commandResponse.Status = WebDriverResult.UnhandledError;
                }
                else if (stuff.StatusCode >= HttpStatusCode.InternalServerError)
                {
                    // 5xx represents an internal server error. The response status should already be set, but
                    // if not, set it to a general error code. The exception is a 501 (NotImplemented) response,
                    // which indicates that the command hasn't been implemented on the server.
                    if (stuff.StatusCode == HttpStatusCode.NotImplemented)
                    {
                        commandResponse.Status = WebDriverResult.UnknownCommand;
                    }
                    else
                    {
                        if (commandResponse.Status == WebDriverResult.Success)
                        {
                            commandResponse.Status = WebDriverResult.UnhandledError;
                        }
                    }
                }
                else
                {
                    commandResponse.Status = WebDriverResult.UnhandledError;
                }
            }

            if (commandResponse.Value is string)
            {
                // First, collapse all \r\n pairs to \n, then replace all \n with
                // System.Environment.NewLine. This ensures the consistency of
                // the values.
                commandResponse.Value = ((string)commandResponse.Value).Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
            }

            return commandResponse;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="HttpCommandExecutor"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HttpCommandExecutor"/> and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release managed and resources;
        /// <see langword="false"/> to only release unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
            }
        }

        private class HttpRequestInfo
        {
            public HttpRequestInfo(Uri serverUri, Command commandToExecute, CommandInfo commandInfo)
            {
                FullUri = commandInfo.CreateCommandUri(serverUri, commandToExecute);
                HttpMethod = commandInfo.Method;
                RequestBody = commandToExecute.ParametersAsJsonString;
            }

            public Uri FullUri { get; set; }
            public string HttpMethod { get; set; }
            public string RequestBody { get; set; }
        }

        private class HttpResponseInfo
        {
            public HttpStatusCode StatusCode { get; set; }
            public string Body { get; set; }
            public string ContentType { get; set; }
        }
    }
}
