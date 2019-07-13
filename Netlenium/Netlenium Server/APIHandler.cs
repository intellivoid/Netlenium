using Intellivoid.HyperWS;
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

            if (CommandLineParameters.MaxSessions > 0)
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
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.SessionStoppedResponse(), 200);
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

    }
}
