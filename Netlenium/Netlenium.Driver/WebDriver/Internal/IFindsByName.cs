using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by their name.
    /// </summary>
    public interface IFindsByName
    {
        /// <summary>
        /// Finds the first element matching the specified name.
        /// </summary>
        /// <param name="name">The name to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementByName(string name);

        /// <summary>
        /// Finds all elements matching the specified name.
        /// </summary>
        /// <param name="name">The name to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsByName(string name);
    }
}
