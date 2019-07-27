namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     A Response for when the maximum sessions are maxed out
    /// </summary>
    internal class MaxSessionsErrorResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public MaxSessionsErrorResponse()
        {
            Status = false;
            ResponseCode = 403;
            ErrorCode = ErrorCode.TooManySessions;
            Message = $"A maximum of {CommandLineParameters.MaxSessions} session(s) are allowed to be created";
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }
    }
}