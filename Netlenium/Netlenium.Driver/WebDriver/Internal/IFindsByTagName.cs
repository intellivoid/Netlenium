using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by their tag name.
    /// </summary>
    public interface IFindsByTagName
    {
        /// <summary>
        /// Finds the first element matching the specified tag name.
        /// </summary>
        /// <param name="tagName">The tag name to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementByTagName(string tagName);

        /// <summary>
        /// Finds all elements matching the specified tag name.
        /// </summary>
        /// <param name="tagName">The tag name to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName);
    }
}
