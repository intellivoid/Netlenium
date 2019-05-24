namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Defines the interface through which the user can determine the capabilities of a driver.
    /// </summary>
    public interface IHasCapabilities
    {
        /// <summary>
        /// Gets the <see cref="ICapabilities"/> object describing the driver's capabilities.
        /// </summary>
        ICapabilities Capabilities { get; }
    }
}
