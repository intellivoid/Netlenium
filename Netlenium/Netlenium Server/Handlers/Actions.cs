using System;
using Netlenium.Driver;
using NetleniumServer.Intellivoid;
using NetleniumServer.Responses;

namespace NetleniumServer.Handlers
{
    /// <summary>
    /// API Handler for Driver Actions
    /// </summary>
    public static class Actions
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
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new NotFoundResponse(), 404);
                    break;
            }
        }

        /// <summary>
        /// Returns a list of Elements that are in the DOM
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void GetElements(HttpRequestEventArgs httpRequestEventArgs)
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
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("by"), 400);
                return;
            }

            if (searchValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("value"), 400);
                return;
            }

            try
            {
                var webServiceResponse = new ElementsResultsResponse();
                foreach (var webElement in SessionManager.activeSessions[sessionId].Driver.Document.GetElements(Utilities.ParseSearchBy(searchBy), searchValue))
                {
                    webServiceResponse.Elements.Add(new Objects.WebElement(webElement));
                }
                WebService.SendJsonResponse(httpRequestEventArgs.Response, webServiceResponse);
            }
            catch (InvalidSearchByValueException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new InvalidSearchByValueResponse(), 400);
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Executes Javascript Code and returns the results
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void ExecuteJavascript(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var code = WebService.GetParameter(httpRequestEventArgs.Request, "code");
            
            if (code == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("code"), 400);
                return;
            }

            try
            {
                var output = SessionManager.activeSessions[sessionId].Driver.Actions.ExecuteJavascript(code);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new JavascriptExecutionResponse(output));
            }
            catch (JavascriptExecutionException jsException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new JavascriptExecutionErrorResponse(jsException.Message), 500);
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Closes the current window handle
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void Close(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
           
            try
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }
    }
}
