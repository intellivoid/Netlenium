using System.Collections.Generic;
using System.Drawing;
using Netlenium.Driver.WebDriver.Firefox;
using Netlenium.Driver.WebDriver.Remote;
using Netlenium.Logging;

namespace Netlenium.Driver.Firefox
{
    /// <inheritdoc />
    /// <summary>
    /// Firefox Driver
    /// </summary>
    public class Driver : IDriver
    {
        public Service Logging { get; }

        public IDriverManager DriverManager { get; }

        public Browser TargetBrowser => Browser.Firefox;

        private Platform targetPlatform;
        
        public Platform TargetPlatform
        {
            get
            {
                if (targetPlatform != Platform.AutoDetect) return targetPlatform;
                Logging.WriteEntry(MessageType.Verbose, "Driver", "Auto detecting platform");
                targetPlatform = Utilities.DetectPlatform();
                Logging.WriteEntry(MessageType.Verbose, "Driver", $"Detected Platform: {targetPlatform}");

                return targetPlatform;
            }
            set
            {
                if (value == Platform.AutoDetect)
                {
                    Logging.WriteEntry(MessageType.Verbose, "Driver", "Auto detecting platform");
                    targetPlatform = Utilities.DetectPlatform();
                    Logging.WriteEntry(MessageType.Verbose, "Driver", $"Detected Platform: {targetPlatform}");
                }
                else
                {
                    targetPlatform = value;
                }
            }
        }

        private readonly Actions actions;
        public IActions Actions => actions;

        private readonly Document document;
        public IDocument Document => document;

        private readonly Proxy proxyConfiguration;
        public IProxy ProxyConfiguration => proxyConfiguration;

        public bool Headless { get; set; }

        public bool DriverLoggingEnabled { get; set; }

        public bool DriverVerboseLoggingEnabled { get; set; }

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
        public RemoteWebDriver RemoteDriver { get; private set; }

        /// <summary>
        /// The Window size that is set if running in headless mode
        /// </summary>
        private Size HeadlessWindowSize { get; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Driver()
        {
            Logging = new Service
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

            DriverManager = new DriverManager(this);
            actions = new Actions(this);
            document = new Document(this);
            proxyConfiguration = new Proxy();
            Headless = true;
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
                var argument = option.Value == string.Empty ? $"-{option.Key}" : $"-{option.Key}={option.Value}";
                Logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting argument '{argument}'");
                DriverOptions.AddArgument(argument);
            }
        }

        public void Start()
        {
            if (DriverService != null)
            {
                if (DriverService.IsRunning)
                {
                    Logging.WriteEntry(MessageType.Error, "Driver", "Remote Driver Service cannot be stopped because it is not running");
                    throw new DriverAlreadyRunningException();
                }
            }

            DriverManager.Initalize();

            Logging.WriteEntry(MessageType.Information, "Driver", "Starting remote driver service");

            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Working Directory: {DriverManager.DriverDirectoryPath}");
            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Driver Executable: {DriverManager.DriverExecutableName}");
            DriverService = FirefoxDriverService.CreateDefaultService(DriverManager.DriverDirectoryPath, DriverManager.DriverExecutableName);
            Logging.WriteEntry(MessageType.Debugging, "Driver", "DriverService Constructed");


            Logging.WriteEntry(MessageType.Verbose, "Driver", "Creating options for driver");
            DriverOptions = new FirefoxOptions();

            var options = new Dictionary<string, string>();

            if (Headless)
            {
                Logging.WriteEntry(MessageType.Debugging, "Driver", $"Headless mode with window-size of {HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
                options.Add("headless", string.Empty);
                options.Add("window-size", $"{HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
                DriverOptions.SetPreference("media.volume_scale", "0.0");
            }
            
            if (proxyConfiguration.Enabled)
            {
                Logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting HTTP Proxy {proxyConfiguration.Host}:{proxyConfiguration.Port}");
                DriverOptions.Proxy.HttpProxy = $"{proxyConfiguration.Host}:{proxyConfiguration.Port}";
                Logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting FTP Proxy {proxyConfiguration.Host}:{proxyConfiguration.Port}");
                DriverOptions.Proxy.FtpProxy = $"{proxyConfiguration.Host}:{proxyConfiguration.Port}";
                Logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting SOCKS Proxy {proxyConfiguration.Host}:{proxyConfiguration.Port}");
                DriverOptions.Proxy.SocksProxy = $"{proxyConfiguration.Host}:{proxyConfiguration.Port}";

                if (proxyConfiguration.AuthenticationRequried)
                {
                    Logging.WriteEntry(MessageType.Debugging, "Driver", "Setting SOCKS Proxy authentication");
                    DriverOptions.Proxy.SocksUserName = proxyConfiguration.Username;
                    DriverOptions.Proxy.SocksPassword = proxyConfiguration.Password;
                }
            }

            DriverService.HideCommandPromptWindow = false;
            string driverCommandLineOptions;
            if (DriverLoggingEnabled == false)
            {
                driverCommandLineOptions = "--log fatal";
                DriverService.SuppressInitialDiagnosticInformation = true;
            }
            else
            {
                driverCommandLineOptions = DriverVerboseLoggingEnabled ? "--log debug" : "--log info";
                DriverService.SuppressInitialDiagnosticInformation = false;
            }

            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Using arguments for driver '{driverCommandLineOptions}'");
            Logging.WriteEntry(MessageType.Verbose, "Driver", "Setting options for driver");
            SetOptions(options);

            Logging.WriteEntry(MessageType.Verbose, "Driver", "Starting Driver Service");
            DriverService.Start(driverCommandLineOptions);

            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Service URL: {DriverService.ServiceUrl}");
            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Process ID: {DriverService.ProcessId}");
            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Port: {DriverService.Port}");

            Logging.WriteEntry(MessageType.Verbose, "Driver", $"Connecting to '{DriverService.ServiceUrl}'");
            RemoteDriver = new RemoteWebDriver(DriverService.ServiceUrl, DriverOptions);

            Logging.WriteEntry(MessageType.Information, "Driver", "Remote Driver Service Started");
        }

        public void Stop()
        {
            if (DriverService != null)
            {
                Logging.WriteEntry(MessageType.Information, "Driver", "Quitting remote driver");
                RemoteDriver.Quit();
                Logging.WriteEntry(MessageType.Verbose, "Driver", "Disposing Driver Service");
                DriverService.Dispose();
                Logging.WriteEntry(MessageType.Information, "Driver", "Remote Driver Service stopped");
            }

            Logging.WriteEntry(MessageType.Error, "Driver", "Remote Driver Service cannot be stopped because it is not running");
            throw new DriverNotRunningException();
        }
    }
}
