using System;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given for the value of an attribute from a element
    /// </summary>
    [Serializable]
    public class AttributeValueResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="attributeValue"></param>
        public AttributeValueResponse(string attributeValue)
        {
            Status = true;
            ResponseCode = 200;
            ErrorCode = ErrorCode.NoError;
            AttributeValue = attributeValue;
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string AttributeValue { get; set; }
    }
}