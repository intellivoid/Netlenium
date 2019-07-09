using Intellivoid.HyperWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer
{
    public static class APIHandler
    {
        /// <summary>
        /// Creates a new Driver Session
        /// </summary>
        /// <param name="httpRequest"></param>
        public static void CreateSession(HttpRequestEventArgs httpRequest)
        {
            var targetBrowser = Utilities.GetParameter(httpRequest, "target_browser");
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
