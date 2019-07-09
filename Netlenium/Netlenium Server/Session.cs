using Netlenium.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer
{
    public class Session
    {
        /// <summary>
        /// The driver associated with this session
        /// </summary>
        public IDriver Driver { get; }

        /// <summary>
        /// The session ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Session(Browser targetBrowser)
        {
            switch(targetBrowser)
            {
                case Browser.Chrome:
                    Driver = new Netlenium.Driver.Chrome.Driver();
                    break;
            }
        }
    }
}
