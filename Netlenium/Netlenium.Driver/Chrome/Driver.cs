using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netlenium.Logging;

namespace Netlenium.Driver.Chrome
{
    public class Driver : IDriver
    {

        private Service logging;
        
        private bool driverManagerEnabled;

        private string driverDirectoryLocation;
   
        private Browser targetBrowser;

        private Actions actions;

        /// <summary>
        /// Logging Service and Properties
        /// </summary>
        public Service Logging { get => logging; set => logging = value; }


        /// <summary>
        /// Property for using the Driver Manager, if this is disabled the
        /// location for the driver must be set manually otherwise an
        /// exception would be thrown
        /// </summary>
        public bool DriverManagerEnabled { get => driverManagerEnabled; set => driverManagerEnabled = value; }

        /// <summary>
        /// The driver directory location that is set manually if
        /// DriverManager is disabled
        /// </summary>
        public string DriverDirectoryLcoation { get => driverDirectoryLocation; set => driverDirectoryLocation = value; }

        public Browser TargetBrowser => targetBrowser;

        public IActions Actions => actions;

        public IDocument Document => throw new NotImplementedException();

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="targetBrowser"></param>
        public void IDriver(Browser targetBrowser)
        {
            this.logging = new Service()
            {
                Name = "Netlenium.Driver",
                InformationEntriesEnabled = true,
                WarningEntriesEnabled = true,
                ErrorEntriesEnabled = true,
                VerboseEntriesEnabled = true,
                DebuggingEntriesEnabled = true
            };

            this.targetBrowser = targetBrowser;
            this.actions = new Actions(this);
        }
    }
}
