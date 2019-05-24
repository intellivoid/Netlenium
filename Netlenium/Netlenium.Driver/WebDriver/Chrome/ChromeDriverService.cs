using System;
using System.Globalization;
using System.Text;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Chrome
{
    /// <summary>
    /// Exposes the service provided by the native ChromeDriver executable.
    /// </summary>
    public sealed class ChromeDriverService : DriverService
    {
        private const string DefaultChromeDriverServiceExecutableName = "chromedriver";

        private static readonly Uri ChromeDriverDownloadUrl = new Uri("http://chromedriver.storage.googleapis.com/index.html");
        private string logPath = string.Empty;
        private string urlPathPrefix = string.Empty;
        private string portServerAddress = string.Empty;
        private string whitelistedIpAddresses = string.Empty;
        private int adbPort = -1;
        private bool enableVerboseLogging;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeDriverService"/> class.
        /// </summary>
        /// <param name="executablePath">The full path to the ChromeDriver executable.</param>
        /// <param name="executableFileName">The file name of the ChromeDriver executable.</param>
        /// <param name="port">The port on which the ChromeDriver executable should listen.</param>
        private ChromeDriverService(string executablePath, string executableFileName, int port)
            : base(executablePath, port, executableFileName, ChromeDriverDownloadUrl)
        {
        }

        /// <summary>
        /// Gets or sets the location of the log file written to by the ChromeDriver executable.
        /// </summary>
        public string LogPath
        {
            get { return logPath; }
            set { logPath = value; }
        }

        /// <summary>
        /// Gets or sets the base URL path prefix for commands (e.g., "wd/url").
        /// </summary>
        public string UrlPathPrefix
        {
            get { return urlPathPrefix; }
            set { urlPathPrefix = value; }
        }

        /// <summary>
        /// Gets or sets the address of a server to contact for reserving a port.
        /// </summary>
        public string PortServerAddress
        {
            get { return portServerAddress; }
            set { portServerAddress = value; }
        }

        /// <summary>
        /// Gets or sets the port on which the Android Debug Bridge is listening for commands.
        /// </summary>
        public int AndroidDebugBridgePort
        {
            get { return adbPort; }
            set { adbPort = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable verbose logging for the ChromeDriver executable.
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public bool EnableVerboseLogging
        {
            get { return enableVerboseLogging; }
            set { enableVerboseLogging = value; }
        }

        /// <summary>
        /// Gets or sets the comma-delimited list of IP addresses that are approved to
        /// connect to this instance of the Chrome driver. Defaults to an empty string,
        /// which means only the local loopback address can connect.
        /// </summary>
        public string WhitelistedIPAddresses
        {
            get { return whitelistedIpAddresses; }
            set { whitelistedIpAddresses = value; }
        }

        /// <summary>
        /// Gets the command-line arguments for the driver service.
        /// </summary>
        protected override string CommandLineArguments
        {
            get
            {
                var argsBuilder = new StringBuilder(base.CommandLineArguments);
                if (adbPort > 0)
                {
                    argsBuilder.AppendFormat(CultureInfo.InvariantCulture, " --adb-port={0}", adbPort);
                }

                if (SuppressInitialDiagnosticInformation)
                {
                    argsBuilder.Append(" --silent");
                }

                if (enableVerboseLogging)
                {
                    argsBuilder.Append(" --verbose");
                }

                if (!string.IsNullOrEmpty(logPath))
                {
                    argsBuilder.AppendFormat(CultureInfo.InvariantCulture, " --log-path={0}", logPath);
                }

                if (!string.IsNullOrEmpty(urlPathPrefix))
                {
                    argsBuilder.AppendFormat(CultureInfo.InvariantCulture, " --url-base={0}", urlPathPrefix);
                }

                if (!string.IsNullOrEmpty(portServerAddress))
                {
                    argsBuilder.AppendFormat(CultureInfo.InvariantCulture, " --port-server={0}", portServerAddress);
                }

                if (!string.IsNullOrEmpty(whitelistedIpAddresses))
                {
                    argsBuilder.Append(string.Format(CultureInfo.InvariantCulture, " -whitelisted-ips={0}", whitelistedIpAddresses));
                }

                return argsBuilder.ToString();
            }
        }

        /// <summary>
        /// Creates a default instance of the ChromeDriverService.
        /// </summary>
        /// <returns>A ChromeDriverService that implements default settings.</returns>
        public static ChromeDriverService CreateDefaultService()
        {
            var serviceDirectory = FindDriverServiceExecutable(ChromeDriverServiceFileName(), ChromeDriverDownloadUrl);
            return CreateDefaultService(serviceDirectory);
        }

        /// <summary>
        /// Creates a default instance of the ChromeDriverService using a specified path to the ChromeDriver executable.
        /// </summary>
        /// <param name="driverPath">The directory containing the ChromeDriver executable.</param>
        /// <returns>A ChromeDriverService using a random port.</returns>
        public static ChromeDriverService CreateDefaultService(string driverPath)
        {
            return CreateDefaultService(driverPath, ChromeDriverServiceFileName());
        }

        /// <summary>
        /// Creates a default instance of the ChromeDriverService using a specified path to the ChromeDriver executable with the given name.
        /// </summary>
        /// <param name="driverPath">The directory containing the ChromeDriver executable.</param>
        /// <param name="driverExecutableFileName">The name of the ChromeDriver executable file.</param>
        /// <returns>A ChromeDriverService using a random port.</returns>
        public static ChromeDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
        {
            return new ChromeDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
        }

        /// <summary>
        /// Returns the Chrome driver filename for the currently running platform
        /// </summary>
        /// <returns>The file name of the Chrome driver service executable.</returns>
        private static string ChromeDriverServiceFileName()
        {
            var fileName = DefaultChromeDriverServiceExecutableName;

            // Unfortunately, detecting the currently running platform isn't as
            // straightforward as you might hope.
            // See: http://mono.wikia.com/wiki/Detecting_the_execution_platform
            // and https://msdn.microsoft.com/en-us/library/3a8hyw88(v=vs.110).aspx
            const int PlatformMonoUnixValue = 128;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    fileName += ".exe";
                    break;

                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    break;

                // Don't handle the Xbox case. Let default handle it.
                // case PlatformID.Xbox:
                //     break;
                default:
                    if ((int)Environment.OSVersion.Platform == PlatformMonoUnixValue)
                    {
                        break;
                    }

                    throw new WebDriverException("Unsupported platform: " + Environment.OSVersion.Platform);
            }

            return fileName;
        }
    }
}
