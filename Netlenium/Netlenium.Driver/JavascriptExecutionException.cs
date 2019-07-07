using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver
{
    [Serializable]
    internal class JavascriptExecutionException : Exception
    {
        public JavascriptExecutionException()
        {
        }

        public JavascriptExecutionException(string message) : base(message)
        {
        }

        public JavascriptExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JavascriptExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}