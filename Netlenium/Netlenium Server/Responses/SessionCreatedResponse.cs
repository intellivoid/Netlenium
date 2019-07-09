using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer.Responses
{
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
