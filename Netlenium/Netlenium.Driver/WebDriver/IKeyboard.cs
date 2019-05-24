using System;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Provides methods representing basic keyboard actions.
    /// </summary>
    //[Obsolete("Use the Actions or ActionBuilder class to simulate keyboard input.")]
    public interface IKeyboard
    {
        /// <summary>
        /// Sends a sequence of keystrokes to the target.
        /// </summary>
        /// <param name="keySequence">A string representing the keystrokes to send.</param>
        void SendKeys(string keySequence);

        /// <summary>
        /// Presses a key.
        /// </summary>
        /// <param name="keyToPress">The key value representing the key to press.</param>
        /// <remarks>The key value must be one of the values from the <see cref="Keys"/> class.</remarks>
        void PressKey(string keyToPress);

        /// <summary>
        /// Releases a key.
        /// </summary>
        /// <param name="keyToRelease">The key value representing the key to release.</param>
        /// <remarks>The key value must be one of the values from the <see cref="Keys"/> class.</remarks>
        void ReleaseKey(string keyToRelease);
    }
}
