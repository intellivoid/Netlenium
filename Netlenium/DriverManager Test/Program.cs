using System;
using Netlenium.Driver;

namespace DriverManager_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IDriver WebDriver = new Netlenium.Driver.Chrome.Driver()
            {
                Headless = true,
                DriverLoggingEnabled = false,
                DriverVerboseLoggingEnabled = false
            };

            WebDriver.Logging.CommandLineLoggingEnabled = true;
            WebDriver.Logging.DebuggingEntriesEnabled = true;
            WebDriver.Logging.VerboseEntriesEnabled = true;
            WebDriver.Logging.InformationEntriesEnabled = true;
            WebDriver.Logging.WarningEntriesEnabled = true;
            WebDriver.Logging.ErrorEntriesEnabled = true;

            WebDriver.ProxyConfiguration.Enabled = false;
            WebDriver.Start();
            
            WebDriver.Actions.LoadURI("https://google.com");
            Console.ReadLine();
        }
    }
}
