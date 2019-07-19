﻿using System;

namespace NetleniumServer.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Generic 404 Response
    /// </summary>
    [Serializable]
    public class NotFoundResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public NotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            Message = "The requested resource was not found";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}