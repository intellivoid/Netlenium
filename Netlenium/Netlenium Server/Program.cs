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
                Port = 8080
            };

            var p = new OptionSet() {

                { "h|help",  "Indiciates if the Help menu should only be displayed",
                  v => CommandLineParameters.Help = v != null },

                { "driver-logging-level=", "The logging level for Netlenium Drivers",
                  v => { if (v != null) CommandLineParameters.DriverLoggingLevel = Convert.ToInt32(v); } },

                { "server-logging-level=", "The logging level for the Web Serivce",
                  v => { if (v != null) CommandLineParameters.ServerLoggingLevel = Convert.ToInt32(v); } },
                
                { "p|port=", "the number of times to repeat the greeting.",
                  v => { if (v != null) CommandLineParameters.Port = Convert.ToInt32(v); } },

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

        }
    }
}
