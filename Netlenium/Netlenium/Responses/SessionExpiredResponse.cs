using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     A response given when the session has expired
    /// </summary>
    [Serializable]
    public class SessionExpiredResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public SessionExpiredResponse()
        {
            Status = false;
            ResponseCode = 400;
            ErrorCode = ErrorCode.SessionExpired;
            Message = "The session has expired";
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }

    }
}