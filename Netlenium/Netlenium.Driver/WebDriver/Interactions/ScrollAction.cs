using System;
using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Creates a double tap gesture on a touch screen.
    /// </summary>
    internal class ScrollAction : TouchAction, IAction
    {
        private int offsetX;
        private int offsetY;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollAction"/> class.
        /// </summary>
        /// <param name="touchScreen">The <see cref="ITouchScreen"/> with which the action will be performed.</param>
        /// <param name="offsetX">The horizontal offset relative to the view port.</param>
        /// <param name="offsetY">The vertical offset relative to the view port.</param>
        public ScrollAction(ITouchScreen touchScreen, int offsetX, int offsetY)
            : this(touchScreen, null, offsetX, offsetY)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollAction"/> class for use with the specified element.
        /// </summary>
        /// <param name="touchScreen">The <see cref="ITouchScreen"/> with which the action will be performed.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        /// <param name="offsetX">The x coordinate relative to the view port.</param>
        /// <param name="offsetY">The y coordinate relative to the view port.</param>
        public ScrollAction(ITouchScreen touchScreen, ILocatable actionTarget, int offsetX, int offsetY)
            : base(touchScreen, actionTarget)
        {
            if (actionTarget == null)
            {
                throw new ArgumentException("Must provide a location for a single tap action.", "actionTarget");
            }

            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        public void Perform()
        {
            TouchScreen.Scroll(ActionLocation, offsetX, offsetY);
        }
    }
}
