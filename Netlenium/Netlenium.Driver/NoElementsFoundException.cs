using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class NoElementsFoundException : Exception
    {
        public NoElementsFoundException()
        {
        }

        public NoElementsFoundException(string message) : base(message)
        {
        }

        public NoElementsFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoElementsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}