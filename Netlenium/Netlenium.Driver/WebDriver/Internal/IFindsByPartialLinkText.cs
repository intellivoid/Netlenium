using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by a partial match on their link text.
    /// </summary>
    public interface IFindsByPartialLinkText
    {
        /// <summary>
        /// Finds the first element matching the specified partial link text.
        /// </summary>
        /// <param name="partialLinkText">The partial link text to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementByPartialLinkText(string partialLinkText);

        /// <summary>
        /// Finds all elements matching the specified partial link text.
        /// </summary>
        /// <param name="partialLinkText">The partial link text to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText);
    }
}
