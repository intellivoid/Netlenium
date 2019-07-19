namespace Netlenium.Driver.Firefox
{ 
    public class Proxy : IProxy
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        
        public bool AuthenticationRequired { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ProxyScheme Scheme { get; set; }
        
        /// <summary>
        /// Public Constructor
        /// </summary>
        public Proxy()
        {
            Enabled = false;
            Host = "0.0.0.0";
            Port = 8080;
            AuthenticationRequired = false;
            Username = string.Empty;
            Password = string.Empty;
        }

    }
}