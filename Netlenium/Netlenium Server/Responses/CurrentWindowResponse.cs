using NetleniumServer.Objects;
using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given to return the details of the current window
    /// </summary>
    [Serializable]
    public class CurrentWindowResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public CurrentWindow CurrentWindow { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="currentWindow"></param>
        public CurrentWindowResponse(CurrentWindow currentWindow)
        {
            Status = true;
            ResponseCode = 200;
            CurrentWindow = currentWindow;
        }
    }
}
