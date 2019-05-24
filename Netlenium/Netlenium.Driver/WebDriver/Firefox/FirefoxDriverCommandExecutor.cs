using System;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Provides a way of executing Commands using the FirefoxDriver.
    /// </summary>
    public class FirefoxDriverCommandExecutor : ICommandExecutor, IDisposable
    {
        private FirefoxDriverServer server;
        private HttpCommandExecutor internalExecutor;
        private TimeSpan commandTimeout;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriverCommandExecutor"/> class.
        /// </summary>
        /// <param name="binary">The <see cref="FirefoxBinary"/> on which to make the connection.</param>
        /// <param name="profile">The <see cref="FirefoxProfile"/> creating the connection.</param>
        /// <param name="host">The name of the host on which to connect to the Firefox extension (usually "localhost").</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public FirefoxDriverCommandExecutor(FirefoxBinary binary, FirefoxProfile profile, string host, TimeSpan commandTimeout)
        {
            server = new FirefoxDriverServer(binary, profile, host);
            this.commandTimeout = commandTimeout;
        }

        /// <summary>
        /// Gets the repository of objects containin information about commands.
        /// </summary>
        public CommandInfoRepository CommandInfoRepository
        {
            get { return internalExecutor.CommandInfoRepository; }
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="commandToExecute">The command you wish to execute</param>
        /// <returns>A response from the browser</returns>
        public Response Execute(Command commandToExecute)
        {
            if (commandToExecute == null)
            {
                throw new ArgumentNullException("commandToExecute", "Command may not be null");
            }

            Response toReturn = null;
            if (commandToExecute.Name == DriverCommand.NewSession)
            {
                server.Start();
                internalExecutor = new HttpCommandExecutor(server.ExtensionUri, commandTimeout);
            }

            // Use a try-catch block to catch exceptions for the Quit
            // command, so that we can get the finally block.
            try
            {
                toReturn = internalExecutor.Execute(commandToExecute);
            }
            finally
            {
                if (commandToExecute.Name == DriverCommand.Quit)
                {
                    Dispose();
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="FirefoxDriverCommandExecutor"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FirefoxDriverCommandExecutor"/> and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release managed and resources;
        /// <see langword="false"/> to only release unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    server.Dispose();
                }

                isDisposed = true;
            }
        }
    }
}
