﻿using System;

namespace Netlenium.Intellivoid
{
    internal abstract class HttpRequestParser : IDisposable
    {
        protected HttpClient Client { get; private set; }
        protected int ContentLength { get; private set; }

        protected HttpRequestParser(HttpClient client, int contentLength)
        {
            Client = client ?? throw new ArgumentNullException("client");
            ContentLength = contentLength;
        }

        public abstract void Parse();

        protected void EndParsing()
        {
            // Disconnect the parser.

            Client.UnsetParser();

            // Resume processing the request.

            Client.ExecuteRequest();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
