using System;
using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <inheritdoc cref="SingleTapAction" />
    /// <summary>
    /// Creates a single tap gesture on a touch screen.
    /// </summary>
    internal class SingleTapAction : TouchAction, IAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTapAction"/> class.
        /// </summary>
        /// <param name="touchScreen">The <see cref="ITouchScreen"/> with which the action will be performed.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        public SingleTapAction(ITouchScreen touchScreen, ILocatable actionTarget)
            : base(touchScreen, actionTarget)
        {
            if (actionTarget == null)
            {
                throw new ArgumentException("Must provide a location for a single tap action.", "actionTarget");
            }
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        public void Perform()
        {
            TouchScreen.SingleTap(ActionLocation);
        }
    }
}
