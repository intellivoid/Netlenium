using System.Drawing;
using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Defines the interface through which the user can discover where an element is on the screen.
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// Gets the location of an element on the screen, scrolling it into view
        /// if it is not currently on the screen.
        /// </summary>
        Point LocationOnScreenOnceScrolledIntoView { get; }

        /// <summary>
        /// Gets the coordinates identifying the location of this element using
        /// various frames of reference.
        /// </summary>
        ICoordinates Coordinates { get; }
    }
}
