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
        private readonly WebDriver.IWebElement webElement;

        private readonly Driver driver;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="webElement"></param>
        public WebElement(Driver driver, WebDriver.IWebElement webElement)
        {
            this.webElement = webElement;
            this.driver = driver;
        }

        public string Text => webElement.Text;

        public Size Size => webElement.Size;

        public Point Location => webElement.Location;

        public bool IsSelected => webElement.Selected;

        public string TagName => webElement.TagName;

        public bool Enabled => webElement.Enabled;

        public string InnerHTML => webElement.GetAttribute("innerHTML");

        public void Clear()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement}]", "Clearing contents on Event");
            webElement.Clear();
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement}]", "Operation completed");
        }

        public void Click()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement}]", "Simulating Click on Event");
            try
            {
                webElement.Click();
            }
            catch (WebDriver.ElementNotVisibleException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is not visible");
                throw new ElementNotVisibleException("The WebElement is not visible");
            }
            catch (WebDriver.ElementNotInteractableException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is not interactable");
                throw new ElementNotInteractableException("The WebElement is not interactable");
            }
            catch (WebDriver.ElementNotSelectableException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is not selectable");
                throw new ElementNotSelectableException("The WebElement is not selectable");
            }
            catch (WebDriver.StaleElementReferenceException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is no longer valid in the document DOM");
                throw new StaleElementReferenceException("The WebElement is no longer valid in the document DOM");
            }
            catch (Exception ex)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", ex.Message);
                throw new DriverException(ex.Message);
            }
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement}]", "Operation completed");
        }
        
        public string GetAttribute(string attributeName)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement}]", $"Getting value of attribute '{attributeName}'");
            var value = webElement.GetAttribute(attributeName);
            if(value == null)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", $"The attribute '{attributeName}' was not found in the element");
                throw new AttributeNotFoundException("The given attribute name was not found in the element");
            }
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, $"IWebElement[{webElement}]", $"Returned value '{attributeName}'");
            return value;
        }

        public void MoveTo()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement}]", "Moving to Element");
            new WebDriver.Interactions.Actions(driver.RemoteDriver).MoveToElement(webElement);
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement}]", "Operation completed");
        }

        public void SendKeys(string input)
        {
            MoveTo();
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement}]", $"Entering '{input}' to element");
            try
            {
                webElement.SendKeys(input);
                driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement}]", "Operation completed");
            }
            catch (WebDriver.ElementNotVisibleException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is not visible");
                throw new ElementNotVisibleException("The WebElement is not visible");
            }
            catch (WebDriver.ElementNotInteractableException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is not interactable");
                throw new ElementNotInteractableException("The WebElement is not interactable");
            }
            catch(WebDriver.ElementNotSelectableException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is not selectable");
                throw new ElementNotSelectableException("The WebElement is not selectable");
            }
            catch(WebDriver.StaleElementReferenceException)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", "The WebElement is no longer valid in the document DOM");
                throw new StaleElementReferenceException("The WebElement is no longer valid in the document DOM");
            }
            catch(Exception ex)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, $"IWebElement[{webElement}]", ex.Message);
                throw new DriverException(ex.Message);
            }
        }

        public void SetAttribute(string attributeName, string value)
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement}]", $"Setting value of attribute '{attributeName}' to ''{value}");
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, $"IWebElement[{webElement}]", $"Executing eval; arguments[0].setAttribute(arguments[{attributeName}], arguments[{value}]);");
            driver.RemoteDriver.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", webElement, attributeName, value);
        }

        public void Submit()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, $"IWebElement[{webElement}]", "Submitting to Web Server");
            webElement.Submit();
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, $"IWebElement[{webElement}]", "Operation completed");
        }
    }
}
