using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a mechanism for finding elements on the page with locators.
    /// </summary>
    internal class RemoteTargetLocator : ITargetLocator
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetLocator"/> class
        /// </summary>
        /// <param name="driver">The driver that is currently in use</param>
        public RemoteTargetLocator(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Move to a different frame using its index
        /// </summary>
        /// <param name="frameIndex">The index of the </param>
        /// <returns>A WebDriver instance that is currently in use</returns>
        public IWebDriver Frame(int frameIndex)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("id", frameIndex);
            driver.InternalExecute(DriverCommand.SwitchToFrame, parameters);
            return driver;
        }

        /// <summary>
        /// Move to different frame using its name
        /// </summary>
        /// <param name="frameName">name of the frame</param>
        /// <returns>A WebDriver instance that is currently in use</returns>
        public IWebDriver Frame(string frameName)
        {
            if (frameName == null)
            {
                throw new ArgumentNullException("frameName", "Frame name cannot be null");
            }

            var name = Regex.Replace(frameName, @"(['""\\#.:;,!?+<>=~*^$|%&@`{}\-/\[\]\(\)])", @"\$1");
            var frameElements = driver.FindElements(By.CssSelector("frame[name='" + name + "'],iframe[name='" + name + "']"));
            if (frameElements.Count == 0)
            {
                frameElements = driver.FindElements(By.CssSelector("frame#" + name + ",iframe#" + name));
                if (frameElements.Count == 0)
                {
                    throw new NoSuchFrameException("No frame element found with name or id " + frameName);
                }
            }

            return Frame(frameElements[0]);
        }

        /// <summary>
        /// Move to a frame element.
        /// </summary>
        /// <param name="frameElement">a previously found FRAME or IFRAME element.</param>
        /// <returns>A WebDriver instance that is currently in use.</returns>
        public IWebDriver Frame(IWebElement frameElement)
        {
            if (frameElement == null)
            {
                throw new ArgumentNullException("frameElement", "Frame element cannot be null");
            }

            var elementReference = frameElement as IWebElementReference;
            if (elementReference == null)
            {
                var elementWrapper = frameElement as IWrapsElement;
                if (elementWrapper != null)
                {
                    elementReference = elementWrapper.WrappedElement as IWebElementReference;
                }
            }

            if (elementReference == null)
            {
                throw new ArgumentException("frameElement cannot be converted to IWebElementReference", "frameElement");
            }

            // TODO: Remove "ELEMENT" addition when all remote ends are spec-compliant.
            var elementDictionary = elementReference.ToDictionary();
            elementDictionary.Add("ELEMENT", elementReference.ElementReferenceId);

            var parameters = new Dictionary<string, object>();
            parameters.Add("id", elementDictionary);
            driver.InternalExecute(DriverCommand.SwitchToFrame, parameters);
            return driver;
        }

        /// <summary>
        /// Select the parent frame of the currently selected frame.
        /// </summary>
        /// <returns>An <see cref="IWebDriver"/> instance focused on the specified frame.</returns>
        public IWebDriver ParentFrame()
        {
            var parameters = new Dictionary<string, object>();
            driver.InternalExecute(DriverCommand.SwitchToParentFrame, parameters);
            return driver;
        }

        /// <summary>
        /// Change to the Window by passing in the name
        /// </summary>
        /// <param name="windowHandleOrName">Window handle or name of the window that you wish to move to</param>
        /// <returns>A WebDriver instance that is currently in use</returns>
        public IWebDriver Window(string windowHandleOrName)
        {
            var parameters = new Dictionary<string, object>();
            if (driver.IsSpecificationCompliant)
            {
                parameters.Add("handle", windowHandleOrName);
                try
                {
                    driver.InternalExecute(DriverCommand.SwitchToWindow, parameters);
                    return driver;
                }
                catch (NoSuchWindowException)
                {
                    // simulate search by name
                    string original = null;
                    try
                    {
                        original = driver.CurrentWindowHandle;
                    }
                    catch (NoSuchWindowException)
                    {
                    }

                    foreach (var handle in driver.WindowHandles)
                    {
                        Window(handle);
                        if (windowHandleOrName == driver.ExecuteScript("return window.name").ToString())
                        {
                            return driver; // found by name
                        }
                    }

                    if (original != null)
                    {
                        Window(original);
                    }

                    throw;
                }
            }
            else
            {
                parameters.Add("name", windowHandleOrName);
            }

            driver.InternalExecute(DriverCommand.SwitchToWindow, parameters);
            return driver;
        }

        /// <summary>
        /// Change the active frame to the default
        /// </summary>
        /// <returns>Element of the default</returns>
        public IWebDriver DefaultContent()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("id", null);
            driver.InternalExecute(DriverCommand.SwitchToFrame, parameters);
            return driver;
        }

        /// <summary>
        /// Finds the active element on the page and returns it
        /// </summary>
        /// <returns>Element that is active</returns>
        public IWebElement ActiveElement()
        {
            var response = driver.InternalExecute(DriverCommand.GetActiveElement, null);
            return driver.GetElementFromResponse(response);
        }

        /// <summary>
        /// Switches to the currently active modal dialog for this particular driver instance.
        /// </summary>
        /// <returns>A handle to the dialog.</returns>
        public IAlert Alert()
        {
            // N.B. We only execute the GetAlertText command to be able to throw
            // a NoAlertPresentException if there is no alert found.
            driver.InternalExecute(DriverCommand.GetAlertText, null);
            return new RemoteAlert(driver);
        }
    }
}
