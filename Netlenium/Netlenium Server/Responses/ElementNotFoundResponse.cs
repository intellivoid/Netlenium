using System;

namespace NetleniumServer.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the element was not found
    /// </summary>
    [Serializable]
    public class ElementNotFoundResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public ElementNotFoundResponse()
        {
            Status = false;
            ResponseCode = 404;
            Message = "The element was not found";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}