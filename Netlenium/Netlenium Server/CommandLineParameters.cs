namespace NetleniumServer
{
    /// <summary>
    /// The Parameters used for this Application
    /// </summary>
    public static class CommandLineParameters
    {
        /// <summary>
        /// Indiciates if the Help menu should only be displayed
        /// </summary>
        public static bool Help { get; set; }

        /// <summary>
        /// Disables standard output
        /// </summary>
        public static bool DisabledStdout { get; set; }

        /// <summary>
        /// Disables logging to files
        /// </summary>
        public static bool DisableFileLogging { get; set; }

        /// <summary>
        /// The logging level for Netlenium Drivers
        /// </summary>
        public static int DriverLoggingLevel { get; set; }

        /// <summary>
        /// The logging level for the Web Serivce
        /// </summary>
        public static int ServerLoggingLevel { get; set; }

        /// <summary>
        /// The port that the Web Service will run on
        /// </summary>
        public static int Port { get; set; }

        /// <summary>
        /// Optional custom name for this server
        /// </summary>
        public static string ServerName { get; set; }

        /// <summary>
        /// The maximum amount of sessions that can be created
        /// </summary>
        public static int MaxSessions { get; set; }

        /// <summary>
        /// The amount of minutes the session is allowed to be inactive before it gets closed
        /// </summary>
        public static int SessionInactivityLimit { get; set; }

        /// <summary>
        /// Disables the ability to start Chrome Drivers
        /// </summary>
        public static bool DisableChromeDriver { get; set; }

        /// <summary>
        /// Disables the ability to start Firefox Drivers
        /// </summary>
        public static bool DisableFirefoxDriver { get; set; }

        /// <summary>
        /// Disables the ability to start Opera Drivers
        /// </summary>
        public static bool DisableOperaDriver { get; set; }

        /// <summary>
        /// Authenticaiton Password for Web Service access
        /// </summary>
        public static string AuthPassword { get; set; }
        
    }
}
