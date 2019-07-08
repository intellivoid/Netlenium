namespace Netlenium.Driver
{
    /// <summary>
    /// Proxy Configuration Interface
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// If the browser is configured to use a proxy
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The host to connect to
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// The port to connect to
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// If authentication is required
        /// </summary>
        bool AuthenticationRequried { get; set; }

        /// <summary>
        /// Username for authentication
        /// </summary>
        string Username { get; set; }


        /// <summary>
        /// Password for authentication
        /// </summary>
        string Password { get; set; }
    }
}
