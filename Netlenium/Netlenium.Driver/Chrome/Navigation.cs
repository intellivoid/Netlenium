using System;

namespace Netlenium.Driver.Chrome
{
    public class Navigation : INavigation
    {
        private readonly Driver _driver;

        /// <summary>
        /// Public Constructor for Navigation
        /// </summary>
        /// <param name="driver"></param>
        public Navigation(Driver driver)
        {
            _driver = driver;
        }

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void GoForward()
        {
            throw new NotImplementedException();
        }

        public void LoadURI(Uri location)
        {
            throw new NotImplementedException();
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }
    }
}
