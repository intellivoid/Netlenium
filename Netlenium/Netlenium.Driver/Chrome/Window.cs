using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver.Chrome
{
    public class Window : IWindow
    {
        private string id;
        public string ID => id;

        private Driver driver;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="id"></param>
        public Window(Driver driver, string id)
        {
            this.id = id;
            this.driver = driver;
        }

        public void SwitchTo()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"Window[{this.id}]", "Switching to this window handle");
            try
            {
                driver.RemoteDriver.SwitchTo().Window(id);
            }
            catch(WebDriver.NoSuchWindowException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"Window[{this.id}]", "Cannot switch to this window handle, no such window exists.");
                throw new NoSuchWindowException();
            }
            catch (Exception exception)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"Window[{this.id}]", $"Cannot switch to this window handle, {exception.Message}");
                throw exception;
            }
        }
    }
}
