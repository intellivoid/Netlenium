namespace Netlenium.Driver.Firefox
{
    public class Proxy : IProxy
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool AuthenticationRequried { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ProxyScheme Scheme { get; set; }

        private Driver driver;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="driver"></param>
        public Proxy(Driver driver)
        {
            this.driver = driver;
            Enabled = false;
            Host = "0.0.0.0";
            Port = 8080;
            AuthenticationRequried = false;
            Username = string.Empty;
            Password = string.Empty;
        }

    }
}