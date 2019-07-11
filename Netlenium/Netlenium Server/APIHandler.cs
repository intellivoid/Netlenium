using Intellivoid.HyperWS;
using System;

namespace NetleniumServer
{
    /// <summary>
    /// Handles API Requests
    /// </summary>
    public static class APIHandler
    {
        /// <summary>
        /// Creates a new Driver Session
        /// </summary>
        /// <param name="httpRequest"></param>
        public static void CreateSession(HttpRequestEventArgs httpRequestEventArgs)
        {
            var targetBrowser = WebService.GetParameter(httpRequestEventArgs.Request, "target_browser");

            if(targetBrowser == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("target_browser"), 400);
                return;
            }

            if (CommandLineParameters.MaxSessions > 0)
            {
                if (SessionManager.activeSessions.Count == CommandLineParameters.MaxSessions)
                {
                    WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "SessionManager", $"Cannot create session for '{targetBrowser.ToString()}', {CommandLineParameters.MaxSessions} Session(s) are allowed at the same time");
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MaxSessionsErrorResponse(), 403);
                    return;
                }
            }

            Session SessionObject = null;

            switch(targetBrowser)
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
            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            if (sessionId == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("session_id"), 400);
                return;
            }

            if(SessionManager.SessionExists(sessionId) == false)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionNotFoundResponse(sessionId), 404);
                return;
            }

            try
            {
                SessionManager.StopSession(sessionId);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionStoppedResponse(), 200);
            }
            catch(Exception ex)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionErrorResponse(ex.Message), 500);
            }

            return;
        }
    }
}
