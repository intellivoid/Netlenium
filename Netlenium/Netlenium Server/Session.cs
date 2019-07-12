using Netlenium.Driver;
using System;

namespace NetleniumServer
{
    /// <summary>
    /// Session Object
    /// </summary>
    public class Session
    {

        /// <summary>
        /// Internal Driver Object
        /// </summary>
        private IDriver driver;

        /// <summary>
        /// The driver associated with this session
        /// </summary>
        public IDriver Driver
        {
            get
            {
                LastActivity = DateTime.Now;
                return driver;
            }
        }

        /// <summary>
        /// The session ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The timestamp that this session has started
        /// </summary>
        public DateTime SessionStarted { get; set; }

        /// <summary>
        /// The session's last activity
        /// </summary>
        public DateTime LastActivity { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Session(Browser targetBrowser)
        {
            switch(targetBrowser)
            {
                case Browser.Chrome:
                    driver = new Netlenium.Driver.Chrome.Driver();
                    break;
            }

            switch(CommandLineParameters.DriverLoggingLevel)
            {
                case 0:
                    Driver.Logging.InformationEntriesEnabled = false;
                    Driver.Logging.WarningEntriesEnabled = false;
                    Driver.Logging.ErrorEntriesEnabled = false;
                    Driver.Logging.VerboseEntriesEnabled = false;
                    Driver.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 1:
                    Driver.Logging.InformationEntriesEnabled = true;
                    Driver.Logging.WarningEntriesEnabled = true;
                    Driver.Logging.ErrorEntriesEnabled = true;
                    Driver.Logging.VerboseEntriesEnabled = false;
                    Driver.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 2:
                    Driver.Logging.InformationEntriesEnabled = true;
                    Driver.Logging.WarningEntriesEnabled = true;
                    Driver.Logging.ErrorEntriesEnabled = true;
                    Driver.Logging.VerboseEntriesEnabled = true;
                    Driver.Logging.DebuggingEntriesEnabled = false;
                    break;

                case 3:
                    Driver.Logging.InformationEntriesEnabled = true;
                    Driver.Logging.WarningEntriesEnabled = true;
                    Driver.Logging.ErrorEntriesEnabled = true;
                    Driver.Logging.VerboseEntriesEnabled = true;
                    Driver.Logging.DebuggingEntriesEnabled = true;
                    break;
            }
            
            SessionStarted = DateTime.Now;
            LastActivity = SessionStarted;
        }
    }
}
