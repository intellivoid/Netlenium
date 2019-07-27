using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     This response is returned when the given SearchBy Paramerter is invalid
    /// </summary>
    [Serializable]
    public class InvalidSearchByValueResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public InvalidSearchByValueResponse()
        {
            Status = false;
            ResponseCode = 400;
            ErrorCode = ErrorCode.InvalidSearchValue;
            Message = "The given value for 'by' is invalid";
        }
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string Message { get; set; }
    }
}