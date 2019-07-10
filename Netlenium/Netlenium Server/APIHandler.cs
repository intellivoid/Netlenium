using Intellivoid.HyperWS;

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
        public static void CreateSession(HttpRequestEventArgs httpRequest)
        {
            var targetBrowser = WebService.GetParameter(httpRequest.Request, "target_browser");

            if(targetBrowser == null)
            {
                WebService.SendJsonResponse(httpRequest.Response, new Responses.MissingParameterResponse("target_browser"), 400);
                return;
            }

            Session SessionObject = null;

            switch(targetBrowser)
            {
                case "chrome":
                    SessionObject = SessionManager.CreateSession(Netlenium.Driver.Browser.Chrome);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequest.Response, new Responses.UnsupportedBrowserResponse(), 400);
                    return;
            }

            WebService.SendJsonResponse(httpRequest.Response, new Responses.SessionCreatedResponse(SessionObject.ID), 200);
            return;
        }


    }
}
