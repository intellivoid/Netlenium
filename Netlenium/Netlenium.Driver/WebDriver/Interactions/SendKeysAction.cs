using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Defines an action for sending a sequence of keystrokes to an element.
    /// </summary>
    internal class SendKeysAction : KeyboardAction, IAction
    {
        private string keysToSend;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendKeysAction"/> class.
        /// </summary>
        /// <param name="keyboard">The <see cref="IKeyboard"/> to use in performing the action.</param>
        /// <param name="mouse">The <see cref="IMouse"/> to use in setting focus to the element on which to perform the action.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> object providing the element on which to perform the action.</param>
        /// <param name="keysToSend">The key sequence to send.</param>
        public SendKeysAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string keysToSend)
            : base(keyboard, mouse, actionTarget)
        {
            this.keysToSend = keysToSend;
        }

        /// <summary>
        /// Performs this action.
        /// </summary>
        public void Perform()
        {
            FocusOnElement();
            Keyboard.SendKeys(keysToSend);
        }
    }
}
