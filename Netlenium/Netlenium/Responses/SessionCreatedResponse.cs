﻿using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response for when a session was created, returning the session ID
    /// </summary>
    [Serializable]
    public class SessionCreatedResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="sessionId"></param>
        public SessionCreatedResponse(string sessionId)
        {
            Status = true;
            ResponseCode = 200;
            ErrorCode = ErrorCode.NoError;
            SessionId = sessionId;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string SessionId { get; set; }
    }
}