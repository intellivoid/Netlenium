using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Returned when the given request is unauthorized
    /// </summary>
    [Serializable]
    class UnauthorizedRequestResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public UnauthorizedRequestResponse()
        {
            Status = false;
            ResponseCode = 401;
            Message = "Unauthorized Request";
        }
    }
}
