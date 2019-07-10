using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response for when a session was created, returning the session ID
    /// </summary>
    [Serializable]
    public class SessionCreatedResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string SessionID { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="sessionId"></param>
        public SessionCreatedResponse(string sessionId)
        {
            Status = true;
            ResponseCode = 200;
            SessionID = sessionId;
        }
    }
}
