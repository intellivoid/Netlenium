using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    public class DriverAlreadyRunningException : Exception
    {
        public DriverAlreadyRunningException()
        {
        }

        public DriverAlreadyRunningException(string message) : base(message)
        {
        }

        public DriverAlreadyRunningException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DriverAlreadyRunningException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}