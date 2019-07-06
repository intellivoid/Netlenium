using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netlenium.Driver;

namespace DriverManager_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var WebDriver = new Netlenium.Driver.Chrome.Driver();

            WebDriver.Logging.CommandLineLoggingEnabled = true;
            WebDriver.Logging.DebuggingEntriesEnabled = false;
            WebDriver.Logging.VerboseEntriesEnabled = false;
            WebDriver.Logging.InformationEntriesEnabled = true;
            WebDriver.Logging.WarningEntriesEnabled = true;
            WebDriver.Logging.ErrorEntriesEnabled = true;

            WebDriver.DriverManager.Initalize();

            Console.ReadLine();
        }
    }
}
