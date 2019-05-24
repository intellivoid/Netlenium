using System;
using System.Collections.Generic;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Defines the interface through which the user can execute advanced keyboard interactions.
    /// </summary>
    internal class RemoteKeyboard : IKeyboard
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteKeyboard"/> class.
        /// </summary>
        /// <param name="driver">The <see cref="RemoteWebDriver"/> for which the keyboard will be managed.</param>
        public RemoteKeyboard(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the target.
        /// </summary>
        /// <param name="keySequence">A string representing the keystrokes to send.</param>
        public void SendKeys(string keySequence)
        {
            if (keySequence == null)
            {
                throw new ArgumentException("key sequence to send must not be null", "keySequence");
            }

            var parameters = new Dictionary<string, object>();
            parameters.Add("value", keySequence.ToCharArray());
            driver.InternalExecute(DriverCommand.SendKeysToActiveElement, parameters);
        }

        /// <summary>
        /// Presses a key.
        /// </summary>
        /// <param name="keyToPress">The key value representing the key to press.</param>
        /// <remarks>The key value must be one of the values from the <see cref="Keys"/> class.</remarks>
        public void PressKey(string keyToPress)
        {
            if (keyToPress == null)
            {
                throw new ArgumentException("key to press must not be null", "keyToPress");
            }

            var parameters = new Dictionary<string, object>();
            parameters.Add("value", keyToPress.ToCharArray());
            driver.InternalExecute(DriverCommand.SendKeysToActiveElement, parameters);
        }

        /// <summary>
        /// Releases a key.
        /// </summary>
        /// <param name="keyToRelease">The key value representing the key to release.</param>
        /// <remarks>The key value must be one of the values from the <see cref="Keys"/> class.</remarks>
        public void ReleaseKey(string keyToRelease)
        {
            if (keyToRelease == null)
            {
                throw new ArgumentException("key to release must not be null", "keyToRelease");
            }

            var parameters = new Dictionary<string, object>();
            parameters.Add("value", keyToRelease.ToCharArray());
            driver.InternalExecute(DriverCommand.SendKeysToActiveElement, parameters);
        }
    }
}
