using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when the element was not found
    /// </summary>
    [Serializable]
    public class ElementNotFoundResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public ElementNotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            Message = "The element was not found";
        }
    }
}
