using System;
using System.Runtime.Serialization;

namespace Netlenium
{
    [Serializable]
    internal class UnsupportedRequestMethodException : Exception
    {
        public UnsupportedRequestMethodException()
        {
        }

        public UnsupportedRequestMethodException(string message) : base(message)
        {
        }

        public UnsupportedRequestMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedRequestMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}