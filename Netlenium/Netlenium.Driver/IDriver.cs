namespace Netlenium.Driver
{
    /// <summary>
    /// Driver Interface
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="targetBrowser"></param>
        void IDriver(Browser targetBrowser);

        /// <summary>
        /// Logging properties of this Driver
        /// </summary>
        Logging.Service Logging { get; set; }

        /// <summary>
        /// Driver Manager for this Driver
        /// </summary>
        IDriverManager DriverManager { get; set; }

        /// <summary>
        /// The driver directory location if DriverManager is disabled
        /// </summary>
        string DriverDirectoryLcoation { get; set; }

        /// <summary>
        /// The target browser that this driver is going to control
        /// </summary>
        Browser TargetBrowser { get; }

        /// <summary>
        /// Driver Actions
        /// </summary>
        IActions Actions { get; }

        /// <summary>
        /// Current Document Properties
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        /// Initalizes the driver by checking for updates or installing
        /// missing resources for the selected platform
        /// </summary>
        void Initalize();
       
    }
}
