using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Defines an action for moving the mouse to a specified offset from its current location.
    /// </summary>
    internal class MoveToOffsetAction : MouseAction, IAction
    {
        private int offsetX;
        private int offsetY;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveToOffsetAction"/> class.
        /// </summary>
        /// <param name="mouse">The <see cref="IMouse"/> with which the action will be performed.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        /// <param name="offsetX">The horizontal offset from the origin of the target to which to move the mouse.</param>
        /// <param name="offsetY">The vertical offset from the origin of the target to which to move the mouse.</param>
        public MoveToOffsetAction(IMouse mouse, ILocatable actionTarget, int offsetX, int offsetY)
            : base(mouse, actionTarget)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        /// <summary>
        /// Performs this action.
        /// </summary>
        public void Perform()
        {
            Mouse.MouseMove(ActionLocation, offsetX, offsetY);
        }
    }
}
