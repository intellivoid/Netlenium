namespace Netlenium.Driver.WebDriver.Interactions.Internal
{
    /// <summary>
    /// Defines an action for mouse interaction with the browser.
    /// </summary>
    internal class MouseAction : WebDriverAction
    {
        private IMouse mouse;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseAction"/> class.
        /// </summary>
        /// <param name="mouse">The <see cref="IMouse"/> with which the action will be performed.</param>
        /// <param name="target">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        public MouseAction(IMouse mouse, ILocatable target)
            : base(target)
        {
            this.mouse = mouse;
        }

        /// <summary>
        /// Gets the coordinates at which to perform the mouse action.
        /// </summary>
        protected ICoordinates ActionLocation
        {
            get
            {
                if (ActionTarget == null)
                {
                    return null;
                }

                return ActionTarget.Coordinates;
            }
        }

        /// <summary>
        /// Gets the mouse with which to perform the action.
        /// </summary>
        protected IMouse Mouse
        {
            get { return mouse; }
        }

        /// <summary>
        /// Moves the mouse to the location at which to perform the action.
        /// </summary>
        protected void MoveToLocation()
        {
            // Only call MouseMove if an actual location was provided. If not,
            // the action will happen in the last known location of the mouse
            // cursor.
            if (ActionLocation != null)
            {
                mouse.MouseMove(ActionLocation);
            }
        }
    }
}
