using System;
using System.Collections.Generic;
using System.Drawing;
using Netlenium.Driver.WebDriver.Firefox;
using Netlenium.Driver.WebDriver.Remote;
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
        /// The Driver Service process manager
        /// </summary>
        private FirefoxDriverService DriverService { get; set; }

        /// <summary>
        /// Options that are passed on to the Chrome Driver
        /// </summary>
        private FirefoxOptions DriverOptions { get; set; }

        /// <summary>
        /// Firefox Driver Profile used for setting options
        /// </summary>
        private FirefoxProfile DriverProfile { get; set; }

        /// <summary>
        /// Remote Driver Client for controlling the Driver Service
        /// </summary>
        public RemoteWebDriver RemoteDriver { get; set; }

        /// <summary>
        /// The Window size that is set if running in headless mode
        /// </summary>
        private Size HeadlessWindowSize { get; set; }

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

            HeadlessWindowSize = new Size(1920, 1080);

            driverManager = new DriverManager(this);
            headless = true;
            targetPlatform = Platform.AutoDetect;
        }


        /// <summary>
        /// Sets the current options for Chrome Driver
        /// </summary>
        /// <param name="options"></param>
        private void SetOptions(Dictionary<string, string> options)
        {
            DriverOptions = new FirefoxOptions();

            foreach (var option in options)
            {
                var Argument = option.Value == string.Empty ? $"-{option.Key}" : $"-{option.Key}={option.Value}";
                logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting argument '{Argument}'");
                DriverOptions.AddArgument(Argument);
            }
        }

        public void Start()
        {
            if (DriverService != null)
            {
                if (DriverService.IsRunning == true)
                {
                    logging.WriteEntry(MessageType.Error, "Driver", "Remote Driver Service cannot be stopped because it is not running");
                    throw new DriverAlreadyRunningException();
                }
            }

            DriverManager.Initalize();

            logging.WriteEntry(MessageType.Information, "Driver", "Starting remote driver service");

            logging.WriteEntry(MessageType.Debugging, "Driver", $"Working Directory: {DriverManager.DriverDirectoryPath}");
            logging.WriteEntry(MessageType.Debugging, "Driver", $"Driver Executable: {DriverManager.DriverExecutableName}");
            DriverService = FirefoxDriverService.CreateDefaultService(DriverManager.DriverDirectoryPath, DriverManager.DriverExecutableName);
            logging.WriteEntry(MessageType.Debugging, "Driver", "DriverService Constructed");

            try
            {
                logging.WriteEntry(MessageType.Verbose, "Driver", "Creating new Firefox Profile");
                DriverProfile = new FirefoxProfile();
                
                DriverProfile.WriteToDisk();
                logging.WriteEntry(MessageType.Verbose, "Driver", $"Profile Created: '{DriverProfile.ProfileDirectory}'");
            }
            catch(Exception ex)
            {
                logging.WriteEntry(MessageType.Error, "Driver", $"Cannot create profile, {ex.Message}");
            }
            
            logging.WriteEntry(MessageType.Verbose, "Driver", "Creating options for driver");
            var options = new Dictionary<string, string>();

            if (Headless == true)
            {
                logging.WriteEntry(MessageType.Debugging, "Driver", $"Headless mode with window-size of {HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
                //options.Add("headless", string.Empty);
                //options.Add("window-size", $"{HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
            }

            //options.Add("profile", DriverProfile.ProfileDirectory);
            //options.Add("profile", "C:\\Users\\Netkas\\AppData\\Local\\Temp\\rust_mozprofile.ffTe22DD6cOY");

            //if (proxyConfiguration.Enabled == true)
            //{
            //    var ProxyExtensionPath = proxyConfiguration.BuildExtension();
            //    options.Add("load-extension", ProxyExtensionPath);
            //}

            DriverService.HideCommandPromptWindow = false;
            if (driverLoggingEnabled == false)
            {
                //options.Add("log-level", "0");
                //options.Add("silent", string.Empty);
                //DriverService.SuppressInitialDiagnosticInformation = true;
            }
            else
            {
                if (driverVerboseLoggingEnabled == true)
                {
                    //options.Add("log-level", "1");
                    //DriverService.EnableVerboseLogging = true;
                }
                else
                {
                    //options.Add("log-level", "2");
                    //DriverService.EnableVerboseLogging = false;
                }

                DriverService.SuppressInitialDiagnosticInformation = false;
            }

            logging.WriteEntry(MessageType.Verbose, "Driver", "Setting options for driver");
            SetOptions(options);

            logging.WriteEntry(MessageType.Verbose, "Driver", "Starting Driver Service");
            DriverService.Start();

            logging.WriteEntry(MessageType.Debugging, "Driver", $"Service URL: {DriverService.ServiceUrl}");
            logging.WriteEntry(MessageType.Debugging, "Driver", $"Process ID: {DriverService.ProcessId}");
            logging.WriteEntry(MessageType.Debugging, "Driver", $"Port: {DriverService.Port}");

            logging.WriteEntry(MessageType.Verbose, "Driver", $"Connecting to '{DriverService.ServiceUrl}'");

            DriverOptions.SetPreference("media.volume_scale", "0.0");
            RemoteDriver = new RemoteWebDriver(DriverService.ServiceUrl, DriverOptions);

            logging.WriteEntry(MessageType.Information, "Driver", "Remote Driver Serivce Started");
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
