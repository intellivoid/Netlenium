using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netlenium.Driver.WebDriver.Remote
{
    public class RemoteWebElementFactory
    {
        private RemoteWebDriver driver;

        public RemoteWebElementFactory(RemoteWebDriver parentDriver)
        {
            driver = parentDriver;
        }

        protected RemoteWebDriver ParentDriver
        {
            get { return driver; }
        }

        /// <summary>
        /// Creates a <see cref="RemoteWebElement"/> from a dictionary containing a reference to an element.
        /// </summary>
        /// <param name="elementDictionary">The dictionary containing the element reference.</param>
        /// <returns>A <see cref="RemoteWebElement"/> containing the information from the specified dictionary.</returns>
        public virtual RemoteWebElement CreateElement(Dictionary<string, object> elementDictionary)
        {
            var elementId = GetElementId(elementDictionary);
            return new RemoteWebElement(ParentDriver, elementId);
        }

        /// <summary>
        /// Gets a value indicating wether the specified dictionary represents a reference to a web element.
        /// </summary>
        /// <param name="elementDictionary">The dictionary to check.</param>
        /// <returns><see langword="true"/> if the dictionary contains an element reference; otherwise, <see langword="false"/>.</returns>
        public bool ContainsElementReference(Dictionary<string, object> elementDictionary)
        {
            var elementPropertyName = string.Empty;
            return TryGetElementPropertyName(elementDictionary, out elementPropertyName);
        }

        public string GetElementId(Dictionary<string, object> elementDictionary)
        {
            var elementPropertyName = string.Empty;
            if (!TryGetElementPropertyName(elementDictionary, out elementPropertyName))
            {
                throw new ArgumentException("elementDictionary", "The specified dictionary does not contain an element reference");
            }

            var elementId = elementDictionary[elementPropertyName].ToString();
            if (string.IsNullOrEmpty(elementId))
            {
                throw new InvalidOperationException("The specified element ID is either null or the empty string.");
            }

            return elementId;
        }

        private bool TryGetElementPropertyName(Dictionary<string, object> elementDictionary, out string elementPropertyName)
        {
            if (elementDictionary == null)
            {
                throw new ArgumentNullException("elementDictionary", "The dictionary containing the element reference cannot be null");
            }

            if (elementDictionary.ContainsKey(RemoteWebElement.ElementReferencePropertyName))
            {
                elementPropertyName = RemoteWebElement.ElementReferencePropertyName;
                return true;
            }

            if (elementDictionary.ContainsKey(RemoteWebElement.LegacyElementReferencePropertyName))
            {
                elementPropertyName = RemoteWebElement.LegacyElementReferencePropertyName;
                return true;
            }

            elementPropertyName = string.Empty;
            return false;
        }
    }
}
