using System;
namespace NetleniumServer.Responses
{
    /// <summary>
    /// This response is returned when the given SearchBy Paramerter is invalid
    /// </summary>
    [Serializable]
    class InvalidSearchByValueResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Consturctor
        /// </summary>
        public InvalidSearchByValueResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The given value for 'by' is invalid";
        }
    }
}
