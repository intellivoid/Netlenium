using Netlenium.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer
{
    /// <summary>
    /// Session Object
    /// </summary>
    public class Session
    {
        /// <summary>
        /// The driver associated with this session
        /// </summary>
        public IDriver Driver { get; }

        /// <summary>
        /// The session ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Session(Browser targetBrowser)
        {
            switch(targetBrowser)
            {
                case Browser.Chrome:
                    Driver = new Netlenium.Driver.Chrome.Driver();
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
        }
    }
}
