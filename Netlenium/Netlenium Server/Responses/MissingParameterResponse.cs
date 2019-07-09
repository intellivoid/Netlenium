using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer.Responses
{
    [Serializable]
    public class MissingParameterResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        public MissingParameterResponse(string parameterName)
        {
            Status = false;
            ResponseCode = 400;
            Message = $"The required parameter '{parameterName}' is missing";
        }
    }
}
