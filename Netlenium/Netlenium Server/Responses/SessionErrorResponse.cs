using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when there is a session error
    /// </summary>
    [Serializable]
    public class SessionErrorResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="error"></param>
        public SessionErrorResponse(string error)
        {
            Status = true;
            ResponseCode = 500;
            Message = error;
        }
    }
}
