using System;
using System.Collections.Generic;
using System.Drawing;
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

        public string Text => this.webElement.Text;

        public Size Size => this.webElement.Size;

        public Point Location => this.webElement.Location;

        public void Click()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement.ToString()}]", "Simulating Click on Event");
            webElement.Click();
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement.ToString()}]", "Operation completed");
        }

        public string GetAttribute(string attributeName)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement.ToString()}]", $"Getting value of attribute '{attributeName}'");
            var value = webElement.GetAttribute(attributeName);
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, $"IWebElement[{webElement.ToString()}]", $"Returned value '{attributeName}'");
            return value;
        }

        public void MoveTo()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement.ToString()}]", "Moving to Element");
            new WebDriver.Interactions.Actions(driver.RemoteDriver).MoveToElement(webElement);
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement.ToString()}]", "Operation completed");
        }

        public void SendKeys(string input)
        {
            MoveTo();
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement.ToString()}]", $"Entering '{input}' to element");
            webElement.SendKeys(input);
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement.ToString()}]", "Operation completed");
        }

        public void SetAttribute(string attributeName, string value)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement.ToString()}]", $"Setting value of attribute '{attributeName}' to ''{value}");
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, $"IWebElement[{webElement.ToString()}]", $"Executing eval; arguments[0].setAttribute(arguments[{attributeName}], arguments[{value}]);");
            driver.RemoteDriver.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", webElement, attributeName, value);
        }
    }
}
