using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the requested driver is disabled
    /// </summary>
    [Serializable]
    public class DriverDisabledResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="targetBrowser"></param>
        public DriverDisabledResponse(string targetBrowser)
        {
            Status = false;
            ResponseCode = 403;
            ErrorCode = ErrorCode.DriverDisabled;
            Message = $"The driver for '{targetBrowser}' is disabled";
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }
        
    }
}