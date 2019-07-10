using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response for when a paramerter is missing
    /// </summary>
    [Serializable]
    public class MissingParameterResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Consturctor
        /// </summary>
        /// <param name="parameterName"></param>
        public MissingParameterResponse(string parameterName)
        {
            Status = false;
            ResponseCode = 400;
            Message = $"The required parameter '{parameterName}' is missing";
        }
    }
}
