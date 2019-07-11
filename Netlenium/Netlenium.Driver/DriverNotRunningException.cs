using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    public class DriverNotRunningException : Exception
    {
        public DriverNotRunningException()
        {
        }

        public DriverNotRunningException(string message) : base(message)
        {
        }

        public DriverNotRunningException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DriverNotRunningException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}