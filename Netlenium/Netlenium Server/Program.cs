using Mono.Options;
using NetleniumServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium_Server
{
    /// <summary>
    /// The Parameters used for this Application
    /// </summary>
    internal class CommandLineParameters
    {
        /// <summary>
        /// Indiciates if the Help menu should only be displayed
        /// </summary>
        public bool Help { get; set; }

        /// <summary>
        /// The logging level for Netlenium Drivers
        /// </summary>
        public int DriverLoggingLevel { get; set; }

        /// <summary>
        /// The logging level for the Web Serivce
        /// </summary>
        public int ServerLoggingLevel { get; set; }

        /// <summary>
        /// The port that the Web Service will run on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The maximum amount of sessions that can be created
        /// </summary>
        public int MaxSessions { get; set; }

        /// <summary>
        /// Access Password for Web Service access
        /// </summary>
        public string AccessPassword { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Parse the command-line arguments
            var CommandLineParameters = new CommandLineParameters()
            {
                Help = false,
                DriverLoggingLevel = 1,
                ServerLoggingLevel = 1,
                Port = 8080,
                MaxSessions = 0,
                AccessPassword = string.Empty
            };

            var p = new OptionSet() {

                { "h|help",  "Indiciates if the Help menu should only be displayed",
                  v => CommandLineParameters.Help = v != null },

                { "driver-logging-level=", "The logging level for Netlenium Drivers",
                  v => { if (v != null) CommandLineParameters.DriverLoggingLevel = Convert.ToInt32(v); } },

                { "server-logging-level=", "The logging level for the Web Serivce",
                  v => { if (v != null) CommandLineParameters.ServerLoggingLevel = Convert.ToInt32(v); } },
                
                { "p|port=", "The port that the Web Service will run on",
                  v => { if (v != null) CommandLineParameters.Port = Convert.ToInt32(v); } },

                { "max-sessions=", "The maximum amount of sessions that can be created",
                  v => { if (v != null) CommandLineParameters.MaxSessions = Convert.ToInt32(v); } },

                { "access-password=", "Access Password for Web Service access",
                  v => { if (v != null) CommandLineParameters.AccessPassword = v; } },

            };
            p.Parse(args);

            if(CommandLineParameters.Help == true)
            {
                ShowHelp();
                Environment.Exit(0);
            }

            Console.Title = "Netlenium Server";
            Console.WriteLine("Written by Zi Xing Narrakas");
            Console.WriteLine();

            WebService.logging.Name = "Netlenium.Server";
            WebService.logging.CommandLineLoggingEnabled = true;
            WebService.logging.DebuggingEntriesEnabled = true;
            WebService.logging.ErrorEntriesEnabled = true;
            WebService.logging.InformationEntriesEnabled = true;
            WebService.logging.VerboseEntriesEnabled = true;
            WebService.logging.WarningEntriesEnabled = true;

            WebService.Start(CommandLineParameters.Port);
            
            Console.ReadLine();
            Environment.Exit(0);

        }

        static void ShowHelp()
        {
            Console.WriteLine("usage: netlenium [options]");
            Console.WriteLine(" options:");
            Console.WriteLine("     -h, --help                      Displays the help menu and exits");
            Console.WriteLine();
            Console.WriteLine("     --driver-logging-level [0-3]    Logging level for Driver Sessions");
            Console.WriteLine("         0 = Silent");
            Console.WriteLine("         1 = Information, Warning & Errors (Default)");
            Console.WriteLine("         2 = Verbose");
            Console.WriteLine("         3 = Debugging");
            Console.WriteLine();
            Console.WriteLine("     --server-logging-level [0-3]    Logging level for the Web Service");
            Console.WriteLine("         0 = Silent");
            Console.WriteLine("         1 = Information, Warning & Errors (Default)");
            Console.WriteLine("         2 = Verbose");
            Console.WriteLine("         3 = Debugging");
            Console.WriteLine();
            Console.WriteLine("     -p, --port [Default: 8080]    The port to run the Web Service on");
            Console.WriteLine();
            Console.WriteLine("     --max-sessions [Optional]     The max amount of sessions that are");
            Console.WriteLine("                                   allowed to be created");
            Console.WriteLine();
            Console.WriteLine("     --access-password [Optional]  Access Password for Web Service access");
        }
    }
}
