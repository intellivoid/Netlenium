using System;
using System.Globalization;
using System.Text;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Opera
{
    /// <summary>
    /// Exposes the service provided by the native OperaDriver executable.
    /// </summary>
    public sealed class OperaDriverService : DriverService
    {
        private const string OperaDriverServiceFileName = "operadriver.exe";
        private static readonly Uri OperaDriverDownloadUrl = new Uri("https://github.com/operasoftware/operachromiumdriver/releases");
        private string logPath = string.Empty;
        private string urlPathPrefix = string.Empty;
        private string portServerAddress = string.Empty;
        private int adbPort = -1;
        private bool enableVerboseLogging;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperaDriverService"/> class.
        /// </summary>
        /// <param name="executablePath">The full path to the OperaDriver executable.</param>
        /// <param name="executableFileName">The file name of the OperaDriver executable.</param>
        /// <param name="port">The port on which the OperaDriver executable should listen.</param>
        private OperaDriverService(string executablePath, string executableFileName, int port)
            : base(executablePath, port, executableFileName, OperaDriverDownloadUrl)
        {
        }

        /// <summary>
        /// Gets or sets the location of the log file written to by the OperaDriver executable.
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
        /// Gets or sets a value indicating whether to enable verbose logging for the OperaDriver executable.
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public bool EnableVerboseLogging
        {
            get { return enableVerboseLogging; }
            set { enableVerboseLogging = value; }
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

                return argsBuilder.ToString();
            }
        }

        /// <summary>
        /// Creates a default instance of the OperaDriverService.
        /// </summary>
        /// <returns>A OperaDriverService that implements default settings.</returns>
        public static OperaDriverService CreateDefaultService()
        {
            var serviceDirectory = FindDriverServiceExecutable(OperaDriverServiceFileName, OperaDriverDownloadUrl);
            return CreateDefaultService(serviceDirectory);
        }

        /// <summary>
        /// Creates a default instance of the OperaDriverService using a specified path to the OperaDriver executable.
        /// </summary>
        /// <param name="driverPath">The directory containing the OperaDriver executable.</param>
        /// <returns>A OperaDriverService using a random port.</returns>
        public static OperaDriverService CreateDefaultService(string driverPath)
        {
            return CreateDefaultService(driverPath, OperaDriverServiceFileName);
        }

        /// <summary>
        /// Creates a default instance of the OperaDriverService using a specified path to the OperaDriver executable with the given name.
        /// </summary>
        /// <param name="driverPath">The directory containing the OperaDriver executable.</param>
        /// <param name="driverExecutableFileName">The name of the OperaDriver executable file.</param>
        /// <returns>A OperaDriverService using a random port.</returns>
        public static OperaDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
        {
            return new OperaDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
        }
    }
}
