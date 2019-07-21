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
            Message = "The request method used is not supported for this Web Service";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}