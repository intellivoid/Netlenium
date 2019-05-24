using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user finds elements by their CSS class.
    /// </summary>
    public interface IFindsByClassName
    {
        /// <summary>
        /// Finds the first element matching the specified CSS class.
        /// </summary>
        /// <param name="className">The CSS class to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        IWebElement FindElementByClassName(string className);

        /// <summary>
        /// Finds all elements matching the specified CSS class.
        /// </summary>
        /// <param name="className">The CSS class to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        ReadOnlyCollection<IWebElement> FindElementsByClassName(string className);
    }
}
