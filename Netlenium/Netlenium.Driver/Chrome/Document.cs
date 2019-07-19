using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Netlenium.Driver.Chrome
{
    /// <inheritdoc />
    /// <summary>
    /// Chrome Driver Document
    /// </summary>
    public class Document : IDocument
    {
        private readonly Driver driver;

        public Document(Driver driver)
        {
            this.driver = driver;
        }

        /// <inheritdoc />
        public string Title => driver.RemoteDriver.Title;

        /// <inheritdoc />
        public string Uri => driver.RemoteDriver.Url;

        /// <inheritdoc />
        public IWebElement GetElement(SearchBy searchBy, string value)
        {
            var results = GetElements(searchBy, value);

            if (results.Count != 0) return results[0];
            driver.Logging.WriteEntry(Logging.MessageType.Error, "Document", "No elements were found");
            throw new NoElementsFoundException();

        }

        /// <inheritdoc />
        public List<IWebElement> GetElements(SearchBy searchBy, string value)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Document", $"Searching for '{value}' by '{searchBy}'");

            ReadOnlyCollection<WebDriver.IWebElement> results;

            switch (searchBy)
            {

                case SearchBy.ClassName:
                    results = driver.RemoteDriver.FindElementsByClassName(value);
                    break;

                case SearchBy.CssSelector:
                    results = driver.RemoteDriver.FindElementsByCssSelector(value);
                    break;

                case SearchBy.Id:
                    results = driver.RemoteDriver.FindElementsById(value);
                    break;

                case SearchBy.Name:
                    results = driver.RemoteDriver.FindElementsByName(value);
                    break;

                case SearchBy.TagName:
                    results = driver.RemoteDriver.FindElementsByTagName(value);
                    break;

                case SearchBy.XPath:
                    results = driver.RemoteDriver.FindElementsByXPath(value);
                    break;

                default:
                    driver.Logging.WriteEntry(Logging.MessageType.Error, "Document", "The given search method is not supported for this driver");
                    throw new UnsupportedSearchMethodException();
            }

            var returnResults = new List<IWebElement>();
            foreach (var webElement in results)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Document", webElement.ToString());
                returnResults.Add(new WebElement(driver, webElement));
            }

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Document", $"{results.Count} results were found");
            return returnResults;
        }
    }
}
