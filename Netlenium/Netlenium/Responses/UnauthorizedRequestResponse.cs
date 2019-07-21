using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Returned when the given request is unauthorized
    /// </summary>
    [Serializable]
    internal class UnauthorizedRequestResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public UnauthorizedRequestResponse()
        {
            Status = false;
            ResponseCode = 401;
            Message = "Unauthorized Request";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}