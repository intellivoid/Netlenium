#pragma warning disable 618
namespace Netlenium.Driver.WebDriver.Interactions.Internal
{
    /// <summary>
    /// Defines an action for keyboard interaction with the browser.
    /// </summary>
    internal class TouchAction : WebDriverAction
    {
        private ITouchScreen touchScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchAction"/> class.
        /// </summary>
        /// <param name="touchScreen">The <see cref="ITouchScreen"/> to use in performing the action.</param>
        /// <param name="actionTarget">An <see cref="ILocatable"/> object providing the element on which to perform the action.</param>
        protected TouchAction(ITouchScreen touchScreen, ILocatable actionTarget)
            : base(actionTarget)
        {
            this.touchScreen = touchScreen;
        }

        /// <summary>
        /// Gets the touch screen with which to perform the action.
        /// </summary>
        protected ITouchScreen TouchScreen
        {
            get { return touchScreen; }
        }

        /// <summary>
        /// Gets the location at which to perform the action.
        /// </summary>
        protected ICoordinates ActionLocation
        {
            get
            {
                if (ActionTarget != null)
                {
                    return ActionTarget.Coordinates;
                }

                return null;
            }
        }
    }
}
