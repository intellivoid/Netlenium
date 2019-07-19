using System;
using System.Collections.Generic;
using Netlenium.Driver;
using NetleniumServer.Intellivoid;
using NetleniumServer.Objects;
using NetleniumServer.Responses;

namespace NetleniumServer.Handlers
{
    /// <summary>
    /// API Handler for Window Handlers
    /// </summary>
    public static class WindowHandler
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
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new NotFoundResponse(), 404);
                    break;
            }
        }

        /// <summary>
        /// Gets the details of the current Window Handler
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void CurrentWindow(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                var currentWindowObject = new CurrentWindow
                {
                    Id = SessionManager.activeSessions[sessionId].Driver.Actions.CurrentWindow.ID,
                    Title = SessionManager.activeSessions[sessionId].Driver.Document.Title,
                    Url = SessionManager.activeSessions[sessionId].Driver.Document.Uri
                };
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new CurrentWindowResponse(currentWindowObject));
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Returns a list of opened Window Handles
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void ListWindowHandles(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                var windowHandlesList = new List<string>();
                foreach(var windowHandler in SessionManager.activeSessions[sessionId].Driver.Actions.GetWindows())
                {
                    windowHandlesList.Add(windowHandler.ID);
                }
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new ListWindowHandlesResponse(windowHandlesList));
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Switches to another Window Handle
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void SwitchTo(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var windowId = WebService.GetParameter(httpRequestEventArgs.Request, "id");

            if (windowId == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("id"), 400);
                return;
            }

            try
            {
                var windowHandle = SessionManager.activeSessions[sessionId].Driver.Actions.GetWindow(windowId);
                windowHandle.SwitchTo();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch(NoSuchWindowException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new WindowNotFoundResponse(), 404);
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }
    }
}
