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

            if (WebService.IsAuthorized(httpRequestEventArg) == false)
            {
                WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.UnauthorizedRequestResponse(), 401);
                return;
            }

            switch (requestPath[1])
            {
                case "get_elements":
                    GetElements(httpRequestEventArg);
                    break;

                case "close":
                    Close(httpRequestEventArg);
                    break;

                case "execute_javascript":
                    ExecuteJavascript(httpRequestEventArg);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
                    break;
            }

            return;
        }

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

        /// <summary>
        /// Executes Javascript Code and returns the results
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void ExecuteJavascript(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var code = WebService.GetParameter(httpRequestEventArgs.Request, "code");
            
            if (code == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("code"), 400);
                return;
            }

            try
            {
                var output = SessionManager.activeSessions[sessionId].Driver.Actions.ExecuteJavascript(code);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.JavascriptExecutionResponse(output), 200);
                return;
            }
            catch (JavascriptExecutionException js_exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.JavascriptExecutionErrorResponse(js_exception.Message), 500);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }

        /// <summary>
        /// Closes the current window handle
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void Close(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
           
            try
            {
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
