using System;

namespace Netlenium.Logging
{
    /// <summary>
    /// CommandLine Utilities
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Prints a log entry out to the commandline
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="serviceName"></param>
        /// <param name="moduleName"></param>
        /// <param name="entry"></param>
        public static void PrintEntry(MessageType messageType, string serviceName, string moduleName, string entry)
        {
            switch (messageType)
            {
                case MessageType.Information:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("[ --- ]");
                    break;

                case MessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[  !  ]");
                    break;

                case MessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[  X  ]");
                    break;

                case MessageType.Verbose:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("[ === ]");
                    break;

                case MessageType.Debugging:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("[ ### ]");
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("[ *** ]");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(string.Format("[ {0} ]", DateTime.Now.ToString("hh:mm:ss tt")));

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(string.Format(" {0}.{1}", serviceName, moduleName));

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(string.Format(" => {0}", entry));
            Console.WriteLine();
        }
    }
}
