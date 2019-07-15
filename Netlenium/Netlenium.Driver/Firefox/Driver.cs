using System;
using Netlenium.Logging;

namespace Netlenium.Driver.Firefox
{
    public class Driver : IDriver
    {
        private Service logging;
        public Service Logging => logging;

        private readonly IDriverManager driverManager;
        public IDriverManager DriverManager { get => driverManager; }

        public Browser TargetBrowser => Browser.Firefox;

        private Platform targetPlatform;
        public Platform TargetPlatform
        {
            get
            {
                if (targetPlatform == Platform.AutoDetect)
                {
                    logging.WriteEntry(MessageType.Verbose, "Driver", "Auto detecting platform");
                    targetPlatform = Utilities.DetectPlatform();
                    logging.WriteEntry(MessageType.Verbose, "Driver", string.Format("Detected Platform: {0}", targetPlatform));
                }

                return targetPlatform;
            }
            set
            {
                if (value == Platform.AutoDetect)
                {
                    logging.WriteEntry(MessageType.Verbose, "Driver", "Auto detecting platform");
                    targetPlatform = Utilities.DetectPlatform();
                    logging.WriteEntry(MessageType.Verbose, "Driver", string.Format("Detected Platform: {0}", targetPlatform));
                }
                else
                {
                    targetPlatform = value;
                }
            }
        }

        public IActions Actions => throw new NotImplementedException();

        public IDocument Document => throw new NotImplementedException();

        public IProxy ProxyConfiguration => throw new NotImplementedException();

        private bool headless;
        public bool Headless { get => headless; set => headless = value; }

        private bool driverLoggingEnabled;
        public bool DriverLoggingEnabled { get => driverLoggingEnabled; set => driverLoggingEnabled = value; }

        private bool driverVerboseLoggingEnabled;
        public bool DriverVerboseLoggingEnabled { get => driverVerboseLoggingEnabled; set => driverVerboseLoggingEnabled = value; }


        /// <summary>
        /// Public Constructor
        /// </summary>
        public Driver()
        {
            logging = new Service()
            {
                CommandLineLoggingEnabled = true,
                DebuggingOutputEnabled = true,
                InformationEntriesEnabled = true,
                DebuggingEntriesEnabled = true,
                ErrorEntriesEnabled = true,
                VerboseEntriesEnabled = true,
                WarningEntriesEnabled = true,
                Name = "Netlenium.Driver.Firefox"
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
