using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     A response when the given target browser is unsupported
    /// </summary>
    [Serializable]
    public class UnsupportedDriverResponse : IResponse

    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public UnsupportedDriverResponse()
        {
            Status = false;
            ResponseCode = 400;
            ErrorCode = ErrorCode.UnsupportedDriver;
            Message = "The given driver is unsupported";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }
    }
}