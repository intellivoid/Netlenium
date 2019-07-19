using System;

namespace NetleniumServer.Responses
{
    /// <inheritdoc />
    /// <summary>
    ///     Generic Response for the root request
    /// </summary>
    [Serializable]
    public class RootResponse : IResponse
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        public RootResponse()
        {
            Status = true;
            ResponseCode = 200;
            ServerName = CommandLineParameters.ServerName;
        }

        public string ServerName { get; set; }
        
        public bool Status { get; set; }

        public int ResponseCode { get; set; }
    }
}