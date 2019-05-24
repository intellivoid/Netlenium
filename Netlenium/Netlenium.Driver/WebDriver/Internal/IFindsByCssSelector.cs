using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by their cascading style sheet (CSS) selector.
    /// </summary>
    public interface IFindsByCssSelector
    {
        /// <summary>
        /// Finds the first element matching the specified CSS selector.
        /// </summary>
        /// <param name="cssSelector">The id to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementByCssSelector(string cssSelector);

        /// <summary>
        /// Finds all elements matching the specified CSS selector.
        /// </summary>
        /// <param name="cssSelector">The CSS selector to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector);
    }
}
