using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Generic 404 Response
    /// </summary>
    [Serializable]
    public class NotFoundResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public NotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            ErrorCode = ErrorCode.ResourceNotFound;
            Message = "The requested resource was not found";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
        public ErrorCode ErrorCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}