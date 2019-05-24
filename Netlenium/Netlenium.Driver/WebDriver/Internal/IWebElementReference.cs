using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the framework can serialize an element to the wire protocol.
    /// </summary>
    internal interface IWebElementReference
    {
        /// <summary>
        /// Gets the internal ID of the element.
        /// </summary>
        string ElementReferenceId { get; }

        /// <summary>
        /// Converts an object into an object that represents an element for the wire protocol.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> that represents an element in the wire protocol.</returns>
        Dictionary<string, object> ToDictionary();
    }
}
