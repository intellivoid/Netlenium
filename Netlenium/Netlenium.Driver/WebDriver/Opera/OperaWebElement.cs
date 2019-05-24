using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Opera
{
    /// <summary>
    /// Provides a mechanism to get elements off the page for test
    /// </summary>
    public class OperaWebElement : RemoteWebElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperaWebElement"/> class
        /// </summary>
        /// <param name="parent">Driver in use</param>
        /// <param name="elementId">Id of the element</param>
        public OperaWebElement(OperaDriver parent, string elementId)
            : base(parent, elementId)
        {
        }
    }
}
