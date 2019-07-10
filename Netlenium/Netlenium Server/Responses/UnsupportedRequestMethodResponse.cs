using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// This response is returned when the given Request Method is not supported on this Web Service
    /// </summary>
    [Serializable]
    public class UnsupportedRequestMethodResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public UnsupportedRequestMethodResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The request method used is not supported for this Web Service";
        }
    }
}
