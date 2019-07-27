using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when there was an unexpected error
    /// </summary>
    [Serializable]
    public class UnexpectedErrorResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="exceptionMessage"></param>
        public UnexpectedErrorResponse(string exceptionMessage)
        {
            Status = false;
            ResponseCode = 500;
            ErrorCode = ErrorCode.UnexpectedError;
            Message = exceptionMessage;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }
    }
}