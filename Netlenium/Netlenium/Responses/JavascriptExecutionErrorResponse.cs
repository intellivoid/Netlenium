using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the Javascript execution failed
    /// </summary>
    [Serializable]
    public class JavascriptExecutionErrorResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="exceptionMessage"></param>
        public JavascriptExecutionErrorResponse(string exceptionMessage)
        {
            Status = false;
            ResponseCode = 500;
            ErrorCode = ErrorCode.JavascriptExecutionError;
            Error = exceptionMessage;
        }

        public string Error { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }
    }
}