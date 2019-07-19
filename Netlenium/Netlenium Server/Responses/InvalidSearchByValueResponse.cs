using System;

namespace NetleniumServer.Responses
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
            Message = "The given value for 'by' is invalid";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}