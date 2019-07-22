using System;

namespace Netlenium.Objects
{
    /// <summary>
    /// Session Object representing details about the Session and Driver
    /// </summary>
    [Serializable]
    public class Session
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="session"></param>
        public Session(Netlenium.Session session)
        {
            ID = session.Id;
            Created = session.SessionStarted.ToString();
            LastActivity = session.LastActivity.ToString();
            Driver = session.Driver.TargetBrowser.ToString();
            Headless = session.Driver.Headless;
            ProxyConfiguration = new Proxy(session.Driver.ProxyConfiguration);
            CurrentWindow = new CurrentWindow
            {
                ID = session.Driver.Actions.CurrentWindow.ID,
                Title = session.Driver.Document.Title,
                Url = session.Driver.Document.Uri
            };
        }

        public string ID { get; set; }

        public string Created { get; set; }

        public string LastActivity { get; set; }

        public string Driver { get; set; }

        public bool Headless { get; set; }

        public Proxy ProxyConfiguration { get; set; }

        public CurrentWindow CurrentWindow { get; set; }
    }
}
