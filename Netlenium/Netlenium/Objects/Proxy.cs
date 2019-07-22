using Netlenium.Driver;
using System;

namespace Netlenium.Objects
{
    /// <summary>
    /// Proxy Configuration Object
    /// </summary>
    [Serializable]
    public class Proxy
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="proxy"></param>
        public Proxy(IProxy proxy)
        {
            Enabled = false;
            Host = string.Empty;
            Port = 0;
            AuthenticationRequired = false;
            Username = string.Empty;
            Password = string.Empty;

            switch(proxy.Scheme)
            {
                case ProxyScheme.HTTP:
                    Scheme = "http";
                    break;

                case ProxyScheme.HTTPS:
                    Scheme = "https";
                    break;

                default:
                    Scheme = "unknown";
                    break;
            }


            Enabled = proxy.Enabled;

            if (proxy.Host != null)
            {
                Host = proxy.Host;
            }

            Port = proxy.Port;

            AuthenticationRequired = proxy.AuthenticationRequired;

            if (proxy.Username != null)
            {
                Username = proxy.Username;
            }

            if (proxy.Password != null)
            {
                Password = proxy.Password;
            }
        }

        public bool Enabled { get; set; }

        public string Scheme { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool AuthenticationRequired { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
