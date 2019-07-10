using System;

namespace NetleniumServer.Responses
{
    [Serializable]
    public class UnsupportedRequestMethodResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        public UnsupportedRequestMethodResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The request method used is not supported for this Web Service";
        }
    }
}
