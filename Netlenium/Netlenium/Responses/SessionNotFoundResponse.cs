using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the Session was not found
    /// </summary>
    [Serializable]
    public class SessionNotFoundResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="sessionId"></param>
        public SessionNotFoundResponse(string sessionId)
        {
            Status = false;
            ResponseCode = 404;
            ErrorCode = ErrorCode.SessionNotFound;
            Message = $"The session '{sessionId}' was not found";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }
    }
}