using Intellivoid.HyperWS;
using Netlenium.Driver;
using System;

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
                WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
            }

            switch (requestPath[1])
            {
                case "clear":
                    Clear(httpRequestEventArg);
                    break;

                case "click":
                    Click(httpRequestEventArg);
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
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
                    break;
            }

            return;
        }

        /// <summary>
        /// Simulates key strokes on the Element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void SendKeys(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var Element = Utilities.GetElement(httpRequestEventArgs);
            var keysValue = WebService.GetParameter(httpRequestEventArgs.Request, "input");

            if (Element == null)
            {
                return;
            }

            if (keysValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("input"), 400);
                return;
            }

            try
            {
                Element.SendKeys(keysValue);
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
        /// Simulates a click event on the element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void Click(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var Element = Utilities.GetElement(httpRequestEventArgs);

            if (Element == null)
            {
                return;
            }

            try
            {
                Element.Click();
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
        /// Gets the value of an attribute from the given element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void GetAttribute(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var Element = Utilities.GetElement(httpRequestEventArgs);
            var AttributeName = WebService.GetParameter(httpRequestEventArgs.Request, "attribute_name");

            if (AttributeName == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("attribute_name"), 400);
                return;
            }

            if (Element == null)
            {
                return;
            }

            try
            {
                var AttributeValue = Element.GetAttribute(AttributeName);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.AttributeValueResponse(AttributeValue), 200);
                return;
            }
            catch (AttributeNotFoundException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.AttributeNotFoundResponse(AttributeName), 404);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }

        }

        /// <summary>
        /// Sets a value to a WebElement's attribute, creates attribute if it doesn't exist
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void SetAttribute(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var Element = Utilities.GetElement(httpRequestEventArgs);
            var AttributeName = WebService.GetParameter(httpRequestEventArgs.Request, "attribute_name");
            var AttributeValue = WebService.GetParameter(httpRequestEventArgs.Request, "attribute_value");

            if (AttributeName == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("attribute_name"), 400);
                return;
            }

            if (AttributeValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("attribute_value"), 400);
                return;
            }

            if (Element == null)
            {
                return;
            }

            try
            {
                Element.SetAttribute(AttributeName, AttributeValue);
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
        /// Submits the given element to the Web Server
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void Submit(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var Element = Utilities.GetElement(httpRequestEventArgs);

            if (Element == null)
            {
                return;
            }

            try
            {
                Element.Submit();
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
        /// Clears the innerHTML of the element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void Clear(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var Element = Utilities.GetElement(httpRequestEventArgs);

            if (Element == null)
            {
                return;
            }

            try
            {
                Element.Clear();
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
