using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver.Chrome
{
    public class WebElement : IWebElement
    {
        private WebDriver.IWebElement webElement;

        private Driver driver;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="webElement"></param>
        public WebElement(Driver driver, WebDriver.IWebElement webElement)
        {
            this.webElement = webElement;
            this.driver = driver;
        }

        public void SendKeys(string input)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement.ToString()}]", $"Entering '{input}' to element");
            webElement.SendKeys(input);
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement.ToString()}]", "Operation completed");
        }
    }
}
