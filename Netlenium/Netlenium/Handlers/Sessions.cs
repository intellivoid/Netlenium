using System;
using Netlenium.Driver;
using Netlenium.Intellivoid;
using Netlenium.Logging;
using Netlenium.Responses;

namespace Netlenium.Handlers
{
    /// <summary>
    /// API Handler for Sessions
    /// </summary>
    public static class Sessions
    {
        /// <summary>
        /// Handles the incoming request for this API Handler
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="httpRequestEventArg"></param>
        public static void HandleRequest(string[] requestPath, HttpRequestEventArgs httpRequestEventArg)
        {
            if(requestPath.Length < 1)
            {
                WebService.SendJsonResponse(httpRequestEventArg.Response, new NotFoundResponse(), 404);
            }

            if (WebService.IsAuthorized(httpRequestEventArg) == false)
            {
                WebService.SendJsonResponse(httpRequestEventArg.Response, new UnauthorizedRequestResponse(), 401);
                return;
            }

            switch (requestPath[1])
            {
                case "create":
                    CreateSession(httpRequestEventArg);
                    break;

                case "close":
                    CloseSession(httpRequestEventArg);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new NotFoundResponse(), 404);
                    break;
            }
        }

        /// <summary>
        /// Creates a new Driver Session
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void CreateSession(HttpRequestEventArgs httpRequestEventArgs)
        {
            var targetDriver = WebService.GetParameter(httpRequestEventArgs.Request, "target_driver");
            var proxy_scheme = WebService.GetParameter(httpRequestEventArgs.Request, "proxy_scheme");
            var proxy_host = WebService.GetParameter(httpRequestEventArgs.Request, "proxy_host");
            var proxy_port = WebService.GetParameter(httpRequestEventArgs.Request, "proxy_port");
            var proxy_username = WebService.GetParameter(httpRequestEventArgs.Request, "proxy_username");
            var proxy_password = WebService.GetParameter(httpRequestEventArgs.Request, "proxy_password");
            

            if (targetDriver == null)
            {
                if(CommandLineParameters.DefaultDriver == string.Empty)
                {
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("target_driver"), 400);
                    return;
                }

                targetDriver = CommandLineParameters.DefaultDriver.ToLower();
            }

            if (SessionManager.ActiveSessions != null)
            {
                if (SessionManager.ActiveSessions.Count == CommandLineParameters.MaxSessions)
                {
                    WebService.Logging.WriteEntry(MessageType.Error, "SessionManager", $"Cannot create session for '{targetDriver}', {CommandLineParameters.MaxSessions} Session(s) are allowed at the same time");
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new MaxSessionsErrorResponse(), 403);
                    return;
                }
            }

            Session sessionObject;

            switch (targetDriver)
            {
                case "chrome":

                    if (CommandLineParameters.DisableChromeDriver)
                    {
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new DriverDisabledResponse(targetDriver), 403);
                        return;
                    }

                    sessionObject = SessionManager.CreateSession(Browser.Chrome);
                    break;

                case "firefox":

                    if (CommandLineParameters.DisableFirefoxDriver)
                    {
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new DriverDisabledResponse(targetDriver), 403);
                        return;
                    }

                    sessionObject = SessionManager.CreateSession(Browser.Firefox);
                    break;
                
                case "opera":

                    if (CommandLineParameters.DisableOperaDriver)
                    {
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new DriverDisabledResponse(targetDriver), 403);
                        return;
                    }

                    sessionObject = SessionManager.CreateSession(Browser.Opera);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnsupportedDriverResponse(), 400);
                    return;
            }

           
            if(proxy_scheme != null)
            {
                if(proxy_host == null)
                {
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("proxy_host"), 400);
                    return;
                }

                if(proxy_port == null)
                {
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("proxy_port"), 400);
                    return;
                }

                switch(proxy_scheme.ToLower())
                {
                    case "http":
                        sessionObject.Driver.ProxyConfiguration.Scheme = ProxyScheme.HTTP;
                        break;

                    case "https":
                        sessionObject.Driver.ProxyConfiguration.Scheme = ProxyScheme.HTTPS;
                        break;

                    default:
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new InvalidProxySchemeResponse(), 400);
                        return;
                }

                sessionObject.Driver.ProxyConfiguration.Enabled = true;
                sessionObject.Driver.ProxyConfiguration.AuthenticationRequired = false;
                sessionObject.Driver.ProxyConfiguration.Host = proxy_host;
                sessionObject.Driver.ProxyConfiguration.Port = Convert.ToInt32(proxy_port);


                if(proxy_username != null)
                {
                    if(proxy_password == null)
                    {
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("proxy_password"), 400);
                        return;
                    }

                    sessionObject.Driver.ProxyConfiguration.AuthenticationRequired = true;
                    sessionObject.Driver.ProxyConfiguration.Username = proxy_username;
                    sessionObject.Driver.ProxyConfiguration.Password = proxy_password;

                }
            }

            WebService.SendJsonResponse(httpRequestEventArgs.Response, new SessionCreatedResponse(sessionObject.Id));
        }

        /// <summary>
        /// Closes a existing session by killing the driver process and cleaning up
        /// used resources
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void CloseSession(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");


            try
            {
                SessionManager.StopSession(sessionId);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception ex)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new SessionErrorResponse(ex.Message), 500);
            }
        }
    }
}
