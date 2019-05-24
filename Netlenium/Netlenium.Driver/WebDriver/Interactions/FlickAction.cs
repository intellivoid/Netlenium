using System;
using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Creates a flick gesture on a touch screen.
    /// </summary>
    internal class FlickAction : TouchAction, IAction
    {
        private int offsetX;
        private int offsetY;
        private int speed;
        private int speedX;
        private int speedY;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlickAction"/> class.
        /// </summary>
        /// <param name="touchScreen">The <see cref="ITouchScreen"/> with which the action will be performed.</param>
        /// <param name="speedX">The horizontal speed in pixels per second.</param>
        /// <param name="speedY">The vertical speed in pixels per second.</param>
        public FlickAction(ITouchScreen touchScreen, int speedX, int speedY)
            : base(touchScreen, null)
        {
            this.speedX = speedX;
            this.speedY = speedY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlickAction"/> class for use with the specified element.
        /// </summary>
        /// <param name="touchScreen">The <see cref="ITouchScreen"/> with which the action will be performed.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> describing an element at which to perform the action.</param>
        /// <param name="offsetX">The x offset relative to the viewport.</param>
        /// <param name="offsetY">The y offset relative to the viewport.</param>
        /// <param name="speed">The speed in pixels per second.</param>
        public FlickAction(ITouchScreen touchScreen, ILocatable actionTarget, int offsetX, int offsetY, int speed)
            : base(touchScreen, actionTarget)
        {
            if (actionTarget == null)
            {
                throw new ArgumentException("Must provide a location for a single tap action.", "actionTarget");
            }

            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.speed = speed;
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        public void Perform()
        {
            if (ActionLocation != null)
            {
                TouchScreen.Flick(ActionLocation, offsetX, offsetY, speed);
            }
            else
            {
                TouchScreen.Flick(speedX, speedY);
            }
        }
    }
}
