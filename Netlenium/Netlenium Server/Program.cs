using NetleniumServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium_Server
{
    class Program
    {
        static void Main(string[] args)
        {
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

            Console.WriteLine(WebService.Start(8080));

            Console.ReadLine();
            Environment.Exit(0);

        }
    }
}
