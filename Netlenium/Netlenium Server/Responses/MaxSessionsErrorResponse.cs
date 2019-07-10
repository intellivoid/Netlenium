using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// A Response for when the maximum sessions are maxed out
    /// </summary>
    class MaxSessionsErrorResponse : IResponse
    {
        public bool Status { get; set;  }

        public int ResponseCode { get; set;  }

        public string Message { get; set; }

        /// <summary>
        /// Public Consturctor
        /// </summary>
        public MaxSessionsErrorResponse()
        {
            Status = false;
            ResponseCode = 403;
            Message = $"A maximum of {CommandLineParameters.MaxSessions} session(s) are allowed to be created";
        }
    }
}
