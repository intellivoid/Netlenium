using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class ElementNotInteractableException : Exception
    {
        public ElementNotInteractableException()
        {
        }

        public ElementNotInteractableException(string message) : base(message)
        {
        }

        public ElementNotInteractableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ElementNotInteractableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}