using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when the given request was a success
    /// </summary>
    [Serializable]
    public class RequestSuccessResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public RequestSuccessResponse()
        {
            Status = true;
            ResponseCode = 200;
        }
    }
}
