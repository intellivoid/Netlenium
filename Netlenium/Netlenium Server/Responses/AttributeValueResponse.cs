using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given for the value of an attribute from a element
    /// </summary>
    [Serializable]
    public class AttributeValueResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string AttributeValue { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="attributeValue"></param>
        public AttributeValueResponse(string attributeValue)
        {
            Status = true;
            ResponseCode = 200;
            AttributeValue = attributeValue;
        }
    }
}
