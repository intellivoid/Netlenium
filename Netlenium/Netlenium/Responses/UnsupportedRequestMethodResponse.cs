using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     This response is returned when the given Request Method is not supported on this Web Service
    /// </summary>
    [Serializable]
    public class UnsupportedRequestMethodResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public UnsupportedRequestMethodResponse()
        {
            Status = false;
            ResponseCode = 400;
            ErrorCode = ErrorCode.UnsupportedRequestMethod;
            Message = "The request method used is not supported for this Web Service";
        }
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }

    }
}