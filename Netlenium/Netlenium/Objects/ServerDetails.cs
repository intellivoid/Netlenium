using System;
using System.Reflection;

namespace Netlenium.Objects
{
    /// <summary>
    /// Server Details object
    /// </summary>
    [Serializable]
    public class ServerDetails
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        public ServerDetails()
        {
            ServerName = CommandLineParameters.ServerName;
            ServerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ChromeDriversDisabled = CommandLineParameters.DisableChromeDriver;
            OperaDriversDisabled = CommandLineParameters.DisableOperaDriver;
            FirefoxDriversDisabled = CommandLineParameters.DisableFirefoxDriver;
            DefaultDriver = CommandLineParameters.DefaultDriver;
            CurrentSessions = SessionManager.TotalActiveSessions;
            MaximumSessions = CommandLineParameters.MaxSessions;
            SessionInactivityLimit = CommandLineParameters.SessionInactivityLimit;
        }
        
        public string ServerName { get; set; }

        public string ServerVersion { get; set; }

        public bool ChromeDriversDisabled { get; set; }

        public bool OperaDriversDisabled { get; set; }

        public bool FirefoxDriversDisabled { get; set; }

        public string DefaultDriver { get; set; }

        public int CurrentSessions { get; set; }

        public int MaximumSessions { get; set; }

        public int SessionInactivityLimit { get; set; }
    }
}
