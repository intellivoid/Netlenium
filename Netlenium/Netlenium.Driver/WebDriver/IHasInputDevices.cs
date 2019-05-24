using System;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Provides access to input devices for advanced user interactions.
    /// </summary>
    //[Obsolete("Use the Actions or ActionBuilder class to simulate mouse and keyboard input.")]
    public interface IHasInputDevices
    {
        /// <summary>
        /// Gets an <see cref="IKeyboard"/> object for sending keystrokes to the browser.
        /// </summary>
        IKeyboard Keyboard { get; }

        /// <summary>
        /// Gets an <see cref="IMouse"/> object for sending mouse commands to the browser.
        /// </summary>
        IMouse Mouse { get; }
    }
}
