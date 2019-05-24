using System;
using System.Collections.Generic;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Holds the information about all commands specified by the JSON wire protocol.
    /// This class cannot be inherited, as it is intended to be a singleton, and
    /// allowing subclasses introduces the possibility of multiple instances.
    /// </summary>
    public abstract class CommandInfoRepository
    {
        private readonly Dictionary<string, CommandInfo> commandDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInfoRepository"/> class.
        /// Protected accessibility prevents a default instance from being created.
        /// </summary>
        protected CommandInfoRepository()
        {
            commandDictionary = new Dictionary<string, CommandInfo>();
        }

        /// <summary>
        /// Gets the level of the W3C WebDriver specification that this repository supports.
        /// </summary>
        public abstract int SpecificationLevel { get; }

        /// <summary>
        /// Gets the <see cref="CommandInfo"/> for a <see cref="DriverCommand"/>.
        /// </summary>
        /// <param name="commandName">The <see cref="DriverCommand"/> for which to get the information.</param>
        /// <returns>The <see cref="CommandInfo"/> for the specified command.</returns>
        public CommandInfo GetCommandInfo(string commandName)
        {
            CommandInfo toReturn = null;
            if (commandDictionary.ContainsKey(commandName))
            {
                toReturn = commandDictionary[commandName];
            }

            return toReturn;
        }

        /// <summary>
        /// Tries to add a command to the list of known commands.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandInfo">The command information.</param>
        /// <returns><see langword="true"/> if the new command has been added successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is used by WebDriver implementations to add additional custom driver-specific commands.
        /// This method will not overwrite existing commands for a specific name, and will return <see langword="false"/>
        /// in that case.
        /// </remarks>
        public bool TryAddCommand(string commandName, CommandInfo commandInfo)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentNullException("commandName", "The name of the command cannot be null or the empty string.");
            }

            if (commandInfo == null)
            {
                throw new ArgumentNullException("commandInfo", "The command information object cannot be null.");
            }

            if (commandDictionary.ContainsKey(commandName))
            {
                return false;
            }

            commandDictionary.Add(commandName, commandInfo);
            return true;
        }

        /// <summary>
        /// Initializes the dictionary of commands for the CommandInfoRepository
        /// </summary>
        protected abstract void InitializeCommandDictionary();
    }
}
