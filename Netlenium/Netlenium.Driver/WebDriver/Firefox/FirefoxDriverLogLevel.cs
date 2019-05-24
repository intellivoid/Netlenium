namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Represents the valid values of logging levels available with the Firefox driver (geckodriver.exe).
    /// </summary>
    public enum FirefoxDriverLogLevel
    {
        /// <summary>
        /// Represents the Trace value, the most detailed logging level available.
        /// </summary>
        Trace,

        /// <summary>
        /// Represents the Debug value
        /// </summary>
        Debug,

        /// <summary>
        /// Represents the Config value
        /// </summary>
        Config,

        /// <summary>
        /// Represents the Info value
        /// </summary>
        Info,

        /// <summary>
        /// Represents the Warn value
        /// </summary>
        Warn,

        /// <summary>
        /// Represents the Error value
        /// </summary>
        Error,

        /// <summary>
        /// Represents the Fatal value, the least detailed logging level available.
        /// </summary>
        Fatal,

        /// <summary>
        /// Represents that the logging value is unspecified, and should be the default level.
        /// </summary>
        Default
    }
}
