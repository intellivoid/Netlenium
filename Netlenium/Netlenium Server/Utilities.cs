using Netlenium.Driver;
using System;
using NetleniumServer.Intellivoid;

namespace NetleniumServer
{
    /// <summary>
    /// General Utilities Class
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Parses the SearchBy value, throws an exception if it's invalid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static SearchBy ParseSearchBy(string input)
        {
            switch (input.ToLower())
            {
                case "class_name":
                    return SearchBy.ClassName;

                case "css_selector":
                    return SearchBy.CssSelector;

                case "id":
                    return SearchBy.Id;

                case "name":
                    return SearchBy.Name;

                case "tag_name":
                    return SearchBy.TagName;

                case "xpath":
                    return SearchBy.XPath;

                default:
                    throw new InvalidSearchByValueException();
            }
        }

        /// <summary>
        /// Parses the Http Request to return the Element
        /// </summary>
        /// <param name="httpRequestEventArgs"></param>
        /// <returns></returns>
        public static IWebElement GetElement(HttpRequestEventArgs httpRequestEventArgs)
        {
            var sessionId = WebService.GetParameter(httpRequestEventArgs.Request, "session_id");
            var searchBy = WebService.GetParameter(httpRequestEventArgs.Request, "by");
            var searchValue = WebService.GetParameter(httpRequestEventArgs.Request, "value");
            var indexValue = WebService.GetParameter(httpRequestEventArgs.Request, "index");

            if (searchBy == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("by"), 400);
                return null;
            }

            if (searchValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("value"), 400);
                return null;
            }

            if (indexValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.MissingParameterResponse("index"), 400);
                return null;
            }

            try
            {
                var elements = SessionManager.activeSessions[sessionId].Driver.Document.GetElements(ParseSearchBy(searchBy), searchValue);

                if (elements.Count == 0)
                {
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.ElementNotFoundResponse(), 404);
                    return null;
                }

                if (Convert.ToInt32(indexValue) >= 0 && Convert.ToInt32(indexValue) < elements.Count)
                {
                    return elements[Convert.ToInt32(indexValue)];
                }

                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.ElementNotFoundResponse(), 404);

                return null;
            }
            catch (InvalidSearchByValueException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.InvalidSearchByValueResponse(), 400);
                return null;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new Responses.UnexpectedErrorResponse(exception.Message), 500);
                return null;
            }

        }

      
    }
}
