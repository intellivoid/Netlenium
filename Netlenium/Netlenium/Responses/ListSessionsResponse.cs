using System.Collections.Generic;

namespace Netlenium.Responses
{
    /// <summary>
    /// Response given to list all Active Sessions
    /// </summary>
    public class ListSessionsResponse : IResponse
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        public ListSessionsResponse()
        {
            Status = true;
            ResponseCode = 200;
            ErrorCode = ErrorCode.NoError;
            Sessions = new List<Objects.Session>();

            if(SessionManager.TotalActiveSessions > 0)
            {
                foreach(var Session in SessionManager.ActiveSessions)
                {
                    Sessions.Add(new Objects.Session(Session.Value));
                }
            }
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public List<Objects.Session> Sessions { get; set; }
    }
}
