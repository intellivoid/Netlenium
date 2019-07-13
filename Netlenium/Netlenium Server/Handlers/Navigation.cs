using Intellivoid.HyperWS;
using System;

namespace NetleniumServer.Handlers
{
    /// <summary>
    /// API Handler for Driver Navigation
    /// </summary>
    public static class Navigation
    {
        /// <summary>
        /// Navigates to the given URL
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void LoadURL(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var url = WebService.GetParameter(httpRequestEventArgs.Request, "url");

            if (url == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("url"), 400);
                return;
            }

            try
            {
                SessionManager.activeSessions[sessionId].Driver.Actions.LoadURI(url);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.RequestSuccessResponse(), 200);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }

        /// <summary>
        /// Goes back on item in the history
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void GoBack(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                SessionManager.activeSessions[sessionId].Driver.Actions.GoBack();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.RequestSuccessResponse(), 200);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }

        /// <summary>
        /// Goes forward on item in the history
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void GoForward(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                SessionManager.activeSessions[sessionId].Driver.Actions.GoForward();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.RequestSuccessResponse(), 200);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }

        /// <summary>
        /// Reloads the current document that's loaded
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void Reload(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                SessionManager.activeSessions[sessionId].Driver.Actions.Reload();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.RequestSuccessResponse(), 200);
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
