using Intellivoid.HyperWS;
using System;

namespace NetleniumServer.Handlers
{
    /// <summary>
    /// API Handler for Window Handlers
    /// </summary>
    public static class WindowHandler
    {
        // <summary>
        /// Handles the incoming request for this API Handler
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="httpRequestEventArg"></param>
        public static void HandleRequest(string[] requestPath, HttpRequestEventArgs httpRequestEventArg)
        {
            if (requestPath.Length < 1)
            {
                WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
            }

            switch (requestPath[1])
            {
                case "current_window":
                    CurrentWindow(httpRequestEventArg);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
                    break;
            }

            return;
        }

        /// <summary>
        /// Gets the details of the current Window Handler
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void CurrentWindow(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                var CurrentWindowObject = new Objects.CurrentWindow()
                {
                    ID = SessionManager.activeSessions[sessionId].Driver.Actions.CurrentWindow.ID,
                    Title = SessionManager.activeSessions[sessionId].Driver.Document.Title,
                    URL = SessionManager.activeSessions[sessionId].Driver.Document.Uri
                };
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.CurrentWindowResponse(CurrentWindowObject), 200);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }
    }
}
