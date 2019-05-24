namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user can access the driver used to find an element.
    /// </summary>
    public interface IWrapsDriver
    {
        /// <summary>
        /// Gets the <see cref="IWebDriver"/> used to find this element.
        /// </summary>
        IWebDriver WrappedDriver { get; }
    }
}
