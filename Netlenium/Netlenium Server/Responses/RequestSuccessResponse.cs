using System;

namespace NetleniumServer.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the given request was a success
    /// </summary>
    [Serializable]
    public class RequestSuccessResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public RequestSuccessResponse()
        {
            Status = true;
            ResponseCode = 200;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}