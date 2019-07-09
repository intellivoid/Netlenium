using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer.Responses
{
    [Serializable]
    public class NotFoundResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        public NotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            Message = "The requested resource was not found";
        }
    }
}
