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

        private Actions actions;
        public IActions Actions => actions;

        private Document document;
        public IDocument Document => document;

        private Proxy proxyConfiguration;
        public IProxy ProxyConfiguration => proxyConfiguration;

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
                FileLoggingEnabled = true,
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
            actions = new Actions(this);
            document = new Document(this);
            proxyConfiguration = new Proxy();
            headless = true;
            targetPlatform = Platform.AutoDetect;
        }


        /// <summary>
        /// Sets the current options for Chrome Driver
        /// </summary>
        /// <param name="options"></param>
        private void SetOptions(Dictionary<string, string> options)
        {
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


            logging.WriteEntry(MessageType.Verbose, "Driver", "Creating options for driver");
            DriverOptions = new FirefoxOptions();

            var options = new Dictionary<string, string>();

            if (Headless == true)
            {
                logging.WriteEntry(MessageType.Debugging, "Driver", $"Headless mode with window-size of {HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
                options.Add("headless", string.Empty);
                options.Add("window-size", $"{HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
                DriverOptions.SetPreference("media.volume_scale", "0.0");
            }
            
            if (proxyConfiguration.Enabled == true)
            {
                logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting HTTP Proxy {string.Format("{0}:{1}", proxyConfiguration.Host, proxyConfiguration.Port)}");
                DriverOptions.Proxy.HttpProxy = string.Format("{0}:{1}", proxyConfiguration.Host, proxyConfiguration.Port);
                logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting FTP Proxy {string.Format("{0}:{1}", proxyConfiguration.Host, proxyConfiguration.Port)}");
                DriverOptions.Proxy.FtpProxy = string.Format("{0}:{1}", proxyConfiguration.Host, proxyConfiguration.Port);
                logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting SOCKS Proxy {string.Format("{0}:{1}", proxyConfiguration.Host, proxyConfiguration.Port)}");
                DriverOptions.Proxy.SocksProxy = string.Format("{0}:{1}", proxyConfiguration.Host, proxyConfiguration.Port);

                if (proxyConfiguration.AuthenticationRequried == true)
                {
                    logging.WriteEntry(MessageType.Debugging, "Driver", "Setting SOCKS Proxy authentication");
                    DriverOptions.Proxy.SocksUserName = proxyConfiguration.Username;
                    DriverOptions.Proxy.SocksPassword = proxyConfiguration.Password;
                }
            }

            DriverService.HideCommandPromptWindow = false;
            var DriverCommandLineOptions = string.Empty;
            if (driverLoggingEnabled == false)
            {
                DriverCommandLineOptions = "--log fatal";
                DriverService.SuppressInitialDiagnosticInformation = true;
            }
            else
            {
                if (driverVerboseLoggingEnabled == true)
                {
                    DriverCommandLineOptions = "--log debug";
                }
                else
                {
                    DriverCommandLineOptions = "--log info";
                }

                DriverService.SuppressInitialDiagnosticInformation = false;
            }

            logging.WriteEntry(MessageType.Debugging, "Driver", $"Using arguments for driver '{DriverCommandLineOptions}'");
            logging.WriteEntry(MessageType.Verbose, "Driver", "Setting options for driver");
            SetOptions(options);

            logging.WriteEntry(MessageType.Verbose, "Driver", "Starting Driver Service");
            DriverService.Start(DriverCommandLineOptions);

            logging.WriteEntry(MessageType.Debugging, "Driver", $"Service URL: {DriverService.ServiceUrl}");
            logging.WriteEntry(MessageType.Debugging, "Driver", $"Process ID: {DriverService.ProcessId}");
            logging.WriteEntry(MessageType.Debugging, "Driver", $"Port: {DriverService.Port}");

            logging.WriteEntry(MessageType.Verbose, "Driver", $"Connecting to '{DriverService.ServiceUrl}'");
            RemoteDriver = new RemoteWebDriver(DriverService.ServiceUrl, DriverOptions);

            logging.WriteEntry(MessageType.Information, "Driver", "Remote Driver Serivce Started");
        }

        public void Stop()
        {
            if (DriverService != null)
            {
                logging.WriteEntry(MessageType.Information, "Driver", "Quitting remote driver");
                RemoteDriver.Quit();
                logging.WriteEntry(MessageType.Verbose, "Driver", "Disposing Driver Service");
                DriverService.Dispose();
                logging.WriteEntry(MessageType.Information, "Driver", "Remote Driver Service stopped");
            }

            logging.WriteEntry(MessageType.Error, "Driver", "Remote Driver Service cannot be stopped because it is not running");
            throw new DriverNotRunningException();
        }
    }
}
