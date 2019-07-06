using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class WebRequestException : Exception
    {
        public WebRequestException()
        {
        }

        public WebRequestException(string message) : base(message)
        {
        }

        public WebRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}