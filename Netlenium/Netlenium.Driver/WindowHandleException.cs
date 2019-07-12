using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class WindowHandleException : Exception
    {
        public WindowHandleException()
        {
        }

        public WindowHandleException(string message) : base(message)
        {
        }

        public WindowHandleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WindowHandleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}