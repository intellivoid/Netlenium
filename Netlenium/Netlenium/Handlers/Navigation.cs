﻿using System;
using Netlenium.Intellivoid;
using Netlenium.Responses;

namespace Netlenium.Handlers
{
    /// <summary>
    /// API Handler for Driver Navigation
    /// </summary>
    public static class Navigation
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
                case "load_url":
                    LoadUrl(httpRequestEventArg);
                    break;

                case "go_forward":
                    GoForward(httpRequestEventArg);
                    break;

                case "go_back":
                    GoBack(httpRequestEventArg);
                    break;

                case "reload":
                    Reload(httpRequestEventArg);
                    break;

                default:
                    WebService.SendJsonResponse(httpRequestEventArg.Response, new NotFoundResponse(), 404);
                    break;
            }
        }

        /// <summary>
        /// Navigates to the given URL
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void LoadUrl(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var url = WebService.GetParameter(httpRequestEventArgs.Request, "url");

            if (url == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("url"), 400);
                return;
            }

            try
            {
                SessionManager.ActiveSessions[sessionId].Driver.Actions.LoadURI(url);
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Goes back on item in the history
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void GoBack(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                SessionManager.ActiveSessions[sessionId].Driver.Actions.GoBack();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Goes forward on item in the history
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void GoForward(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                SessionManager.ActiveSessions[sessionId].Driver.Actions.GoForward();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }

        /// <summary>
        /// Reloads the current document that's loaded
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        private static void Reload(HttpRequestEventArgs httpRequestEventArgs)
        {
            if (WebService.VerifySession(httpRequestEventArgs) == false)
            {
                return;
            }

            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");

            try
            {
                SessionManager.ActiveSessions[sessionId].Driver.Actions.Reload();
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new RequestSuccessResponse());
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
            }
        }
    }
}
