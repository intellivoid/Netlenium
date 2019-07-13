using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class DriverException : Exception
    {
        public DriverException()
        {
        }

        public DriverException(string message) : base(message)
        {
        }

        public DriverException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DriverException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}