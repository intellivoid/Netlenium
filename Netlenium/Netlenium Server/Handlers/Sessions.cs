using Intellivoid.HyperWS;
using System;

namespace NetleniumServer.Handlers
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
                WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
            }

            if (WebService.IsAuthorized(httpRequestEventArg) == false)
            {
                WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.UnauthorizedRequestResponse(), 401);
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
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
                    break;
            }

            return;
        }

        /// <summary>
        /// Creates a new Driver Session
        /// </summary>
        /// <param name="httpRequest"></param>
        public static void CreateSession(HttpRequestEventArgs httpRequestEventArgs)
        {
            var targetBrowser = WebService.GetParameter(httpRequestEventArgs.Request, "target_browser");

            if (targetBrowser == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("target_browser"), 400);
                return;
            }

            if (SessionManager.activeSessions != null)
            {
                if (SessionManager.activeSessions.Count == CommandLineParameters.MaxSessions)
                {
                    WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "SessionManager", $"Cannot create session for '{targetBrowser.ToString()}', {CommandLineParameters.MaxSessions} Session(s) are allowed at the same time");
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MaxSessionsErrorResponse(), 403);
                    return;
                }
            }

            Session SessionObject = null;

            switch (targetBrowser)
            {
                case "chrome":
                    SessionObject = SessionManager.CreateSession(Netlenium.Driver.Browser.Chrome);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnsupportedBrowserResponse(), 400);
                    return;
            }

            WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionCreatedResponse(SessionObject.ID), 200);
            return;
        }

        /// <summary>
        /// Closes a existing session by killing the driver process and cleaning up
        /// used resources
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void CloseSession(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");


            try
            {
                SessionManager.StopSession(sessionId);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.RequestSuccessResponse(), 200);
            }
            catch (Exception ex)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionErrorResponse(ex.Message), 500);
            }

            return;
        }
    }
}
