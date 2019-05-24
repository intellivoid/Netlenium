namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Defines an interface allowing the user to access application cache status
    /// </summary>
    public interface IApplicationCache
    {
        /// <summary>
        /// Gets the current state of the application cache.
        /// </summary>
        AppCacheStatus Status { get; }
    }
}
