using System;
using System.Globalization;
using System.Text;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.IE
{
    /// <summary>
    /// Exposes the service provided by the native IEDriverServer executable.
    /// </summary>
    public sealed class InternetExplorerDriverService : DriverService
    {
        private const string InternetExplorerDriverServiceFileName = "IEDriverServer.exe";
        private static readonly Uri InternetExplorerDriverDownloadUrl = new Uri("http://selenium-release.storage.googleapis.com/index.html");

        private InternetExplorerDriverLogLevel loggingLevel = InternetExplorerDriverLogLevel.Fatal;
        private string host = string.Empty;
        private string logFile = string.Empty;
        private string libraryExtractionPath = string.Empty;
        private string whitelistedIpAddresses = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriverService"/> class.
        /// </summary>
        /// <param name="executablePath">The full path to the IEDriverServer executable.</param>
        /// <param name="executableFileName">The file name of the IEDriverServer executable.</param>
        /// <param name="port">The port on which the IEDriverServer executable should listen.</param>
        private InternetExplorerDriverService(string executablePath, string executableFileName, int port)
            : base(executablePath, port, executableFileName, InternetExplorerDriverDownloadUrl)
        {
        }

        /// <summary>
        /// Gets or sets the value of the host adapter on which the IEDriverServer should listen for connections.
        /// </summary>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// Gets or sets the location of the log file written to by the IEDriverServer.
        /// </summary>
        public string LogFile
        {
            get { return logFile; }
            set { logFile = value; }
        }

        /// <summary>
        /// Gets or sets the logging level used by the IEDriverServer.
        /// </summary>
        public InternetExplorerDriverLogLevel LoggingLevel
        {
            get { return loggingLevel; }
            set { loggingLevel = value; }
        }

        /// <summary>
        /// Gets or sets the path to which the supporting library of the IEDriverServer.exe is extracted.
        /// Defaults to the temp directory if this property is not set.
        /// </summary>
        /// <remarks>
        /// The IEDriverServer.exe requires extraction of a supporting library to perform some of its functions. Setting
        /// This library is extracted to the temp directory if this property is not set. If the property is set, it must
        /// be set to a valid directory.
        /// </remarks>
        public string LibraryExtractionPath
        {
            get { return libraryExtractionPath; }
            set { libraryExtractionPath = value; }
        }

        /// <summary>
        /// Gets or sets the comma-delimited list of IP addresses that are approved to
        /// connect to this instance of the IEDriverServer. Defaults to an empty string,
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
                if (!string.IsNullOrEmpty(host))
                {
                    argsBuilder.Append(string.Format(CultureInfo.InvariantCulture, " -host={0}", host));
                }

                if (!string.IsNullOrEmpty(logFile))
                {
                    argsBuilder.Append(string.Format(CultureInfo.InvariantCulture, " -log-file={0}", logFile));
                }

                if (!string.IsNullOrEmpty(libraryExtractionPath))
                {
                    argsBuilder.Append(string.Format(CultureInfo.InvariantCulture, " -extract-path={0}", libraryExtractionPath));
                }

                if (loggingLevel != InternetExplorerDriverLogLevel.Fatal)
                {
                    argsBuilder.Append(string.Format(CultureInfo.InvariantCulture, " -log-level={0}", loggingLevel.ToString().ToUpperInvariant()));
                }

                if (!string.IsNullOrEmpty(whitelistedIpAddresses))
                {
                    argsBuilder.Append(string.Format(CultureInfo.InvariantCulture, " -whitelisted-ips={0}", whitelistedIpAddresses));
                }

                if (SuppressInitialDiagnosticInformation)
                {
                    argsBuilder.Append(" -silent");
                }

                return argsBuilder.ToString();
            }
        }

        /// <summary>
        /// Creates a default instance of the InternetExplorerDriverService.
        /// </summary>
        /// <returns>A InternetExplorerDriverService that implements default settings.</returns>
        public static InternetExplorerDriverService CreateDefaultService()
        {
            var serviceDirectory = FindDriverServiceExecutable(InternetExplorerDriverServiceFileName, InternetExplorerDriverDownloadUrl);
            return CreateDefaultService(serviceDirectory);
        }

        /// <summary>
        /// Creates a default instance of the InternetExplorerDriverService using a specified path to the IEDriverServer executable.
        /// </summary>
        /// <param name="driverPath">The directory containing the IEDriverServer executable.</param>
        /// <returns>A InternetExplorerDriverService using a random port.</returns>
        public static InternetExplorerDriverService CreateDefaultService(string driverPath)
        {
            return CreateDefaultService(driverPath, InternetExplorerDriverServiceFileName);
        }

        /// <summary>
        /// Creates a default instance of the InternetExplorerDriverService using a specified path to the IEDriverServer executable with the given name.
        /// </summary>
        /// <param name="driverPath">The directory containing the IEDriverServer executable.</param>
        /// <param name="driverExecutableFileName">The name of the IEDriverServer executable file.</param>
        /// <returns>A InternetExplorerDriverService using a random port.</returns>
        public static InternetExplorerDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
        {
            return new InternetExplorerDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
        }
    }
}
