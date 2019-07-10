using Mono.Options;
using NetleniumServer;
using System;

namespace Netlenium_Server
{

    /// <summary>
    /// Main Program Class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main Execution Pointer
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Parse the command-line arguments

            CommandLineParameters.Help = false;
            CommandLineParameters.DriverLoggingLevel = 1;
            CommandLineParameters.ServerLoggingLevel = 1;
            CommandLineParameters.Port = 6410;
            CommandLineParameters.MaxSessions = 0;
            CommandLineParameters.AuthPassword = string.Empty;
            

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

                { "auth-password=", "Authentication Password for Web Service access",
                  v => { if (v != null) CommandLineParameters.AuthPassword = v; } },

            };
            p.Parse(args);

            if(CommandLineParameters.Help == true)
            {
                ShowHelp();
                Environment.Exit(0);
            }
            
            WebService.logging.Name = "Netlenium.Server";
            WebService.logging.CommandLineLoggingEnabled = true;

            switch (CommandLineParameters.ServerLoggingLevel)
            {
                case 0:
                    WebService.logging.InformationEntriesEnabled = false;
                    WebService.logging.WarningEntriesEnabled = false;
                    WebService.logging.ErrorEntriesEnabled = false;
                    WebService.logging.VerboseEntriesEnabled = false;
                    WebService.logging.DebuggingEntriesEnabled = false;
                    break;

                case 1:
                    WebService.logging.InformationEntriesEnabled = true;
                    WebService.logging.WarningEntriesEnabled = true;
                    WebService.logging.ErrorEntriesEnabled = true;
                    WebService.logging.VerboseEntriesEnabled = false;
                    WebService.logging.DebuggingEntriesEnabled = false;
                    break;

                case 2:
                    WebService.logging.InformationEntriesEnabled = true;
                    WebService.logging.WarningEntriesEnabled = true;
                    WebService.logging.ErrorEntriesEnabled = true;
                    WebService.logging.VerboseEntriesEnabled = true;
                    WebService.logging.DebuggingEntriesEnabled = false;
                    break;

                case 3:
                    WebService.logging.InformationEntriesEnabled = true;
                    WebService.logging.WarningEntriesEnabled = true;
                    WebService.logging.ErrorEntriesEnabled = true;
                    WebService.logging.VerboseEntriesEnabled = true;
                    WebService.logging.DebuggingEntriesEnabled = true;
                    break;

                default:
                    Console.WriteLine("The parameter 'server-logging-level' must have a value between 0-3");
                    Environment.Exit(1);
                    break;
            }

            if(CommandLineParameters.DriverLoggingLevel < 0)
            {
                Console.WriteLine("The parameter 'driver-logging-level' must have a value between 0-3");
                Environment.Exit(1);
            }

            if(CommandLineParameters.DriverLoggingLevel > 3)
            {
                Console.WriteLine("The parameter 'driver-logging-level' must have a value between 0-3");
                Environment.Exit(1);
            }

            Console.Title = "Netlenium Server";
            Console.WriteLine("Written by Zi Xing Narrakas");
            Console.WriteLine();

            WebService.Start(CommandLineParameters.Port);

            Console.ReadLine();
            Environment.Exit(0);

        }

        /// <summary>
        /// Displays the Help Menu
        /// </summary>
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
            Console.WriteLine("     -p, --port [Default: 6410]    The port to run the Web Service on");
            Console.WriteLine();
            Console.WriteLine("     --max-sessions [Optional]     The max amount of sessions that are");
            Console.WriteLine("                                   allowed to be created");
            Console.WriteLine();
            Console.WriteLine("     --auth-password [Optional]    Authentication Password for Web Service access");
        }
    }
}
