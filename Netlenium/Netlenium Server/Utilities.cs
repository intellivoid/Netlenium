using Intellivoid.HyperWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer
{
    public static class Utilities
    {
        /// <summary>
        /// Returns the Paramerter value, returns null if it doesn't exist
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string GetParameter(HttpRequestEventArgs httpRequest, string parameterName)
        {
            switch (httpRequest.Request.RequestType.ToUpper())
            {
                case "GET":
                    return httpRequest.Request.QueryString.Get(parameterName);

                case "POST":
                    return httpRequest.Request.Form.Get(parameterName);

                default:
                    return null;
            }
        }
    }
}
