using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Edge
{
    /// <summary>
    /// Provides a mechanism to get elements off the page for test
    /// </summary>
    public class EdgeWebElement : RemoteWebElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeWebElement"/> class
        /// </summary>
        /// <param name="parent">Driver in use</param>
        /// <param name="elementId">Id of the element</param>
        public EdgeWebElement(EdgeDriver parent, string elementId)
            : base(parent, elementId)
        {
        }
    }
}
