using System.Collections.Generic;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Defines the interface through which the user can access the driver used to find an element.
    /// </summary>
    internal interface IHasCapabilitiesDictionary
    {
        /// <summary>
        /// Gets the underlying Dictionary for a given set of capabilities.
        /// </summary>
        Dictionary<string, object> CapabilitiesDictionary { get; }
    }
}
