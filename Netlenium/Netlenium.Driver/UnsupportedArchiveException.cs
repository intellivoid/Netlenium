using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class UnsupportedArchiveException : Exception
    {
        public UnsupportedArchiveException()
        {
        }

        public UnsupportedArchiveException(string message) : base(message)
        {
        }

        public UnsupportedArchiveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedArchiveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}