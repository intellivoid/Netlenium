using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Permissions;
using Netlenium.Driver.WebDriver.Internal;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Exposes the service provided by a native WebDriver server executable.
    /// </summary>
    public abstract class DriverService : ICommandServer
    {
        private string driverServicePath;
        private string driverServiceExecutableName;
        private string driverServiceHostName = "localhost";
        private int driverServicePort;
        private bool silent;
        private bool hideCommandPromptWindow;
        private bool isDisposed;
        private Process driverServiceProcess;
        private string commandLineArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverService"/> class.
        /// </summary>
        /// <param name="servicePath">The full path to the directory containing the executable providing the service to drive the browser.</param>
        /// <param name="port">The port on which the driver executable should listen.</param>
        /// <param name="driverServiceExecutableName">The file name of the driver service executable.</param>
        /// <param name="driverServiceDownloadUrl">A URL at which the driver service executable may be downloaded.</param>
        /// <exception cref="ArgumentException">
        /// If the path specified is <see langword="null"/> or an empty string.
        /// </exception>
        /// <exception cref="DriverServiceNotFoundException">
        /// If the specified driver service executable does not exist in the specified directory.
        /// </exception>
        protected DriverService(string servicePath, int port, string driverServiceExecutableName, Uri driverServiceDownloadUrl)
        {
            if (string.IsNullOrEmpty(servicePath))
            {
                throw new ArgumentException("Path to locate driver executable cannot be null or empty.", "servicePath");
            }

            var executablePath = Path.Combine(servicePath, driverServiceExecutableName);
            if (!File.Exists(executablePath))
            {
                throw new DriverServiceNotFoundException(string.Format(CultureInfo.InvariantCulture, "The file {0} does not exist. The driver can be downloaded at {1}", executablePath, driverServiceDownloadUrl));
            }

            driverServicePath = servicePath;
            this.driverServiceExecutableName = driverServiceExecutableName;
            driverServicePort = port;
            commandLineArguments = string.Format(CultureInfo.InvariantCulture, "--port={0}", driverServicePort);
        }

        /// <summary>
        /// Gets the Uri of the service.
        /// </summary>
        public Uri ServiceUrl
        {
            get { return new Uri(string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}", driverServiceHostName, driverServicePort)); }
        }

        /// <summary>
        /// Gets or sets the host name of the service. Defaults to "localhost."
        /// </summary>
        /// <remarks>
        /// Most driver service executables do not allow connections from remote
        /// (non-local) machines. This property can be used as a workaround so
        /// that an IP address (like "127.0.0.1" or "::1") can be used instead.
        /// </remarks>
        public string HostName
        {
            get { return driverServiceHostName; }
            set { driverServiceHostName = value; }
        }

        /// <summary>
        /// Gets or sets the port of the service.
        /// </summary>
        public int Port
        {
            get { return driverServicePort; }
            set { driverServicePort = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the initial diagnostic information is suppressed
        /// when starting the driver server executable. Defaults to <see langword="false"/>, meaning
        /// diagnostic information should be shown by the driver server executable.
        /// </summary>
        public bool SuppressInitialDiagnosticInformation
        {
            get { return silent; }
            set { silent = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the service is running.
        /// </summary>
        public bool IsRunning
        {
            [SecurityPermission(SecurityAction.Demand)]
            get { return driverServiceProcess != null && !driverServiceProcess.HasExited; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command prompt window of the service should be hidden.
        /// </summary>
        public bool HideCommandPromptWindow
        {
            get { return hideCommandPromptWindow; }
            set { hideCommandPromptWindow = value; }
        }

        /// <summary>
        /// Gets the process ID of the running driver service executable. Returns 0 if the process is not running.
        /// </summary>
        public int ProcessId
        {
            get
            {
                if (IsRunning)
                {
                    // There's a slight chance that the Process object is running,
                    // but does not have an ID set. This should be rare, but we
                    // definitely don't want to throw an exception.
                    try
                    {
                        return driverServiceProcess.Id;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the executable file name of the driver service.
        /// </summary>
        protected string DriverServiceExecutableName
        {
            get { return driverServiceExecutableName; }
        }

        /// <summary>
        /// Gets the command-line arguments for the driver service.
        /// </summary>
        protected virtual string CommandLineArguments
        {
            get
            {
                return commandLineArguments;
            }
            set
            {
                commandLineArguments = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the time to wait for an initial connection before timing out.
        /// </summary>
        protected virtual TimeSpan InitializationTimeout
        {
            get { return TimeSpan.FromSeconds(20); }
        }

        /// <summary>
        /// Gets a value indicating the time to wait for the service to terminate before forcing it to terminate.
        /// </summary>
        protected virtual TimeSpan TerminationTimeout
        {
            get { return TimeSpan.FromSeconds(10); }
        }

        /// <summary>
        /// Gets a value indicating whether the service has a shutdown API that can be called to terminate
        /// it gracefully before forcing a termination.
        /// </summary>
        protected virtual bool HasShutdown
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the service is responding to HTTP requests.
        /// </summary>
        protected virtual bool IsInitialized
        {
            get
            {
                var isInitialized = false;
                try
                {
                    var serviceHealthUri = new Uri(ServiceUrl, new Uri(DriverCommand.Status, UriKind.Relative));
                    var request = WebRequest.Create(serviceHealthUri) as HttpWebRequest;
                    request.KeepAlive = false;
                    request.Timeout = 5000;
                    var response = request.GetResponse() as HttpWebResponse;

                    // Checking the response from the 'status' end point. Note that we are simply checking
                    // that the HTTP status returned is a 200 status, and that the resposne has the correct
                    // Content-Type header. A more sophisticated check would parse the JSON response and
                    // validate its values. At the moment we do not do this more sophisticated check.
                    isInitialized = response.StatusCode == HttpStatusCode.OK && response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
                    response.Close();
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return isInitialized;
            }
        }

        /// <summary>
        /// Releases all resources associated with this <see cref="DriverService"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts the DriverService.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand)]
        public void Start()
        {
            driverServiceProcess = new Process();
            driverServiceProcess.StartInfo.FileName = Path.Combine(driverServicePath, driverServiceExecutableName);
            driverServiceProcess.StartInfo.Arguments = CommandLineArguments;
            driverServiceProcess.StartInfo.UseShellExecute = false;
            driverServiceProcess.StartInfo.CreateNoWindow = hideCommandPromptWindow;
            driverServiceProcess.Start();
            var serviceAvailable = WaitForServiceInitialization();

            if (!serviceAvailable)
            {
                var msg = "Cannot start the driver service on " + ServiceUrl;
                throw new WebDriverException(msg);
            }
        }

        /// <summary>
        /// Starts the DriverService with custom commandline arguments for the driver
        /// </summary>
        /// <param name="ExtraCommandlineArguments"></param>
        [SecurityPermission(SecurityAction.Demand)]
        public void Start(string ExtraCommandlineArguments)
        {
            driverServiceProcess = new Process();
            driverServiceProcess.StartInfo.FileName = Path.Combine(driverServicePath, driverServiceExecutableName);
            driverServiceProcess.StartInfo.Arguments = string.Format("{0} {1}", CommandLineArguments, ExtraCommandlineArguments);
            driverServiceProcess.StartInfo.UseShellExecute = false;
            driverServiceProcess.StartInfo.CreateNoWindow = hideCommandPromptWindow;
            driverServiceProcess.Start();
            var serviceAvailable = WaitForServiceInitialization();

            if (!serviceAvailable)
            {
                var msg = "Cannot start the driver service on " + ServiceUrl;
                throw new WebDriverException(msg);
            }
        }

        /// <summary>
        /// Finds the specified driver service executable.
        /// </summary>
        /// <param name="executableName">The file name of the executable to find.</param>
        /// <param name="downloadUrl">A URL at which the driver service executable may be downloaded.</param>
        /// <returns>The directory containing the driver service executable.</returns>
        /// <exception cref="DriverServiceNotFoundException">
        /// If the specified driver service executable does not exist in the current directory or in a directory on the system path.
        /// </exception>
        protected static string FindDriverServiceExecutable(string executableName, Uri downloadUrl)
        {
            var serviceDirectory = FileUtilities.FindFile(executableName);
            if (string.IsNullOrEmpty(serviceDirectory))
            {
                throw new DriverServiceNotFoundException(string.Format(CultureInfo.InvariantCulture, "The {0} file does not exist in the current directory or in a directory on the PATH environment variable. The driver can be downloaded at {1}.", executableName, downloadUrl));
            }

            return serviceDirectory;
        }

        /// <summary>
        /// Releases all resources associated with this <see cref="DriverService"/>.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the Dispose method was explicitly called; otherwise, <see langword="false"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Stop();
                }

                isDisposed = true;
            }
        }

        /// <summary>
        /// Stops the DriverService.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand)]
        private void Stop()
        {
            if (IsRunning)
            {
                if (HasShutdown)
                {
                    var shutdownUrl = new Uri(ServiceUrl, "/shutdown");
                    var timeout = DateTime.Now.Add(TerminationTimeout);
                    while (IsRunning && DateTime.Now < timeout)
                    {
                        try
                        {
                            // Issue the shutdown HTTP request, then wait a short while for
                            // the process to have exited. If the process hasn't yet exited,
                            // we'll retry. We wait for exit here, since catching the exception
                            // for a failed HTTP request due to a closed socket is particularly
                            // expensive.
                            var request = WebRequest.Create(shutdownUrl) as HttpWebRequest;
                            request.KeepAlive = false;
                            var response = request.GetResponse() as HttpWebResponse;
                            response.Close();
                            driverServiceProcess.WaitForExit(3000);
                        }
                        catch (WebException)
                        {
                        }
                    }
                }

                // If at this point, the process still hasn't exited, wait for one
                // last-ditch time, then, if it still hasn't exited, kill it. Note
                // that falling into this branch of code should be exceedingly rare.
                if (IsRunning)
                {
                    driverServiceProcess.WaitForExit(Convert.ToInt32(TerminationTimeout.TotalMilliseconds));
                    if (!driverServiceProcess.HasExited)
                    {
                        driverServiceProcess.Kill();
                    }
                }

                driverServiceProcess.Dispose();
                driverServiceProcess = null;
            }
        }

        /// <summary>
        /// Waits until a the service is initialized, or the timeout set
        /// by the <see cref="InitializationTimeout"/> property is reached.
        /// </summary>
        /// <returns><see langword="true"/> if the service is properly started and receiving HTTP requests;
        /// otherwise; <see langword="false"/>.</returns>
        private bool WaitForServiceInitialization()
        {
            var isInitialized = false;
            var timeout = DateTime.Now.Add(InitializationTimeout);
            while (!isInitialized && DateTime.Now < timeout)
            {
                // If the driver service process has exited, we can exit early.
                if (!IsRunning)
                {
                    break;
                }

                isInitialized = IsInitialized;
            }

            return isInitialized;
        }
    }
}
