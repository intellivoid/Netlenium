using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    public class ElementNotIntractableException : Exception
    {
        public ElementNotIntractableException()
        {
        }

        public ElementNotIntractableException(string message) : base(message)
        {
        }

        public ElementNotIntractableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ElementNotIntractableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}