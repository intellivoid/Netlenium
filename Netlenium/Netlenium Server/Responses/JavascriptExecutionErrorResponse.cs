using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when the Javascript execution failed
    /// </summary>
    [Serializable]
    public class JavascriptExecutionErrorResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Error { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="exceptionMessage"></param>
        public JavascriptExecutionErrorResponse(string exceptionMessage)
        {
            Status = false;
            ResponseCode = 500;
            Error = exceptionMessage;
        }
    }
}
