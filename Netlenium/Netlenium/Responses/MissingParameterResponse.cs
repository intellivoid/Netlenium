using System;

namespace Netlenium.Responses
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
            ErrorCode = ErrorCode.MissingParameter;
            Message = $"The required parameter '{parameterName}' is missing";
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }
    }
}