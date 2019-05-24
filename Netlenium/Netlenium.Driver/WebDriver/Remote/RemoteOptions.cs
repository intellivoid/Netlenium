namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a mechanism for setting options needed for the driver during the test.
    /// </summary>
    internal class RemoteOptions : IOptions
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteOptions"/> class
        /// </summary>
        /// <param name="driver">Instance of the driver currently in use</param>
        public RemoteOptions(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Gets an object allowing the user to manipulate cookies on the page.
        /// </summary>
        public ICookieJar Cookies
        {
            get { return new RemoteCookieJar(driver); }
        }

        /// <summary>
        /// Gets an object allowing the user to manipulate the currently-focused browser window.
        /// </summary>
        /// <remarks>"Currently-focused" is defined as the browser window having the window handle
        /// returned when IWebDriver.CurrentWindowHandle is called.</remarks>
        public IWindow Window
        {
            get { return new RemoteWindow(driver); }
        }

        /// <summary>
        /// Gets an object allowing the user to examine the logs of the current driver instance.
        /// </summary>
        public ILogs Logs
        {
            get { return new RemoteLogs(driver); }
        }

        /// <summary>
        /// Provides access to the timeouts defined for this driver.
        /// </summary>
        /// <returns>An object implementing the <see cref="ITimeouts"/> interface.</returns>
        public ITimeouts Timeouts()
        {
            return new RemoteTimeouts(driver);
        }
    }
}
