using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class UnsupportedSchemeException : Exception
    {
        public UnsupportedSchemeException()
        {
        }

        public UnsupportedSchemeException(string message) : base(message)
        {
        }

        public UnsupportedSchemeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedSchemeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}