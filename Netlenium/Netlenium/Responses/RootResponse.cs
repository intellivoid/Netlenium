using System;
using Netlenium.Objects;

namespace Netlenium.Responses
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
            ServerDetails = new ServerDetails();
        }

        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public ServerDetails ServerDetails { get; set; }
    }
}