using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver.WebAPI.Google
{
    /// <summary>
    /// Raised when the resource was not found or does not exist
    /// </summary>
    [Serializable]
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Public Constructor without context
        /// </summary>
        public ResourceNotFoundException()
        {
        }

        /// <summary>
        /// Public constructor with message
        /// </summary>
        /// <param name="message"></param>
        public ResourceNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Public constructor with stack trace
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Public constructor with serialization information
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}