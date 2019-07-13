using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    public class ElementNotSelectableException : Exception
    {
        public ElementNotSelectableException()
        {
        }

        public ElementNotSelectableException(string message) : base(message)
        {
        }

        public ElementNotSelectableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ElementNotSelectableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}