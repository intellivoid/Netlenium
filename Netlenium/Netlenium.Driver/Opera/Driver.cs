using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Netlenium.Driver.WebDriver.Opera;
using Netlenium.Driver.WebDriver.Remote;
using Netlenium.Logging;

namespace Netlenium.Driver.Opera
{
    public class Driver : IDriver
    {
        public Service Logging { get; }
        public IDriverManager DriverManager { get; }
        
        public Browser TargetBrowser => Browser.Opera;

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
        
        public IActions Actions { get; }
        
        public IDocument Document { get; }
        
        public IProxy ProxyConfiguration { get; }
        
        public bool Headless { get; set; }
        
        public bool DriverLoggingEnabled { get; set; }
        
        public bool DriverVerboseLoggingEnabled { get; set; }
        
        /// <summary>
        /// The Driver Service process manager
        /// </summary>
        private OperaDriverService DriverService { get; set; }

        /// <summary>
        /// Options that are passed on to the Opera Driver
        /// </summary>
        private OperaOptions DriverOptions { get; set; }
        
        /// <summary>
        /// Remote Driver Client for controlling the Driver Service
        /// </summary>
        public RemoteWebDriver RemoteDriver { get; private set; }

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
                Name = "Netlenium.Driver.Opera"
            };

            HeadlessWindowSize = new Size(1920, 1080);
            DriverManager = new DriverManager(this);
            Actions = new Actions(this);
            Document = new Document(this);
            ProxyConfiguration = new Proxy();
            Headless = true;
            targetPlatform = Platform.AutoDetect;
        }

        
        /// <summary>
        /// Sets the current options for Firefox Driver
        /// </summary>
        /// <param name="options"></param>
        private void SetOptions(Dictionary<string, string> options)
        {
            DriverOptions = new OperaOptions();
            
            foreach (var option in options)
            {
                var argument = option.Value == string.Empty ? $"--{option.Key}" : $"--{option.Key}={option.Value}";
                Logging.WriteEntry(MessageType.Debugging, "Driver", $"Setting argument '{argument}'");
                DriverOptions.AddArgument(argument);
            }
        }
        
        private Size HeadlessWindowSize { get; set; }

        public void Start()
        {
             if(DriverService != null)
            {
                if(DriverService.IsRunning)
                {
                    Logging.WriteEntry(MessageType.Error, "Driver", "Remote Driver Service cannot be stopped because it is not running");
                    throw new DriverAlreadyRunningException();
                }
            }

            DriverManager.Initialize();

            Logging.WriteEntry(MessageType.Information, "Driver", "Starting remote driver service");

            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Working Directory: {DriverManager.DriverDirectoryPath}");
            Logging.WriteEntry(MessageType.Debugging, "Driver", $"Driver Executable: {DriverManager.DriverExecutableName}");
            DriverService = OperaDriverService.CreateDefaultService(DriverManager.DriverDirectoryPath, DriverManager.DriverExecutableName);
            Logging.WriteEntry(MessageType.Debugging, "Driver", "DriverService Constructed");


            Logging.WriteEntry(MessageType.Verbose, "Driver", "Creating options for driver");
            var options = new Dictionary<string, string>();

            if(Headless)
            {
                Logging.WriteEntry(MessageType.Debugging, "Driver", $"Headless mode with window-size of {HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
                options.Add("headless", string.Empty);
                options.Add("mute-audio", string.Empty);
                options.Add("window-size", $"{HeadlessWindowSize.Width}x{HeadlessWindowSize.Height}");
            }


            //if (proxyConfiguration.Enabled == true)
            //{
            //    var proxyExtensionPath = proxyConfiguration.BuildExtension();
            //    options.Add("load-extension", proxyExtensionPath);
            //}

            if (DriverLoggingEnabled == false)
            {
                options.Add("log-level", "0");
                options.Add("silent", string.Empty);
                DriverService.SuppressInitialDiagnosticInformation = true;
            }
            else
            {
                if(DriverVerboseLoggingEnabled)
                {
                    options.Add("log-level", "1");
                    DriverService.EnableVerboseLogging = true;
                }
                else
                {
                    options.Add("log-level", "2");
                    DriverService.EnableVerboseLogging = false;
                }

                DriverService.SuppressInitialDiagnosticInformation = false;
            }

            Logging.WriteEntry(MessageType.Verbose, "Driver", "Setting options for driver");
            SetOptions(options);

            var loggingPath = $"{ApplicationPaths.LoggingDirectory}{Path.DirectorySeparatorChar}chrome_driver.log";
            Logging.WriteEntry(MessageType.Verbose, "Driver", $"Setting logging path '{loggingPath}'");
            DriverService.LogPath = loggingPath;

            Logging.WriteEntry(MessageType.Verbose, "Driver", "Starting Driver Service");
            DriverService.Start();

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
                return;
            }

            Logging.WriteEntry(MessageType.Error, "Driver", "Remote Driver Service cannot be stopped because it is not running");
            throw new DriverNotRunningException();
        }
    }
}