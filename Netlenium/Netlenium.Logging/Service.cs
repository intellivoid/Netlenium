using System;
using System.Diagnostics;

namespace Netlenium.Logging
{
    /// <summary>
    /// Service Object for Logging
    /// </summary>
    public class Service
    {
        /// <summary>
        /// The name of the service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Option for if Information Type Entries are allowed to be logged
        /// </summary>
        public bool InformationEntriesEnabled { get; set; }

        /// <summary>
        /// Option for if Warning Type Entries are allowed to be logged
        /// </summary>
        public bool WarningEntriesEnabled { get; set; }

        /// <summary>
        /// Option for if Error Type Entries are allowed to be logged
        /// </summary>
        public bool ErrorEntriesEnabled { get; set; }

        /// <summary>
        /// Option for if Verbose Type Entries are allowed to be logged
        /// </summary>
        public bool VerboseEntriesEnabled { get; set; }

        /// <summary>
        /// Option for if Debugging Type Entries are allowed to be logged
        /// </summary>
        public bool DebuggingEntriesEnabled { get; set; }

        /// <summary>
        /// If Enabled, all entries will be printed to the CommandLine
        /// </summary>
        public bool CommandLineLoggingEnabled { get; set; }

        /// <summary>
        /// If enabled, all enteries will be printed out to the debugging menu
        /// </summary>
        public bool DebuggingOutputEnabled { get; set; }

        /// <summary>
        /// If enabled, all entried will be logged to a file
        /// </summary>
        public bool FileLoggingEnabled { get; set; }

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="moduleName"></param>
        /// <param name="message"></param>
        public void WriteEntry(MessageType messageType, string moduleName, string message)
        {
            switch(messageType)
            {
                case MessageType.Debugging:
                    if(DebuggingEntriesEnabled == false)
                    {
                        return;
                    }
                    break;

                case MessageType.Error:
                    if (ErrorEntriesEnabled == false)
                    {
                        return;
                    }
                    break;

                case MessageType.Information:
                    if(InformationEntriesEnabled == false)
                    {
                        return;
                    }
                    break;

                case MessageType.Verbose:
                    if(VerboseEntriesEnabled == false)
                    {
                        return;
                    }
                    break;

                case MessageType.Warning:
                    if (WarningEntriesEnabled == false)
                    {
                        return;
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }

            if(CommandLineLoggingEnabled)
            {
                CommandLine.PrintEntry(
                    messageType,
                    Name,
                    moduleName,
                    message
                );
            }

            if(FileLoggingEnabled)
            {
                FileLogging.WriteEntry(messageType,
                    Name,
                    moduleName,
                    message
                );
            }

            if(DebuggingOutputEnabled)
            {
                Debug.Print($"[{messageType.ToString()}] {Name} => {moduleName} :: {message}");
            }
        }
    }
}
