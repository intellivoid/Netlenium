using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer
{
    /// <summary>
    /// Response Interface for JSON Responses
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Indication that the request was successful or not 
        /// </summary>
        bool Status { get; set; }

        /// <summary>
        /// The response code of the request
        /// </summary>
        int ResponseCode { get; set; }
    }
}
