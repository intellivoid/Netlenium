using System;
using System.Runtime.Serialization;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// The exception that is thrown when a window is not found.
    /// </summary>
    [Serializable]
    public class NoSuchWindowException : NotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchWindowException"/> class.
        /// </summary>
        public NoSuchWindowException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchWindowException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NoSuchWindowException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchWindowException"/> class with
        /// a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <see langword="null"/> if no inner exception is specified.</param>
        public NoSuchWindowException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchWindowException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        protected NoSuchWindowException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
