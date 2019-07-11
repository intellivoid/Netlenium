using System;
using System.Runtime.Serialization;

namespace NetleniumServer
{
    [Serializable]
    internal class InvalidSearchByValueException : Exception
    {
        public InvalidSearchByValueException()
        {
        }

        public InvalidSearchByValueException(string message) : base(message)
        {
        }

        public InvalidSearchByValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSearchByValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}