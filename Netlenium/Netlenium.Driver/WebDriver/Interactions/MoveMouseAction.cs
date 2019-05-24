using System;
using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Defines an action for moving the mouse to a specified location.
    /// </summary>
    internal class MoveMouseAction : MouseAction, IAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveMouseAction"/> class.
        /// </summary>
        /// <param name="mouse">The <see cref="IMouse"/> with which the action will be performed.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        public MoveMouseAction(IMouse mouse, ILocatable actionTarget)
            : base(mouse, actionTarget)
        {
            if (actionTarget == null)
            {
                throw new ArgumentException("Must provide a location for a move action.", "actionTarget");
            }
        }

        /// <summary>
        /// Performs this action.
        /// </summary>
        public void Perform()
        {
            Mouse.MouseMove(ActionLocation);
        }
    }
}
