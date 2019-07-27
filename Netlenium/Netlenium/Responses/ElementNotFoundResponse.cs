using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the element was not found
    /// </summary>
    [Serializable]
    public class ElementNotFoundResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public ElementNotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            ErrorCode = ErrorCode.ElementNotFound;
            Message = "The element was not found";
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }
        
        public string Message { get; set; }
    }
}