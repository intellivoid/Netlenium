using System;
using System.Runtime.Serialization;

namespace NetleniumServer.Intellivoid
{
    [Serializable]
    internal class WebServerException : Exception
    {
        public WebServerException()
        {
        }

        public WebServerException(string message) : base(message)
        {
        }

        public WebServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}