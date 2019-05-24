namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Interface allowing the user to determine if the driver instance supports web storage.
    /// </summary>
    public interface IHasWebStorage
    {
        /// <summary>
        /// Gets a value indicating whether web storage is supported for this driver.
        /// </summary>
        bool HasWebStorage { get; }

        /// <summary>
        /// Gets an <see cref="IWebStorage"/> object for managing web storage.
        /// </summary>
        IWebStorage WebStorage { get; }
    }
}
