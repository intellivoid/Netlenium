namespace Netlenium
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
        /// Fetches missing driver resources and or updates outdated resources then exits
        /// </summary>
        public static bool UpdateDrivers { get; set; }

        /// <summary>
        /// Clears unused cache files created by Netlenium then exits
        /// </summary>
        public static bool ClearCache { get; set; }

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
        /// The maximum number of sessions that can be created
        /// </summary>
        public static int MaxSessions { get; set; }

        /// <summary>
        /// The amount of minutes that a session is allowed to be inactive for
        /// </summary>
        public static int SessionInactivityLimit { get; set; }

        /// <summary>
        /// The default driver to start when no target browser is provided
        /// </summary>
        public static string DefaultDriver { get; set; }

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

        /// <summary>
        /// Authentication Password for Administrating the server
        /// </summary>
        public static string AdministratorPassword { get; set; }
        
    }
}
