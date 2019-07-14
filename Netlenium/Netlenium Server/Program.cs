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
            CommandLineParameters.MaxSessions = 100;
            CommandLineParameters.SessionInactivityLimit = 10;
            CommandLineParameters.AuthPassword = string.Empty;
            

            var p = new OptionSet() {

                { "h|help",  "Displays the help menu and exit",
                  v => CommandLineParameters.Help = v != null },

                { "disable-stdout",  "Disables standard output",
                  v => CommandLineParameters.DisabledStdout = v != null },

                { "driver-logging-level=", "Logging level for Driver Sessions",
                  v => { if (v != null) CommandLineParameters.DriverLoggingLevel = Convert.ToInt32(v); } },

                { "server-logging-level=", "Logging level for the Web Service",
                  v => { if (v != null) CommandLineParameters.ServerLoggingLevel = Convert.ToInt32(v); } },
                
                { "p|port=", "The port to run the Web Service on",
                  v => { if (v != null) CommandLineParameters.Port = Convert.ToInt32(v); } },

                { "max-sessions=", "The maximum amount of sessions that can be created",
                  v => { if (v != null) CommandLineParameters.MaxSessions = Convert.ToInt32(v); } },

                { "session-inactivity-limit=", "The amount of minutes that a session is allowed to be inactive",
                  v => { if (v != null) CommandLineParameters.SessionInactivityLimit = Convert.ToInt32(v); } },

                { "auth-password=", "Authentication Password for Web Service access",
                  v => { if (v != null) CommandLineParameters.AuthPassword = v; } },

            };
            p.Parse(args);

            if(CommandLineParameters.Help == true)
            {
                ShowHelp();
                Environment.Exit(0);
            }
            
            VerifyValues();

            WebService.logging.Name = "Netlenium.Server";

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

            try
            {
                WebService.Start(CommandLineParameters.Port);
            }
            catch(Exception)
            {
                Environment.Exit(1);
            }

            while (true)
            {
                Console.ReadKey(true);
            };

        }

        /// <summary>
        /// Verifies the values in the given arguments
        /// </summary>
        static void VerifyValues()
        {
            if (CommandLineParameters.MaxSessions < 1)
            {
                Console.WriteLine("The parameter 'max-sessions' cannot have a value that's less than 1");
                Environment.Exit(64);
            }

            if (CommandLineParameters.MaxSessions > 99999)
            {
                Console.WriteLine("The parameter 'max-sessions' cannot have a value that's greater than 99999");
                Environment.Exit(64);
            }

            if(CommandLineParameters.SessionInactivityLimit < 0)
            {
                Console.WriteLine("The parameter 'session-inactivity-limit' cannot have a value that's less than 0");
                Environment.Exit(64);
            }

            if (CommandLineParameters.SessionInactivityLimit > 99999)
            {
                Console.WriteLine("The parameter 'session-inactivity-limit' cannot have a value that's greater than 99999");
                Environment.Exit(64);
            }
            
            if(CommandLineParameters.AuthPassword != string.Empty)
            {
                if(CommandLineParameters.AuthPassword.Length < 6)
                {
                    Console.WriteLine("The parameter 'auth-password' is invalid, the password must be greater than 6 characters");
                    Environment.Exit(64);
                }
            }

            if(CommandLineParameters.DisabledStdout == true)
            {
                WebService.logging.CommandLineLoggingEnabled = false;
            }
            else
            {
                WebService.logging.CommandLineLoggingEnabled = true;
            }
        }

        /// <summary>
        /// Displays the Help Menu
        /// </summary>
        static void ShowHelp()
        {
            Console.WriteLine(" USAGE:");
            Console.WriteLine("     netlenium [OPTIONS]         (main server)");
            Console.WriteLine();
            Console.WriteLine(" OPTIONS:");
            Console.WriteLine("     -h, --help");
            Console.WriteLine("         Displays the help menu and exits");
            Console.WriteLine();
            Console.WriteLine("     --disable-stdout");
            Console.WriteLine("         Disables standard output");
            Console.WriteLine();
            Console.WriteLine("     --driver-logging-level [0-3]");
            Console.WriteLine("         Logging level for Driver Sessions");
            Console.WriteLine("         0 = Silent");
            Console.WriteLine("         1 = Information, Warning & Errors (Default)");
            Console.WriteLine("         2 = Verbose");
            Console.WriteLine("         3 = Debugging");
            Console.WriteLine();
            Console.WriteLine("     --server-logging-level [0-3]");
            Console.WriteLine("         Logging level for the Web Service");
            Console.WriteLine("         0 = Silent");
            Console.WriteLine("         1 = Information, Warning & Errors (Default)");
            Console.WriteLine("         2 = Verbose");
            Console.WriteLine("         3 = Debugging");
            Console.WriteLine();
            Console.WriteLine("     -p, --port [Default: 6410]");
            Console.WriteLine("         The port to run the Web Service on");
            Console.WriteLine();
            Console.WriteLine("     --max-sessions [Default: 100]");
            Console.WriteLine("         The max amount of sessions that are allowed to be created");
            Console.WriteLine();
            Console.WriteLine("     --session-inactivity-limit [Default: 10]");
            Console.WriteLine("         The amount of minutes that a session is allowed to be inactive");
            Console.WriteLine("         before it gets closed. Using 0 as the value will never close");
            Console.WriteLine("         inactive session");
            Console.WriteLine();
            Console.WriteLine("     --auth-password [Optional]");
            Console.WriteLine("         Authentication Password for Web Service access");
        }
    }
}
