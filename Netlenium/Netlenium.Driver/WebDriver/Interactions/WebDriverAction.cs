namespace Netlenium.Driver.WebDriver.Interactions.Internal
{
    /// <summary>
    /// Defines an action for keyboard and mouse interaction with the browser.
    /// </summary>
    internal abstract class WebDriverAction
    {
        private ILocatable where;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverAction"/> class for the given element.
        /// </summary>
        /// <param name="actionLocation">An <see cref="ILocatable"/> object that provides coordinates for this action.</param>
        protected WebDriverAction(ILocatable actionLocation)
        {
            where = actionLocation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverAction"/> class.
        /// </summary>
        /// <remarks>This action will take place in the context of the previous action's coordinates.</remarks>
        protected WebDriverAction()
        {
        }

        /// <summary>
        /// Gets the target of the action providing coordinates of the action.
        /// </summary>
        protected ILocatable ActionTarget
        {
            get { return where; }
        }
    }
}
