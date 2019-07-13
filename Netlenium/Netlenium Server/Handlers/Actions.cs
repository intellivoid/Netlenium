using Intellivoid.HyperWS;
using Netlenium.Driver;
using System;

namespace NetleniumServer.Handlers
{
    /// <summary>
    /// API Handler for Driver Actions
    /// </summary>
    public static class Actions
    {
        /// <summary>
        /// Returns a list of Elements that are in the DOM
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void GetElements(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var searchBy = WebService.GetParameter(httpRequestEventArgs.Request, "by");
            var searchValue = WebService.GetParameter(httpRequestEventArgs.Request, "value");

            if (searchBy == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("by"), 400);
                return;
            }

            if (searchValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("value"), 400);
                return;
            }

            try
            {
                var WebServiceResponse = new Responses.ElementsResultsResponse();
                foreach (IWebElement webElement in SessionManager.activeSessions[sessionId].Driver.Document.GetElements(Utilities.ParseSearchBy(searchBy), searchValue))
                {
                    WebServiceResponse.Elements.Add(new Objects.WebElement(webElement));
                }
                WebService.SendJsonResponse(httpRequestEventArgs.Response, WebServiceResponse, 200);
                return;
            }
            catch (InvalidSearchByValueException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.InvalidSearchByValueResponse(), 400);
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
