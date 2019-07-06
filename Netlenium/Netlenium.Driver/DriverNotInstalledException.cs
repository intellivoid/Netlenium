using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class DriverNotInstalledException : Exception
    {
        public DriverNotInstalledException()
        {
        }

        public DriverNotInstalledException(string message) : base(message)
        {
        }

        public DriverNotInstalledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DriverNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}