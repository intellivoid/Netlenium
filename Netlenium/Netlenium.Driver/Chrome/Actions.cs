using Netlenium.Driver.WebDriver;
using System;
using System.Collections.Generic;

namespace Netlenium.Driver.Chrome
{
    public class Actions : IActions
    {
        private Driver driver;
        private IJavaScriptExecutor javaScriptExecutor;

        public Actions(Driver driver)
        {
            this.driver = driver;
        }

        public IWindow CurrentWindow => new Window(driver, driver.RemoteDriver.CurrentWindowHandle);

        public void Close()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Actions", $"Closing current window handle '{CurrentWindow}'");
            try
            {
                driver.RemoteDriver.Close();
            }
            catch(Exception ex)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Actions", $"Cannot close window handle, {ex.Message}");
                throw new WindowHandleException();
            }

            var CurrentWindows = GetWindows();
            if(CurrentWindows.Count > 0)
            {
                CurrentWindows[0].SwitchTo();
                return;
            }

            driver.Logging.WriteEntry(Logging.MessageType.Warning, "Actions", "There are no available window handles to switch to");
        }

        public string ExecuteJavascript(string code)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Actions", $"Executing JS '{code}'");
            try
            {
                javaScriptExecutor = (IJavaScriptExecutor)driver.RemoteDriver;
                var results = (string)javaScriptExecutor.ExecuteScript(code);
                driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Actions", $"Returned '{results}'");
                return results;
            }
            catch(Exception ex)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Actions", $"Execution Error '{ex.Message}'");
                throw new JavascriptExecutionException(ex.Message);
            }
        }

        public IWindow GetWindow(string Id)
        {
            var WindowHandles = driver.RemoteDriver.WindowHandles;
            if(WindowHandles.Contains(Id) == false)
            {
                throw new NoSuchWindowException();
            }

            return new Window(driver, WindowHandles[WindowHandles.IndexOf(Id)]);
        }

        public List<IWindow> GetWindows()
        {
            List<IWindow> results = new List<IWindow>();
            foreach (var WindowHandle in driver.RemoteDriver.WindowHandles)
            {
                results.Add(new Window(driver, WindowHandle));
            }
            return results;
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
