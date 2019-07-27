using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given after a successful Javascript Execution
    /// </summary>
    [Serializable]
    public class JavascriptExecutionResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="output"></param>
        public JavascriptExecutionResponse(string output)
        {
            Status = true;
            ResponseCode = 200;
            ErrorCode = ErrorCode.NoError;
            Output = output;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Output { get; set; }
    }
}