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
        /// Property for using the Driver Manager, if this is disabled the
        /// location for the driver must be set manually otherwise an
        /// exception would be thrown
        /// </summary>
        bool DriverManagerEnabled { get; set; }

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
       
    }
}
