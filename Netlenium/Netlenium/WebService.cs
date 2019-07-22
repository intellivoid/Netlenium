using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Netlenium.Handlers;
using Netlenium.Intellivoid;
using Netlenium.Logging;
using Netlenium.Responses;
using Newtonsoft.Json;

namespace Netlenium
{
    /// <summary>
    /// The main Web Service that handles requests
    /// </summary>
    public static class WebService
    {
        /// <summary>
        /// Private Server Object
        /// </summary>
        private static HttpServer server;

        /// <summary>
        /// Logging configuration for the Web Server
        /// </summary>
        public static Service Logging = new Service();

        private static CancellationToken cancellationToken;

        /// <summary>
        /// The directory of the executing 
        /// </summary>
        private static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(File.Exists($"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}Netlenium.so") ? path : Process.GetCurrentProcess().MainModule.FileName);
            }
        }

        /// <summary>
        /// Starts the Web Service on a random port, if port is set to another value other than 0
        /// it will use that port instead of a random port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string Start(int port = 0)
        {
            server = new HttpServer();

            if (port != 0)
            {
                server.EndPoint = new IPEndPoint(IPAddress.Loopback, port);
            }

            server.RequestReceived += (s, e) => { RequestReceived(e); };
            server.Start();

            cancellationToken = new CancellationToken();
            Task.Run(async () => await BackgroundTasks(cancellationToken));

            return $"http://{server.EndPoint}/";
        }

        /// <summary>
        /// Stops the HTTP Server gracefully
        /// </summary>
        public static void Stop()
        {
            server.Stop();
        }


        /// <summary>
        /// Sends a response back to the client
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="content"></param>
        public static void SendResponse(HttpResponse httpResponse, string content)
        {
            httpResponse.Headers.Add("X-Powered-By", "Netlenium");
            using (var writer = new StreamWriter(httpResponse.OutputStream))
            {
                writer.Write(content);
            }
        }

        /// <summary>
        /// Sends a JSON Response back to the client
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="content"></param>
        /// <param name="statusCode"></param>
        public static void SendJsonResponse(HttpResponse httpResponse, object content, int statusCode = 200)
        {
            httpResponse.StatusCode = statusCode;
            httpResponse.Headers.Add("content-Type", "application/json");

            SendResponse(httpResponse, JsonConvert.SerializeObject(content, Formatting.None));
        }

        /// <summary>
        /// Sends a response back to the client as a file
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="filepath"></param>
        public static void SendFile(HttpResponse httpResponse, string filepath)
        {
            httpResponse.Headers.Add("X-Powered-By", "Netlenium Framework");

            using (var stream = File.OpenRead(filepath))
            {
                stream.CopyTo(httpResponse.OutputStream);
            }
        }

        /// <summary>
        /// Determines if the request is authorized
        /// </summary>
        /// <param name="httpRequestEvent"></param>
        /// <returns></returns>
        public static bool IsAuthorized(HttpRequestEventArgs httpRequestEvent)
        {
            if(CommandLineParameters.AuthPassword == string.Empty)
            {
                return true;
            }

            var authenticationPassword = GetParameter(httpRequestEvent.Request, "auth");

            if (authenticationPassword == null)
            {
                Logging.WriteEntry(MessageType.Warning, "WebService", "Authentication Failed, Reason: Missing Parameter 'auth'");
                return false;
            }

            if(authenticationPassword != CommandLineParameters.AuthPassword)
            {
                Logging.WriteEntry(MessageType.Warning, "WebService", "Authentication Failed, Reason: Incorrect Password");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies if the given session is valid
        /// </summary>
        /// <param name="httpRequestEvent"></param>
        /// <returns></returns>
        public static bool VerifySession(HttpRequestEventArgs httpRequestEvent)
        {
            var sessionId = GetParameter(httpRequestEvent.Request, "session_id");

            if (sessionId == null)
            {
                SendJsonResponse(httpRequestEvent.Response, new MissingParameterResponse("session_id"), 400);
                return false;
            }

            if (SessionManager.SessionExpired(sessionId))
            {
                SendJsonResponse(httpRequestEvent.Response, new SessionExpiredResponse(), 400);
                return false;
            }

            if(SessionManager.SessionExists(sessionId) == false)
            {
                SendJsonResponse(httpRequestEvent.Response, new SessionNotFoundResponse(sessionId), 404);
                return false;
            }

            SessionManager.ActiveSessions[sessionId].LastActivity = DateTime.Now;

            // TODO: Check if the driver is still running on the session

            return true;
        }

        /// <summary>
        /// Raised when a request is received
        /// </summary>
        /// <param name="httpRequestEvent"></param>
        private static void RequestReceived(HttpRequestEventArgs httpRequestEvent)
        {
            if (httpRequestEvent.Request.RequestType.ToUpper() != "GET" && httpRequestEvent.Request.RequestType.ToUpper() != "POST")
            {
                SendJsonResponse(httpRequestEvent.Response, new UnsupportedRequestMethodException(), 400);
                return;
            }

            var requestPath = httpRequestEvent.Request.Path.ToLower().Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if(requestPath.Length > 0)
            {
                try
                {
                    switch (requestPath[0])
                    {
                        case "actions":
                            Actions.HandleRequest(requestPath, httpRequestEvent);
                            break;

                        case "navigation":
                            Navigation.HandleRequest(requestPath, httpRequestEvent);
                            break;

                        case "sessions":
                            Sessions.HandleRequest(requestPath, httpRequestEvent);
                            break;

                        case "web_element":
                            WebElement.HandleRequest(requestPath, httpRequestEvent);
                            break;

                        case "window_handler":
                            WindowHandler.HandleRequest(requestPath, httpRequestEvent);
                            break;

                        case "favicon.ico":
                            var faviconLocation = $"{AssemblyDirectory}{Path.DirectorySeparatorChar}WebResources{Path.DirectorySeparatorChar}favicon.ico";
                            if (File.Exists(faviconLocation))
                            {
                                httpRequestEvent.Response.Headers.Add("Content-Type", "image/ico");
                                SendFile(httpRequestEvent.Response, faviconLocation);
                            }
                            break;

                        default:
                            SendJsonResponse(httpRequestEvent.Response, new NotFoundResponse(), 404);
                            break;
                    }
                }
                catch(Exception ex)
                {
                    SendJsonResponse(httpRequestEvent.Response, new InternalServerErrorResponse(ex), 500);
                    return;
                }
                
            }
            else
            {
                if (IsAuthorized(httpRequestEvent) == false)
                {
                    SendJsonResponse(httpRequestEvent.Response, new UnauthorizedRequestResponse(), 401);
                    return;
                }

                SendJsonResponse(httpRequestEvent.Response, new RootResponse());
            }
        }

        /// <summary>
        /// Async tasks that run in the background of the Web Server
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task BackgroundTasks(CancellationToken cancellationToken)
        {

            await Task.Run(async () =>
            {
                while (true)
                {
                    SessionManager.Sync();
                    await Task.Delay(5000, cancellationToken);
                    if (cancellationToken.IsCancellationRequested) break;
                    if (server.State == HttpServerState.Stopped) break;
                }
            }, cancellationToken);

        }

        /// <summary>
        /// Gets the parameter either from a GET or POST request
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetParameter(HttpRequest httpRequest, string parameter)
        {
            switch (httpRequest.RequestType.ToUpper())
            {
                case "GET":
                    return httpRequest.QueryString.Get(parameter);

                case "POST":
                    return httpRequest.Form.Get(parameter);

                default:
                    throw new UnsupportedRequestMethodException();
            }
        }
    }
}
