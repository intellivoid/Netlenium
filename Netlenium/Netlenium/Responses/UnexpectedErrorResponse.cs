using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when there was an unexpected error
    /// </summary>
    [Serializable]
    public class UnexpectedErrorResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="exceptionMessage"></param>
        public UnexpectedErrorResponse(string exceptionMessage)
        {
            Status = false;
            ResponseCode = 500;
            Message = exceptionMessage;
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}