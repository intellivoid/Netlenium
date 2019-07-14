using System;
using System.Collections.Generic;


namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response for listing all the Window Handles that are currently available
    /// </summary>
    [Serializable]
    public class ListWindowHandlesResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public List<string> WindowHandles { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="windowHandles"></param>
        public ListWindowHandlesResponse(List<string> windowHandles)
        {
            Status = true;
            ResponseCode = 200;
            WindowHandles = windowHandles;
        }
    }
}
