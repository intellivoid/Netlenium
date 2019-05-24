namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Interface allowing the user to determine if the driver instance supports geolocation.
    /// </summary>
    public interface IHasLocationContext
    {
        /// <summary>
        /// Gets a value indicating whether manipulating geolocation is supported for this driver.
        /// </summary>
        bool HasLocationContext { get; }

        /// <summary>
        /// Gets an <see cref="ILocationContext"/> object for managing browser location.
        /// </summary>
        ILocationContext LocationContext { get; }
    }
}
