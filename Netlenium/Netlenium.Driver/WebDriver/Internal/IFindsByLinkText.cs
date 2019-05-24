using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by their link text.
    /// </summary>
    public interface IFindsByLinkText
    {
        /// <summary>
        /// Finds the first element matching the specified link text.
        /// </summary>
        /// <param name="linkText">The link text to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementByLinkText(string linkText);

        /// <summary>
        /// Finds all elements matching the specified link text.
        /// </summary>
        /// <param name="linkText">The link text to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText);
    }
}
