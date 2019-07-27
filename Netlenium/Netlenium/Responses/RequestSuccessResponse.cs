using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the given request was a success
    /// </summary>
    [Serializable]
    public class RequestSuccessResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public RequestSuccessResponse()
        {
            Status = true;
            ResponseCode = 200;
            ErrorCode = ErrorCode.NoError;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }
    }
}