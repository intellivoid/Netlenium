using System;

namespace Netlenium.Intellivoid
{
    public class HttpRequestEventArgs : EventArgs
    {
        public HttpRequestEventArgs(HttpContext context)
        {
            Context = context ?? throw new ArgumentNullException("context");
        }

        public HttpContext Context { get; private set; }

        public HttpServerUtility Server => Context.Server;

        public HttpRequest Request => Context.Request;

        public HttpResponse Response => Context.Response;
    }

    public delegate void HttpRequestEventHandler(object sender, HttpRequestEventArgs e);
}
