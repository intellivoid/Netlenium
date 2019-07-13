﻿using Intellivoid.HyperWS;
using Netlenium.Driver;
using System;

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
        public static void CreateSession(HttpRequestEventArgs httpRequestEventArgs)
        {
            var targetBrowser = WebService.GetParameter(httpRequestEventArgs.Request, "target_browser");

            if(targetBrowser == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("target_browser"), 400);
                return;
            }

            if(SessionManager.activeSessions != null)
            {
                if (SessionManager.activeSessions.Count == CommandLineParameters.MaxSessions)
                {
                    WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "SessionManager", $"Cannot create session for '{targetBrowser.ToString()}', {CommandLineParameters.MaxSessions} Session(s) are allowed at the same time");
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MaxSessionsErrorResponse(), 403);
                    return;
                }
            }

            Session SessionObject = null;

            switch(targetBrowser)
            {
                case "chrome":
                    SessionObject = SessionManager.CreateSession(Netlenium.Driver.Browser.Chrome);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnsupportedBrowserResponse(), 400);
                    return;
            }

            WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionCreatedResponse(SessionObject.ID), 200);
            return;
        }

        /// <summary>
        /// Stops an existing session by killing the driver process and cleaning up
        /// used resources
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void StopSession(HttpRequestEventArgs httpRequestEventArgs)
        {
            if(WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            

            try
            {
                SessionManager.StopSession(sessionId);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.RequestSuccessResponse(), 200);
            }
            catch(Exception ex)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionErrorResponse(ex.Message), 500);
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
                foreach(IWebElement webElement in SessionManager.activeSessions[sessionId].Driver.Document.GetElements(Utilities.ParseSearchBy(searchBy), searchValue))
                {
                    WebServiceResponse.Elements.Add(new Objects.WebElement(webElement));
                }
                WebService.SendJsonResponse(httpRequestEventArgs.Response, WebServiceResponse, 200);
                return;
            }
            catch(InvalidSearchByValueException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.InvalidSearchByValueResponse(), 400);
                return;
            }
            catch(Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }

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
            if(WebService.VerifySession(httpRequestEventArgs) == false)
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
            catch(AttributeNotFoundException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.AttributeNotFoundResponse(AttributeName), 404);
                return;
            }
            catch(Exception exception)
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

    }
}
