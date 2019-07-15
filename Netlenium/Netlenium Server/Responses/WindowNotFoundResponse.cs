﻿using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when the Window was not found
    /// </summary>
    [Serializable]
    public class WindowNotFoundResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode  { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public WindowNotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            Message = "The window handler was not found";
        }
    }
}