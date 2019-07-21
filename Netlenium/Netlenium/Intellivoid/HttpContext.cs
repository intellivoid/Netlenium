namespace Netlenium.Intellivoid
{
    
    /// <summary>
    /// HttpContext Class
    /// </summary>
    public class HttpContext
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="client"></param>
        internal HttpContext(HttpClient client)
        {
            Server = client.Server.ServerUtility;
            Request = new HttpRequest(client);
            Response = new HttpResponse(this);
        }

        /// <summary>
        /// HTTP Server Utility
        /// </summary>
        public HttpServerUtility Server { get; private set; }

        /// <summary>
        /// HTTP Request
        /// </summary>
        public HttpRequest Request { get; private set; }

        /// <summary>
        /// HTTP Response
        /// </summary>
        public HttpResponse Response { get; private set; }
    }
}
