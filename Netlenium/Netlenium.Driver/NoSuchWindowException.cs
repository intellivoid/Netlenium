using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    public class NoSuchWindowException : Exception
    {
        public NoSuchWindowException()
        {
        }

        public NoSuchWindowException(string message) : base(message)
        {
        }

        public NoSuchWindowException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSuchWindowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}