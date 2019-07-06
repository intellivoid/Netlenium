using System;
using Netlenium.Logging;

namespace Netlenium.Driver.Chrome
{
    /// <summary>
    /// Chrome Driver
    /// </summary>
    public class Driver : IDriver
    {
        private Service logging;
        public Service Logging => logging;

        private IDriverManager driverManager;
        public IDriverManager DriverManager { get => driverManager; }

        public Browser TargetBrowser => Browser.Chrome;

        public IActions Actions => throw new NotImplementedException();

        public IDocument Document => throw new NotImplementedException();

        private bool headless;
        public bool Headless { get => headless; set => headless = value; }

        private Platform targetPlatform;
        public Platform TargetPlatform {
            get => targetPlatform;
            set
            {
                if(value == Platform.AutoDetect)
                {
                    targetPlatform = Utilities.DetectPlatform();
                }
                else
                {
                    targetPlatform = value;
                }
            }
        }

        public Driver()
        {
            logging = new Service()
            {
                CommandLineLoggingEnabled = true,
                InformationEntriesEnabled = true,
                DebuggingEntriesEnabled = true,
                ErrorEntriesEnabled = true,
                VerboseEntriesEnabled = true,
                WarningEntriesEnabled = true,
                Name = "Netlenium.Driver.Chrome"
            };

            driverManager = new DriverManager(this);
            headless = true;
            targetPlatform = Platform.AutoDetect;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
