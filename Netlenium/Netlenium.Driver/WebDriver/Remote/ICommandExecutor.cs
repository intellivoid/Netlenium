using System;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a way to send commands to the remote server
    /// </summary>
    public interface ICommandExecutor : IDisposable
    {
        /// <summary>
        /// Gets the repository of objects containin information about commands.
        /// </summary>
        CommandInfoRepository CommandInfoRepository { get; }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="commandToExecute">The command you wish to execute</param>
        /// <returns>A response from the browser</returns>
        Response Execute(Command commandToExecute);
    }
}
