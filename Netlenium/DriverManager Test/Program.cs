using System;
using Netlenium.Driver;

namespace DriverManager_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var WebDriver = new Netlenium.Driver.Chrome.Driver()
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
            while(true)
            {
                Console.ReadLine();
                foreach(IWindow window in WebDriver.Actions.GetWindows())
                {
                    Console.WriteLine(window.ID);
                    window.SwitchTo();
                }
            }
        }
    }
}
