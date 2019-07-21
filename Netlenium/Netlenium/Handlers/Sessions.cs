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
            var targetBrowser = WebService.GetParameter(httpRequestEventArgs.Request, "target_browser");

            if (targetBrowser == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("target_browser"), 400);
                return;
            }

            if (SessionManager.ActiveSessions != null)
            {
                if (SessionManager.ActiveSessions.Count == CommandLineParameters.MaxSessions)
                {
                    WebService.Logging.WriteEntry(MessageType.Error, "SessionManager", $"Cannot create session for '{targetBrowser}', {CommandLineParameters.MaxSessions} Session(s) are allowed at the same time");
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new MaxSessionsErrorResponse(), 403);
                    return;
                }
            }

            Session sessionObject;

            switch (targetBrowser)
            {
                case "chrome":

                    if (CommandLineParameters.DisableChromeDriver)
                    {
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new DriverDisabledResponse(targetBrowser), 403);
                        return;
                    }

                    sessionObject = SessionManager.CreateSession(Browser.Chrome);
                    break;

                case "firefox":

                    if (CommandLineParameters.DisableFirefoxDriver)
                    {
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new DriverDisabledResponse(targetBrowser), 403);
                        return;
                    }

                    sessionObject = SessionManager.CreateSession(Browser.Firefox);
                    break;
                
                case "opera":

                    if (CommandLineParameters.DisableOperaDriver)
                    {
                        WebService.SendJsonResponse(httpRequestEventArgs.Response, new DriverDisabledResponse(targetBrowser), 403);
                        return;
                    }

                    sessionObject = SessionManager.CreateSession(Browser.Opera);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnsupportedBrowserResponse(), 400);
                    return;
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
