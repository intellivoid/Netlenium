using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class UnsupportedSearchMethodException : Exception
    {
        public UnsupportedSearchMethodException()
        {
        }

        public UnsupportedSearchMethodException(string message) : base(message)
        {
        }

        public UnsupportedSearchMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedSearchMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}