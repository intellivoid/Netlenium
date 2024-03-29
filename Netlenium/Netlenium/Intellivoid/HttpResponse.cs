﻿using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;

namespace Netlenium.Intellivoid
{
    public class HttpResponse
    {
        private readonly HttpContext context;
        public string CacheControl { get; set; }

        public string CharSet { get; set; }

        public string ContentType { get; set; }

        public HttpCookieCollection Cookies { get; private set; }

        public DateTime ExpiresAbsolute { get; set; }

        public Encoding HeadersEncoding { get; set; }

        public NameValueCollection Headers { get; private set; }

        public bool IsClientConnected => true;

        public bool IsRequestBeingRedirected => !String.IsNullOrEmpty(RedirectLocation);

        public HttpOutputStream OutputStream { get; private set; }

        public string RedirectLocation { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string Status
        {
            get
            {
                if (StatusDescription == null)
                    return StatusCode.ToString(CultureInfo.InvariantCulture);
                return StatusCode.ToString(CultureInfo.InvariantCulture) + " " + StatusDescription;
            }
            set
            {
                StatusCode = 0;
                StatusDescription = null;

                if (value != null)
                {
                    var parts = value.Split(new[] { ' ' }, 2);
                    int statusCode;

                    if (int.TryParse(parts[0], out statusCode))
                    {
                        StatusCode = statusCode;

                        if (parts.Length == 2)
                            StatusDescription = parts[1];
                        else
                            StatusDescription = null;
                    }
                }
            }
        }

        internal HttpResponse(HttpContext context)
        {
            this.context = context ?? throw new ArgumentNullException("context");

            CacheControl = "private";
            CharSet = "utf-8";
            ContentType = "text/html";
            ExpiresAbsolute = DateTime.Now.AddSeconds(2);
            HeadersEncoding = Encoding.UTF8;
            Headers = new NameValueCollection();
            OutputStream = new HttpOutputStream(new MemoryStream());
            StatusCode = 200;
            StatusDescription = "OK";
            Cookies = new HttpCookieCollection();
        }

        public void Redirect(string location)
        {
            Redirect(location, false);
        }

        public void RedirectPermanent(string location)
        {
            Redirect(location, true);
        }

        private void Redirect(string location, bool permanent)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            if (!location.Contains(":"))
            {
                var sb = new StringBuilder();
                var url = context.Request.Url;

                sb.Append(url.Scheme);
                sb.Append("://");
                sb.Append(url.Host);

                if (!url.IsDefaultPort)
                {
                    sb.Append(':');
                    sb.Append(url.Port);
                }

                if (location.Length == 0)
                    location = context.Request.Path;

                if (location[0] == '/')
                {
                    sb.Append(location);
                }
                else
                {
                    var path = context.Request.Path;
                    var pos = path.LastIndexOf('/');

                    if (pos == -1)
                        sb.Append('/');
                    else
                        sb.Append(path.Substring(0, pos + 1));

                    sb.Append(location);
                }

                location = sb.ToString();
            }

            RedirectLocation = location;
            StatusCode = permanent ? 301 : 302;
            StatusDescription = "Moved";
        }
    }
}
