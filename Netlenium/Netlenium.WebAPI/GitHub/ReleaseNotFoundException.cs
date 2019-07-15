using System;
using System.Runtime.Serialization;

namespace Netlenium.WebAPI.GitHub
{
    [Serializable]
    internal class ReleaseNotFoundException : Exception
    {
        public ReleaseNotFoundException()
        {
        }

        public ReleaseNotFoundException(string message) : base(message)
        {
        }

        public ReleaseNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReleaseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}