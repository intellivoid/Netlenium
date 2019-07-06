namespace Netlenium.Driver
{
    /// <summary>
    /// Driver Interface
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Logging properties of this Driver
        /// </summary>
        Logging.Service Logging { get; }

        /// <summary>
        /// Driver Manager for this Driver
        /// </summary>
        IDriverManager DriverManager { get; }

        /// <summary>
        /// The target browser that this driver is going to control
        /// </summary>
        Browser TargetBrowser { get; }

        /// <summary>
        /// The target platform that this driver will attempt to work with
        /// </summary>
        Platform TargetPlatform { get; set; }

        /// <summary>
        /// Driver Actions
        /// </summary>
        IActions Actions { get; }

        /// <summary>
        /// Current Document loaded in the Driver
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        /// If the browser should run in headless mode before startingz
        /// </summary>
        bool Headless { get; set; }
        
        /// <summary>
        /// If set to true, the driver will display logging details
        /// </summary>
        bool DriverLoggingEnabled { get; set; }

        /// <summary>
        /// If set to true, the driver will display verbose logging details if Logging is enabled
        /// </summary>
        bool DriverVerboseLoggingEnabled { get; set; }

        /// <summary>
        /// Starts the driver
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the driver
        /// </summary>
        void Stop();
    }
}
