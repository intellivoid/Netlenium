﻿using System;

namespace Netlenium.Responses
{
    /// <summary>
    /// Response given when a unexpected error raises
    /// </summary>
    public class InternalServerErrorResponse : IResponse
    {

        /// <summary>
        /// Public Consturctor
        /// </summary>
        /// <param name="exception"></param>
        public InternalServerErrorResponse(Exception exception)
        {
            Status = false;
            ResponseCode = 500;
            ErrorCode = ErrorCode.UnexpectedError;
            Error = exception.Message;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Error { get; set; }
    }
}
