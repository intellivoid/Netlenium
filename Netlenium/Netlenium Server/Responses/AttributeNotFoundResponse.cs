using System;

namespace NetleniumServer.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given when the attribute was not found in the WebElement
    /// </summary>
    [Serializable]
    public class AttributeNotFoundResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="attributeName"></param>
        public AttributeNotFoundResponse(string attributeName)
        {
            Status = false;
            ResponseCode = 404;
            Message = $"The attribute '{attributeName}' was not found in the element";
        }

        public string Message { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}