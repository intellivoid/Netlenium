using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Defines an action for releasing the currently held mouse button.
    /// </summary>
    /// <remarks>
    /// This action can be called for an element different than the one
    /// ClickAndHoldAction was called for. However, if this action is
    /// performed out of sequence (without holding down the mouse button,
    /// for example) the results will be different.
    /// </remarks>
    internal class ButtonReleaseAction : MouseAction, IAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonReleaseAction"/> class.
        /// </summary>
        /// <param name="mouse">The <see cref="IMouse"/> with which the action will be performed.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        public ButtonReleaseAction(IMouse mouse, ILocatable actionTarget)
            : base(mouse, actionTarget)
        {
        }

        /// <summary>
        /// Performs this action.
        /// </summary>
        public void Perform()
        {
            // Releases the mouse button currently left held.
            // between browsers.
            MoveToLocation();
            Mouse.MouseUp(ActionLocation);
        }
    }
}
