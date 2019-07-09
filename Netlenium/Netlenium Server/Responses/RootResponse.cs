using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer.Responses
{
    [Serializable]
    public class RootResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string ServerName { get; set; }

        public RootResponse()
        {
            Status = true;
            ResponseCode = 200;
            ServerName = "Netlenium Server";
        }
    }
}
