using System;
using System.Collections.Generic;
using Netlenium.Objects;

namespace Netlenium.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Response given to display a list of Elements
    /// </summary>
    [Serializable]
    public class ElementsResultsResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public ElementsResultsResponse()
        {
            Status = true;
            ResponseCode = 200;
            ErrorCode = ErrorCode.NoError;
            Elements = new List<WebElement>();
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public List<WebElement> Elements { get; set; }
    }
}