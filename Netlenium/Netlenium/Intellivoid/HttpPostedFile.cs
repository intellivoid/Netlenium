﻿using System;
using System.IO;

namespace Netlenium.Intellivoid
{
    public class HttpPostedFile
    {
        public HttpPostedFile(int contentLength, string contentType, string fileName, Stream inputStream)
        {
            ContentLength = contentLength;
            ContentType = contentType;
            FileName = fileName ?? throw new ArgumentNullException("fileName");
            InputStream = inputStream ?? throw new ArgumentNullException("inputStream");
        }

        public int ContentLength { get; private set; }

        public string ContentType { get; private set; }

        public string FileName { get; private set; }

        public Stream InputStream { get; private set; }
    }
}
