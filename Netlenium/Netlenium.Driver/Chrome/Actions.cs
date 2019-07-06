using System;

namespace Netlenium.Driver.Chrome
{
    public class Actions : IActions
    {
        private Driver driver;

        public Actions(Driver driver)
        {
            this.driver = driver;
        }

        public void GoBack()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Actions", "Navigating back one page in the history");
            driver.RemoteDriver.Navigate().Back();
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Actions", "Navigation completed");
        }

        public void GoForward()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Actions", "Navigating forward one page in the history");
            driver.RemoteDriver.Navigate().Forward();
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Actions", "Navigation completed");
        }

        public void LoadURI(Uri location)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Actions", $"Navigating to '{location.ToString()}'");
            driver.RemoteDriver.Navigate().GoToUrl(location.ToString());
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Actions", "Navigation completed");
        }

        public void LoadURI(string location)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Actions", $"Navigating to '{location}'");
            driver.RemoteDriver.Navigate().GoToUrl(location);
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Actions", "Navigation completed");
        }

        public void Reload()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Actions", "Reloading the current document");
            driver.RemoteDriver.Navigate().Refresh();
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Actions", "Navigation completed");
        }
    }
}
