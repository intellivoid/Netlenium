using System;
using Netlenium.Driver;
using NetleniumServer.Intellivoid;
using NetleniumServer.Responses;

namespace NetleniumServer.Handlers
{
    /// <summary>
    /// API Handler for WebElement
    /// </summary>
    public static class WebElement
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
                case "clear":
                    Clear(httpRequestEventArg);
                    break;

                case "click":
                    Click(httpRequestEventArg);
                    break;

                case "move_to":
                    MoveTo(httpRequestEventArg);
                    break;

                case "get_attribute":
                    GetAttribute(httpRequestEventArg);
                    break;

                case "set_attribute":
                    SetAttribute(httpRequestEventArg);
                    break;

                case "send_keys":
                    SendKeys(httpRequestEventArg);
                    break;

                case "submit":
                    Submit(httpRequestEventArg);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new NotFoundResponse(), 404);
                    break;
            }
        }

        /// <summary>
        /// Simulates key strokes on the Element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void SendKeys(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var element = Utilities.GetElement(httpRequestEventArgs);
            var keysValue = WebService.GetParameter(httpRequestEventArgs.Request, "input");

            if (element == null)
            {
                return;
            }

            if (keysValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("input"), 400);
                return;
            }

            try
            {
                element.SendKeys(keysValue);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Simulates a click event on the element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void Click(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var element = Utilities.GetElement(httpRequestEventArgs);

            if (element == null)
            {
                return;
            }

            try
            {
                element.Click();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Moves to the selected Element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void MoveTo(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var element = Utilities.GetElement(httpRequestEventArgs);

            if (element == null)
            {
                return;
            }

            try
            {
                element.MoveTo();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Gets the value of an attribute from the given element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void GetAttribute(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var element = Utilities.GetElement(httpRequestEventArgs);
            var attributeName = WebService.GetParameter(httpRequestEventArgs.Request, "attribute_name");

            if (attributeName == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("attribute_name"), 400);
                return;
            }

            if (element == null)
            {
                return;
            }

            try
            {
                var attributeValue = element.GetAttribute(attributeName);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new AttributeValueResponse(attributeValue));
            }
            catch (AttributeNotFoundException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new AttributeNotFoundResponse(attributeName), 404);
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }

        }

        /// <summary>
        /// Sets a value to a WebElement's attribute, creates attribute if it doesn't exist
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void SetAttribute(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var element = Utilities.GetElement(httpRequestEventArgs);
            var attributeName = WebService.GetParameter(httpRequestEventArgs.Request, "attribute_name");
            var attributeValue = WebService.GetParameter(httpRequestEventArgs.Request, "attribute_value");

            if (attributeName == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("attribute_name"), 400);
                return;
            }

            if (attributeValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("attribute_value"), 400);
                return;
            }

            if (element == null)
            {
                return;
            }

            try
            {
                element.SetAttribute(attributeName, attributeValue);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Submits the given element to the Web Server
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void Submit(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var element = Utilities.GetElement(httpRequestEventArgs);

            if (element == null)
            {
                return;
            }

            try
            {
                element.Submit();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Clears the innerHTML of the element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void Clear(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var element = Utilities.GetElement(httpRequestEventArgs);

            if (element == null)
            {
                return;
            }

            try
            {
                element.Clear();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }
    }
}
