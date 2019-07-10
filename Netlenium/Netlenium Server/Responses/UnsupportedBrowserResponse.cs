using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// A response when the given target browser is unsupported
    /// </summary>
    [Serializable]
    public class UnsupportedBrowserResponse : IResponse

    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public UnsupportedBrowserResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The given browser is unsupported";
        }
    }
}
