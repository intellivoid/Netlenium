using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class ElementNotVisibleException : Exception
    {
        public ElementNotVisibleException()
        {
        }

        public ElementNotVisibleException(string message) : base(message)
        {
        }

        public ElementNotVisibleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ElementNotVisibleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}