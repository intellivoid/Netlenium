namespace Netlenium
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

        /// <summary>
        /// The error code that is returned
        /// </summary>
        ErrorCode ErrorCode { get; set; }
    }
}
