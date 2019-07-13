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
        /// Stops an existing session by killing the driver process and cleaning up
        /// used resources
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void StopSession(HttpRequestEventArgs httpRequestEventArgs)
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
