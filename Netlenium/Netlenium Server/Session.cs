using Netlenium.Driver;
using System;

namespace NetleniumServer
{
    /// <summary>
    /// Session Object
    /// </summary>
    public class Session
    {

        /// <summary>
        /// Internal Driver Object
        /// </summary>
        private IDriver driver;

        /// <summary>
        /// The driver associated with this session
        /// </summary>
        public IDriver Driver
        {
            get
            {
                LastActivity = DateTime.Now;
                return driver;
            }
        }

        /// <summary>
        /// The session ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The timestamp that this session has started
        /// </summary>
        public DateTime SessionStarted { get; set; }

        /// <summary>
        /// The session's last activity
        /// </summary>
        public DateTime LastActivity { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Session(Browser targetBrowser)
        {
            switch(targetBrowser)
            {
                case Browser.Chrome:
                    driver = new Netlenium.Driver.Chrome.Driver();
                    break;

                case Browser.Firefox:
                    driver = new Netlenium.Driver.Firefox.Driver();
                    break;
                
                case Browser.Opera:
                    driver = new Netlenium.Driver.Opera.Driver();
                    break;
            }

            Utilities.ApplyOptionsToDriver(driver);

            SessionStarted = DateTime.Now;
            LastActivity = SessionStarted;
        }
    }
}
