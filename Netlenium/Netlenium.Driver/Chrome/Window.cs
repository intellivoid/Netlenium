using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver.Chrome
{
    public class Window : IWindow
    {
        private string id;
        public string ID => id;

        private Driver driver;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="id"></param>
        public Window(Driver driver, string id)
        {
            this.id = id;
            this.driver = driver;
        }

        public void SwitchTo()
        {
            try
            {
                driver.RemoteDriver.SwitchTo().Window(id);
            }
            catch(Exception)
            {
                throw new NoSuchWindowException();
            }
        }
    }
}
