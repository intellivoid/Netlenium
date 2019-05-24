using System;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a mechanism to execute commands on the browser
    /// </summary>
    public class DriverServiceCommandExecutor : ICommandExecutor
    {
        private DriverService service;
        private HttpCommandExecutor internalExecutor;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverServiceCommandExecutor"/> class.
        /// </summary>
        /// <param name="driverService">The <see cref="DriverService"/> that drives the browser.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout)
            : this(driverService, commandTimeout, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverServiceCommandExecutor"/> class.
        /// </summary>
        /// <param name="driverService">The <see cref="DriverService"/> that drives the browser.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        /// <param name="enableKeepAlive"><see langword="true"/> if the KeepAlive header should be sent
        /// with HTTP requests; otherwise, <see langword="false"/>.</param>
        public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout, bool enableKeepAlive)
        {
            service = driverService;
            internalExecutor = new HttpCommandExecutor(driverService.ServiceUrl, commandTimeout, enableKeepAlive);
        }

        /// <summary>
        /// Gets the <see cref="CommandInfoRepository"/> object associated with this executor.
        /// </summary>
        public CommandInfoRepository CommandInfoRepository
        {
            get { return internalExecutor.CommandInfoRepository; }
        }

        /// <summary>
        /// Gets the <see cref="HttpCommandExecutor"/> that sends commands to the remote
        /// end WebDriver implementation.
        /// </summary>
        public HttpCommandExecutor HttpExecutor
        {
            get { return internalExecutor; }
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
                throw new ArgumentNullException("commandToExecute", "Command to execute cannot be null");
            }

            Response toReturn = null;
            if (commandToExecute.Name == DriverCommand.NewSession)
            {
                service.Start();
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
        /// Releases all resources used by the <see cref="DriverServiceCommandExecutor"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HttpCommandExecutor"/> and
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
                    service.Dispose();
                }

                isDisposed = true;
            }
        }
    }
}
