using Intellivoid.HyperWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer
{
    public static class APIHandler
    {
        /// <summary>
        /// Creates a new Driver Session
        /// </summary>
        /// <param name="httpRequest"></param>
        public static void CreateSession(HttpRequestEventArgs httpRequest)
        {
            WebService.SendJsonResponse();
        }


    }
}
