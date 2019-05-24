using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Chrome
{
    /// <summary>
    /// Provides a mechanism to get elements off the page for test
    /// </summary>
    public class ChromeWebElement : RemoteWebElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeWebElement"/> class.
        /// </summary>
        /// <param name="parent">Driver in use</param>
        /// <param name="elementId">Id of the element</param>
        public ChromeWebElement(ChromeDriver parent, string elementId)
            : base(parent, elementId)
        {
        }
    }
}
