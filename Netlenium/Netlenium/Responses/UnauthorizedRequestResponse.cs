using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Returned when the given request is unauthorized
    /// </summary>
    [Serializable]
    internal class UnauthorizedRequestResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public UnauthorizedRequestResponse()
        {
            Status = false;
            ResponseCode = 401;
            ErrorCode = ErrorCode.Unauthorized;
            Message = "Unauthorized Request";
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }
    }
}