﻿using System;
using Netlenium.Objects;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given to return the details of the current window
    /// </summary>
    [Serializable]
    public class CurrentWindowResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="currentWindow"></param>
        public CurrentWindowResponse(CurrentWindow currentWindow)
        {
            Status = true;
            ResponseCode = 200;
            CurrentWindow = currentWindow;
        }

        public CurrentWindow CurrentWindow { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}