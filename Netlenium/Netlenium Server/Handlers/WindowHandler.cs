using Netlenium.Driver;
using System;
using System.Collections.Generic;
using NetleniumServer.Intellivoid;

namespace NetleniumServer.Handlers
{
    /// <summary>
    /// API Handler for Window Handlers
    /// </summary>
    public static class WindowHandler
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
                case "current_window":
                    CurrentWindow(httpRequestEventArg);
                    break;

                case "list_windows":
                    ListWindowHandles(httpRequestEventArg);
                    break;

                case "switch_to":
                    SwitchTo(httpRequestEventArg);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new Responses.NotFoundResponse(), 404);
                    break;
            }

            return;
        }

        /// <summary>
        /// Gets the details of the current Window Handler
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void CurrentWindow(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                var CurrentWindowObject = new Objects.CurrentWindow()
                {
                    ID = SessionManager.activeSessions[sessionId].Driver.Actions.CurrentWindow.ID,
                    Title = SessionManager.activeSessions[sessionId].Driver.Document.Title,
                    URL = SessionManager.activeSessions[sessionId].Driver.Document.Uri
                };
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.CurrentWindowResponse(CurrentWindowObject), 200);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }

        /// <summary>
        /// Returns a list of opened Window Handles
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void ListWindowHandles(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                var WindowHandlesList = new List<string>();
                foreach(IWindow WindowHandler in SessionManager.activeSessions[sessionId].Driver.Actions.GetWindows())
                {
                    WindowHandlesList.Add(WindowHandler.ID);
                }
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.ListWindowHandlesResponse(WindowHandlesList), 200);
                return;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return;
            }
        }

        /// <summary>
        /// Switches to another Window Handle
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        public static void SwitchTo(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var windowId = WebService.GetParameter(httpRequestEventArgs.Request, "id");

            if (windowId == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("id"), 400);
                return;
            }

            try
            {
                var WindowHandle = SessionManager.activeSessions[sessionId].Driver.Actions.GetWindow(windowId);
                WindowHandle.SwitchTo();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.RequestSuccessResponse(), 200);
                return;
            }
            catch(NoSuchWindowException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.WindowNotFoundResponse(), 404);
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
