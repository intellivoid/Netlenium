using System;

namespace Netlenium.Responses
{
    /// <summary>
    /// A Response given when the given scheme for the proxy configuration is unsupported
    /// </summary>
    [Serializable]
    public class InvalidProxySchemeResponse : IResponse
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        public InvalidProxySchemeResponse()
        {
            Status = false;
            ResponseCode = 400;
            ErrorCode = ErrorCode.InvalidProxyScheme;
            Message = "The given proxy scheme is unsupported";
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }

    }
}
