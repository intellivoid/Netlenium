using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Generic 404 Response
    /// </summary>
    [Serializable]
    public class NotFoundResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public NotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            Message = "The requested resource was not found";
        }
    }
}
