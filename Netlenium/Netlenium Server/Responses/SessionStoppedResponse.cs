using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when the Session was stopped without any errors
    /// </summary>
    [Serializable]
    public class SessionStoppedResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public SessionStoppedResponse()
        {
            Status = true;
            ResponseCode = 200;
        }
    }
}
