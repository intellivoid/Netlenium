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
        /// The root request
        /// </summary>
        /// <param name="httpRequest"></param>
        public static void Root(HttpRequestEventArgs httpRequest)
        {
            WebService.SendJsonResponse(httpRequest.Response, new Responses.RootResponse(), 200);
        }


    }
}
