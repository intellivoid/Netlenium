using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by their ID.
    /// </summary>
    public interface IFindsById
    {
        /// <summary>
        /// Finds the first element matching the specified id.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementById(string id);

        /// <summary>
        /// Finds all elements matching the specified id.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsById(string id);
    }
}
