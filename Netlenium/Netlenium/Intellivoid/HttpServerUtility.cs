﻿using System;
using System.Text;

namespace Netlenium.Intellivoid
{
    public class HttpServerUtility
    {
        internal HttpServerUtility()
        {
        }

        public string MachineName => Environment.MachineName;

        public string HtmlEncode(string value)
        {
            return HttpUtil.HtmlEncode(value);
        }

        public string HtmlDecode(string value)
        {
            return HttpUtil.HtmlDecode(value);
        }

        public string UrlEncode(string text)
        {
            return Uri.EscapeDataString(text);
        }

        public string UrlDecode(string text)
        {
            return UrlDecode(text, Encoding.UTF8);
        }

        public string UrlDecode(string text, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            return HttpUtil.UriDecode(text, encoding);
        }
    }
}
