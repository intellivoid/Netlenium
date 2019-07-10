using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer.Responses
{
    [Serializable]
    public class UnsupportedBrowserResponse : IResponse

    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        public UnsupportedBrowserResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The given browser is unsupported";
        }
    }
}
