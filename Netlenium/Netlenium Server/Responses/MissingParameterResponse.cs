using System;

namespace NetleniumServer.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response for when a parameter is missing
    /// </summary>
    [Serializable]
    public class MissingParameterResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="parameterName"></param>
        public MissingParameterResponse(string parameterName)
        {
            Status = false;
            ResponseCode = 400;
            Message = $"The required parameter '{parameterName}' is missing";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}