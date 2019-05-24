namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Provides methods by which an interaction with the browser can be performed.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Performs this action on the browser.
        /// </summary>
        void Perform();
    }
}
