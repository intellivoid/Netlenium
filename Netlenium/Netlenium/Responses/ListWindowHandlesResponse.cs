﻿using System;
using System.Collections.Generic;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response for listing all the Window Handles that are currently available
    /// </summary>
    [Serializable]
    public class ListWindowHandlesResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="windowHandles"></param>
        public ListWindowHandlesResponse(List<string> windowHandles)
        {
            Status = true;
            ResponseCode = 200;
            ErrorCode = ErrorCode.NoError;
            WindowHandles = windowHandles;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public List<string> WindowHandles { get; set; }
    }
}