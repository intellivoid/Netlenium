using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Defines an interface allowing the user to manipulate cookies on the current page.
    /// </summary>
    internal class RemoteCookieJar : ICookieJar
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCookieJar"/> class.
        /// </summary>
        /// <param name="driver">The driver that is currently in use</param>
        public RemoteCookieJar(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Gets all cookies defined for the current page.
        /// </summary>
        public ReadOnlyCollection<Cookie> AllCookies
        {
            get { return GetAllCookies(); }
        }

        /// <summary>
        /// Method for creating a cookie in the browser
        /// </summary>
        /// <param name="cookie"><see cref="Cookie"/> that represents a cookie in the browser</param>
        public void AddCookie(Cookie cookie)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("cookie", cookie);
            driver.InternalExecute(DriverCommand.AddCookie, parameters);
        }

        /// <summary>
        /// Delete the cookie by passing in the name of the cookie
        /// </summary>
        /// <param name="name">The name of the cookie that is in the browser</param>
        public void DeleteCookieNamed(string name)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("name", name);
            driver.InternalExecute(DriverCommand.DeleteCookie, parameters);
        }

        /// <summary>
        /// Delete a cookie in the browser by passing in a copy of a cookie
        /// </summary>
        /// <param name="cookie">An object that represents a copy of the cookie that needs to be deleted</param>
        public void DeleteCookie(Cookie cookie)
        {
            if (cookie != null)
            {
                DeleteCookieNamed(cookie.Name);
            }
        }

        /// <summary>
        /// Delete All Cookies that are present in the browser
        /// </summary>
        public void DeleteAllCookies()
        {
            driver.InternalExecute(DriverCommand.DeleteAllCookies, null);
        }

        /// <summary>
        /// Method for returning a getting a cookie by name
        /// </summary>
        /// <param name="name">name of the cookie that needs to be returned</param>
        /// <returns>A Cookie from the name</returns>
        public Cookie GetCookieNamed(string name)
        {
            Cookie cookieToReturn = null;
            if (name != null)
            {
                var allCookies = AllCookies;
                foreach (var currentCookie in allCookies)
                {
                    if (name.Equals(currentCookie.Name))
                    {
                        cookieToReturn = currentCookie;
                        break;
                    }
                }
            }

            return cookieToReturn;
        }

        /// <summary>
        /// Method for getting a Collection of Cookies that are present in the browser
        /// </summary>
        /// <returns>ReadOnlyCollection of Cookies in the browser</returns>
        private ReadOnlyCollection<Cookie> GetAllCookies()
        {
            var toReturn = new List<Cookie>();
            var returned = driver.InternalExecute(DriverCommand.GetAllCookies, new Dictionary<string, object>()).Value;

            try
            {
                var cookies = returned as object[];
                if (cookies != null)
                {
                    foreach (var rawCookie in cookies)
                    {
                        var cookieDictionary = rawCookie as Dictionary<string, object>;
                        if (rawCookie != null)
                        {
                            toReturn.Add(Cookie.FromDictionary(cookieDictionary));
                        }
                    }
                }

                return new ReadOnlyCollection<Cookie>(toReturn);
            }
            catch (Exception e)
            {
                throw new WebDriverException("Unexpected problem getting cookies", e);
            }
        }
    }
}
