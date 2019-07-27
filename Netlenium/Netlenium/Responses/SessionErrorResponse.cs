using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when there is a session error
    /// </summary>
    [Serializable]
    public class SessionErrorResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="error"></param>
        public SessionErrorResponse(string error)
        {
            Status = true;
            ResponseCode = 500;
            ErrorCode = ErrorCode.SessionError;
            Message = error;
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }
    }
}