namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Represents the application cache status.
    /// </summary>
    public enum AppCacheStatus
    {
        /// <summary>
        /// AppCache status is uncached
        /// </summary>
        Uncached = 0,

        /// <summary>
        /// AppCache status is idle
        /// </summary>
        Idle = 1,

        /// <summary>
        /// AppCache status is checkint
        /// </summary>
        Checking,

        /// <summary>
        /// AppCache status is downloading
        /// </summary>
        Downloading,

        /// <summary>
        /// AppCache status is updated-ready
        /// </summary>
        UpdateReady,

        /// <summary>
        /// AppCache status is obsolete
        /// </summary>
        Obsolete
    }
}
