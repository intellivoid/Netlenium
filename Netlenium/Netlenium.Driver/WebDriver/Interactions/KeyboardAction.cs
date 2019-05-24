namespace Netlenium.Driver.WebDriver.Interactions.Internal
{
    /// <summary>
    /// Defines an action for keyboard interaction with the browser.
    /// </summary>
    internal class KeyboardAction : WebDriverAction
    {
        private IKeyboard keyboard;
        private IMouse mouse;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardAction"/> class.
        /// </summary>
        /// <param name="keyboard">The <see cref="IKeyboard"/> to use in performing the action.</param>
        /// <param name="mouse">The <see cref="IMouse"/> to use in setting focus to the element on which to perform the action.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> object providing the element on which to perform the action.</param>
        protected KeyboardAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget)
            : base(actionTarget)
        {
            this.keyboard = keyboard;
            this.mouse = mouse;
        }

        /// <summary>
        /// Gets the keyboard with which to perform the action.
        /// </summary>
        protected IKeyboard Keyboard
        {
            get { return keyboard; }
        }

        /// <summary>
        /// Focuses on the element on which the action is to be performed.
        /// </summary>
        protected void FocusOnElement()
        {
            if (ActionTarget != null)
            {
                mouse.Click(ActionTarget.Coordinates);
            }
        }
    }
}
