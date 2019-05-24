using System;
using System.Collections.Generic;
using Netlenium.Driver.WebDriver.Interactions.Internal;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Defines the interface through which the user can discover where an element is on the screen.
    /// </summary>
    internal class RemoteCoordinates : ICoordinates
    {
        private RemoteWebElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCoordinates"/> class.
        /// </summary>
        /// <param name="element">The <see cref="RemoteWebElement"/> to be located.</param>
        public RemoteCoordinates(RemoteWebElement element)
        {
            this.element = element;
        }

        /// <summary>
        /// Gets the location of an element in absolute screen coordinates.
        /// </summary>
        public System.Drawing.Point LocationOnScreen
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the location of an element relative to the origin of the view port.
        /// </summary>
        public System.Drawing.Point LocationInViewport
        {
            get { return element.LocationOnScreenOnceScrolledIntoView; }
        }

        /// <summary>
        /// Gets the location of an element's position within the HTML DOM.
        /// </summary>
        public System.Drawing.Point LocationInDom
        {
            get { return element.Location; }
        }

        /// <summary>
        /// Gets a locator providing a user-defined location for this element.
        /// </summary>
        public object AuxiliaryLocator
        {
            get
            {
                var elementReference = element as IWebElementReference;
                if (elementReference == null)
                {
                    return null;
                }

                // Note that the OSS dialect of the wire protocol for the Actions API
                // uses the raw ID of the element, not an element reference. To use this,
                // extract the ID using the well-known key to the dictionary for element
                // references.
                return elementReference.ElementReferenceId;
            }
        }
    }
}
