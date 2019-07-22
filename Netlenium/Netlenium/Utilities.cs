using System;
using System.Collections.Generic;
using Netlenium.Driver;
using Netlenium.Intellivoid;
using Netlenium.Responses;

namespace Netlenium
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
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("by"), 400);
                return null;
            }

            if (searchValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("value"), 400);
                return null;
            }

            if (indexValue == null)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new MissingParameterResponse("index"), 400);
                return null;
            }

            try
            {
                var elements = SessionManager.ActiveSessions[sessionId].Driver.Document.GetElements(ParseSearchBy(searchBy), searchValue);

                if (elements.Count == 0)
                {
                    WebService.SendJsonResponse(httpRequestEventArgs.Response, new ElementNotFoundResponse(), 404);
                    return null;
                }

                if (Convert.ToInt32(indexValue) >= 0 && Convert.ToInt32(indexValue) < elements.Count)
                {
                    return elements[Convert.ToInt32(indexValue)];
                }

                WebService.SendJsonResponse(httpRequestEventArgs.Response, new ElementNotFoundResponse(), 404);

                return null;
            }
            catch (InvalidSearchByValueException)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new InvalidSearchByValueResponse(), 400);
                return null;
            }
            catch (Exception exception)
            {
                WebService.SendJsonResponse(httpRequestEventArgs.Response, new UnexpectedErrorResponse(exception.Message), 500);
                return null;
            }

        }

        /// <summary>
        /// Applies the given options to the driver
        /// </summary>
        /// <param name="driver"></param>
        public static void ApplyOptionsToDriver(IDriver driver)
        {
            if (CommandLineParameters.DisabledStdout)
            {
                driver.Logging.CommandLineLoggingEnabled = false;
            }
            else
            {
                driver.Logging.CommandLineLoggingEnabled = true;
            }

            if (CommandLineParameters.DisableFileLogging)
            {
                driver.Logging.FileLoggingEnabled = false;
            }
            else
            {
                driver.Logging.FileLoggingEnabled = true;
            }

            if(CommandLineParameters.DisableHeadlessMode)
            {
                driver.Headless = false;
            }
            else
            {
                driver.Headless = true;
            }

            switch (CommandLineParameters.DriverLoggingLevel)
            {
                case 0:
                    driver.Logging.InformationEntriesEnabled = false;
                    driver.Logging.WarningEntriesEnabled = false;
                    driver.Logging.ErrorEntriesEnabled = false;
                    driver.Logging.VerboseEntriesEnabled = false;
                    driver.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 1:
                    driver.Logging.InformationEntriesEnabled = true;
                    driver.Logging.WarningEntriesEnabled = true;
                    driver.Logging.ErrorEntriesEnabled = true;
                    driver.Logging.VerboseEntriesEnabled = false;
                    driver.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 2:
                    driver.Logging.InformationEntriesEnabled = true;
                    driver.Logging.WarningEntriesEnabled = true;
                    driver.Logging.ErrorEntriesEnabled = true;
                    driver.Logging.VerboseEntriesEnabled = true;
                    driver.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 3:
                    driver.Logging.InformationEntriesEnabled = true;
                    driver.Logging.WarningEntriesEnabled = true;
                    driver.Logging.ErrorEntriesEnabled = true;
                    driver.Logging.VerboseEntriesEnabled = true;
                    driver.Logging.DebuggingEntriesEnabled = true;
                    break;
            }

        }

        /// <summary>
        /// Prompts the user with a list and asks for an input
        /// </summary>
        /// <param name="message"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string CommandLineAskOptions(string message, List<string> options)
        {
            Console.WriteLine(message);
            foreach(var option in options)
            {
                Console.WriteLine($" {options.IndexOf(option)}) {option}");
            }
            Console.WriteLine();

            while(true)
            {
                Console.Write("Choice: ");
                var Input = Console.ReadLine();

                try
                {
                    var IntInput = Convert.ToInt32(Input);
                    

                    if (options.Count > IntInput && options[IntInput] != null)
                    {
                        return options[IntInput];
                    }

                    Console.WriteLine("Invalid Option!");

                }
                catch(Exception)
                {
                    Console.WriteLine("Invalid Option!");
                    continue;
                }
            }
        }

        /// <summary>
        /// Prompts the user with a yes or no input
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool CommandLineAskYesNo(string message)
        {
            while(true)
            {
                Console.Write($"{message} (Y/N): ");
                var Input = Console.ReadLine().ToLower();

                switch(Input)
                {
                    case "y": return true;
                    case "yes": return true;
                    case "n": return false;
                    case "no": return false;
                    default:
                        Console.WriteLine("Invalid Option!");
                        break;
                }
            }
        }

    }
}
