using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Defines an action for releasing a modifier key (Shift, Alt, or Control) on the keyboard.
    /// </summary>
    internal class KeyUpAction : SingleKeyAction, IAction
    {
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyUpAction"/> class.
    /// </summary>
    /// <param name="keyboard">The <see cref="IKeyboard"/> to use in performing the action.</param>
    /// <param name="mouse">The <see cref="IMouse"/> to use in setting focus to the element on which to perform the action.</param>
    /// <param name="actionTarget">An <see cref="ILocatable"/> object providing the element on which to perform the action.</param>
    /// <param name="key">The modifier key (<see cref="Keys.Shift"/>, <see cref="Keys.Control"/>, <see cref="Keys.Alt"/>,
    /// <see cref="Keys.Meta"/>, <see cref="Keys.Command"/>,<see cref="Keys.LeftAlt"/>,
    /// <see cref="Keys.LeftControl"/>,<see cref="Keys.LeftShift"/>) to use in the action.</param>
    public KeyUpAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string key)
            : base(keyboard, mouse, actionTarget, key)
        {
        }

        /// <summary>
        /// Performs this action.
        /// </summary>
        public void Perform()
        {
            FocusOnElement();
            Keyboard.ReleaseKey(Key);
        }
    }
}
