using System;

namespace NetleniumServer.Responses
{
    /// <summary>
    /// Generic Response for the root request
    /// </summary>
    [Serializable]
    public class RootResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string ServerName { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public RootResponse()
        {
            Status = true;
            ResponseCode = 200;
            ServerName = CommandLineParameters.ServerName;
        }
    }
}
