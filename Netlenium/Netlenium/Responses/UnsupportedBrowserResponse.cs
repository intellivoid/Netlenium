using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     A response when the given target browser is unsupported
    /// </summary>
    [Serializable]
    public class UnsupportedBrowserResponse : IResponse

    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public UnsupportedBrowserResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The given browser is unsupported";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}