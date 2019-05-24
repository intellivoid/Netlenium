using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Defines an action for clicking the secondary mouse button on an element, displaying a context menu.
    /// </summary>
    internal class ContextClickAction : MouseAction, IAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextClickAction"/> class.
        /// </summary>
        /// <param name="mouse">The <see cref="IMouse"/> with which the action will be performed.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        public ContextClickAction(IMouse mouse, ILocatable actionTarget)
            : base(mouse, actionTarget)
        {
        }

        /// <summary>
        /// Performs this action.
        /// </summary>
        public void Perform()
        {
            MoveToLocation();
            Mouse.ContextClick(ActionLocation);
        }
    }
}
