using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by XPath.
    /// </summary>
    public interface IFindsByXPath
    {
        /// <summary>
        /// Finds the first element matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">The XPath query to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementByXPath(string xpath);

        /// <summary>
        /// Finds all elements matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">The XPath query to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath);
    }
}
