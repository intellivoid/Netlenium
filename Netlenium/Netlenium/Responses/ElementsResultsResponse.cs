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
            Elements = new List<WebElement>();
        }

        public List<WebElement> Elements { get; set; }
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}