using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    public class AttributeNotFoundException : Exception
    {
        public AttributeNotFoundException()
        {
        }

        public AttributeNotFoundException(string message) : base(message)
        {
        }

        public AttributeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AttributeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}