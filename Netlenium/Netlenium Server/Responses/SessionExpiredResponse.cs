using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// A response given when the session has expired
    /// </summary>
    [Serializable]
    public class SessionExpiredResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public SessionExpiredResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The session has expired";
        }
    }
}
