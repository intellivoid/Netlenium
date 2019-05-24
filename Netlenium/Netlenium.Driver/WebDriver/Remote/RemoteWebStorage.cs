using Netlenium.Driver.WebDriver.Html5;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides remote access to the <see cref="IWebStorage"/> API.
    /// </summary>
    public class RemoteWebStorage : IWebStorage
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebStorage"/> class.
        /// </summary>
        /// <param name="driver">The driver instance.</param>
        public RemoteWebStorage(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Gets the local storage for the site currently opened in the browser.
        /// </summary>
        public ILocalStorage LocalStorage
        {
            get
            {
                return new RemoteLocalStorage(driver);
            }
        }

        /// <summary>
        /// Gets the session storage for the site currently opened in the browser.
        /// </summary>
        public ISessionStorage SessionStorage
        {
            get
            {
                return new RemoteSessionStorage(driver);
            }
        }
    }
}
