using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    public class UnsupportedPlatformException : Exception
    {
        public UnsupportedPlatformException()
        {
        }

        public UnsupportedPlatformException(string message) : base(message)
        {
        }

        public UnsupportedPlatformException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedPlatformException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}