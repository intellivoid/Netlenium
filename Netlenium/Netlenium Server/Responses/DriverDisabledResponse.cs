using System;

namespace NetleniumServer.Responses
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
            Message = $"The driver for '{targetBrowser}' is disabled";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}