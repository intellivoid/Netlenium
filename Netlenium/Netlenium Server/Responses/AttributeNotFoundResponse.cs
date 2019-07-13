using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given when the attribute was not found in the WebElement
    /// </summary>
    [Serializable]
    public class AttributeNotFoundResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="attributeName"></param>
        public AttributeNotFoundResponse(string attributeName)
        {
            Status = false;
            ResponseCode = 404;
            Message = $"The attribute '{attributeName}' was not found in the element";
        }
    }
}
