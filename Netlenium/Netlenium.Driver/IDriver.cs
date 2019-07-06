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
        Logging.Service Logging { get; set; }

        /// <summary>
        /// Driver Manager for this Driver
        /// </summary>
        IDriverManager DriverManager { get; set; }

        /// <summary>
        /// The target browser that this driver is going to control
        /// </summary>
        Browser TargetBrowser { get; }

        /// <summary>
        /// Driver Actions
        /// </summary>
        IActions Actions { get; }

        /// <summary>
        /// Current Document loaded in the Driver
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        /// If the browser should run in headless mode before starting
        /// </summary>
        bool Headless { get; set; }

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
