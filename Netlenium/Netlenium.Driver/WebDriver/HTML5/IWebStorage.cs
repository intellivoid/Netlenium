namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Represents both local and session storage for the site currently opened in the browser.
    /// </summary>
    public interface IWebStorage
    {
        /// <summary>
        /// Gets the local storage for the site currently opened in the browser.
        /// </summary>
        ILocalStorage LocalStorage { get; }

        /// <summary>
        /// Gets the session storage for the site currently opened in the browser.
        /// </summary>
        ISessionStorage SessionStorage { get; }
    }
}
