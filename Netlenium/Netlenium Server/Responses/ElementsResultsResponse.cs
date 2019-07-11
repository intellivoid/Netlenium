using System;
using System.Collections.Generic;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Response given to display a list of Elements
    /// </summary>
    [Serializable]
    public class ElementsResultsResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public List<Objects.WebElement> Elements { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public ElementsResultsResponse()
        {
            Elements = new List<Objects.WebElement>();
        }
    }
}
