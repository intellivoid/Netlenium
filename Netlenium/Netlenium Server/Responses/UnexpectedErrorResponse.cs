using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when there was an unexpected error
    /// </summary>
    [Serializable]
    public class UnexpectedErrorResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="exceptionMessage"></param>
        public UnexpectedErrorResponse(string exceptionMessage)
        {
            Status = false;
            ResponseCode = 500;
            Message = exceptionMessage;
        }
    }
}
