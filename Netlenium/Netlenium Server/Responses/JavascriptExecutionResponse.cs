using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given after a succesful Javascript Execution
    /// </summary>
    [Serializable]
    public class JavascriptExecutionResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Output { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="output"></param>
        public JavascriptExecutionResponse(string output)
        {
            Status = true;
            ResponseCode = 200;
            Output = output;
        }
    }
}
