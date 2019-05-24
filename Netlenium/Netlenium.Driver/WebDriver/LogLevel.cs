namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Represents the levels of logging available to driver instances.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Show all log messages.
        /// </summary>
        All,

        /// <summary>
        /// Show messages with information useful for debugging.
        /// </summary>
        Debug,

        /// <summary>
        /// Show informational messages.
        /// </summary>
        Info,

        /// <summary>
        /// Show messages corresponding to non-critical issues.
        /// </summary>
        Warning,

        /// <summary>
        /// Show messages corresponding to critical issues.
        /// </summary>
        Severe,

        /// <summary>
        /// Show no log messages.
        /// </summary>
        Off
    }
}
