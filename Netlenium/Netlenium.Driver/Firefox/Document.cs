using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Netlenium.Driver.Firefox
{
    public class Document : IDocument
    {
        private Driver driver;

        public Document(Driver driver)
        {
            this.driver = driver;
        }

        public string Title => driver.RemoteDriver.Title;

        public string Uri => driver.RemoteDriver.Url;

        public IWebElement GetElement(SearchBy searchBy, string value)
        {
            var Results = GetElements(searchBy, value);

            if (Results.Count == 0)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Document", "No elements were found");
                throw new NoElementsFoundException();
            }

            return Results[0];
        }

        public List<IWebElement> GetElements(SearchBy searchBy, string value)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Document", $"Searching for '{value}' by '{searchBy}'");

            ReadOnlyCollection<WebDriver.IWebElement> Results;

            switch (searchBy)
            {

                case SearchBy.ClassName:
                    Results = driver.RemoteDriver.FindElementsByClassName(value);
                    break;

                case SearchBy.CssSelector:
                    Results = driver.RemoteDriver.FindElementsByCssSelector(value);
                    break;

                case SearchBy.Id:
                    Results = driver.RemoteDriver.FindElementsById(value);
                    break;

                case SearchBy.Name:
                    Results = driver.RemoteDriver.FindElementsByName(value);
                    break;

                case SearchBy.TagName:
                    Results = driver.RemoteDriver.FindElementsByTagName(value);
                    break;

                case SearchBy.XPath:
                    Results = driver.RemoteDriver.FindElementsByXPath(value);
                    break;

                default:
                    driver.Logging.WriteEntry(Logging.MessageType.Error, "Document", "The given search method is not supported for this driver");
                    throw new UnsupportedSearchMethodException();
            }

            var ReturnResults = new List<IWebElement>();
            foreach (WebDriver.IWebElement webElement in Results)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Document", webElement.ToString());
                ReturnResults.Add(new WebElement(driver, webElement));
            }

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Document", $"{Results.Count} results were found");
            return ReturnResults;
        }
    }
}
