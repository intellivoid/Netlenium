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
            var WebDriver = new Netlenium.Driver.Chrome.Driver()
            {
                Headless = false,
                DriverLoggingEnabled = false,
                DriverVerboseLoggingEnabled = false
            };

            WebDriver.Logging.CommandLineLoggingEnabled = false;
            WebDriver.Logging.DebuggingEntriesEnabled = true;
            WebDriver.Logging.VerboseEntriesEnabled = true;
            WebDriver.Logging.InformationEntriesEnabled = true;
            WebDriver.Logging.WarningEntriesEnabled = true;
            WebDriver.Logging.ErrorEntriesEnabled = true;

            WebDriver.Start();
            WebDriver.Actions.LoadURI("https://google.com");
            WebDriver.Document.GetElements(SearchBy.XPath, "//*[@id=\"tsf\"]/div[2]/div/div[1]/div/div[1]/input")[0].SendKeys("Hello World");
            Console.WriteLine(WebDriver.Actions.ExecuteJavascript("return document.title;"));

            Console.ReadLine();
        }
    }
}
