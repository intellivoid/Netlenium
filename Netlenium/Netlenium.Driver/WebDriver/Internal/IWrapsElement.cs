namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user can discover if there is an underlying element to be used.
    /// </summary>
    public interface IWrapsElement
    {
        /// <summary>
        /// Gets the <see cref="IWebElement"/> wrapped by this object.
        /// </summary>
        IWebElement WrappedElement { get; }
    }
}
