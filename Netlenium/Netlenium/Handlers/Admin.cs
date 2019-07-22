using Netlenium.Intellivoid;
using Netlenium.Responses;

namespace Netlenium.Handlers
{
    /// <summary>
    /// API Handler for Admin Actions
    /// </summary>
    public static class Admin
    {
        /// <summary>
        /// Handles the incoming request for this API Handler
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="httpRequestEventArg"></param>
        public static void HandleRequest(string[] requestPath, HttpRequestEventArgs httpRequestEventArg)
        {
            if (requestPath.Length < 1)
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
                case "active_sessions":
                    ActiveSessions(httpRequestEventArg);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new NotFoundResponse(), 404);
                    break;
            }
        }

        /// <summary>
        /// Lists all active sessions with their related information
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void ActiveSessions(HttpRequestEventArgs httpRequestEventArgs)
        {

        }
    }
}
