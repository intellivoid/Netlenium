using System;

namespace NetleniumServer.Responses
{
    [Serializable]
    public class SessionCreatedResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string SessionID { get; set; }

        public SessionCreatedResponse(string sessionId)
        {
            Status = true;
            ResponseCode = 200;
            SessionID = sessionId;
        }
    }
}
