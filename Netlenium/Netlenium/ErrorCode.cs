namespace Netlenium
{
    /// <summary>
    /// List of Error Codes
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// No Erorr has been returned
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Unknown Undocumented Error
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// This error was unexpected, could be a bug
        /// </summary>
        UnexpectedError = 100,

        /// <summary>
        /// The attribute was not found in the element
        /// </summary>
        AttributeNotFound = 101,

        /// <summary>
        /// The given proxy scheme is not valid
        /// </summary>
        InvalidProxyScheme = 102,

        /// <summary>
        /// The given driver is not supported in Netlenium
        /// </summary>
        UnsupportedDriver = 103,

        /// <summary>
        /// The method used for the HTTP request is unsupported
        /// </summary>
        UnsupportedRequestMethod = 104,

        /// <summary>
        /// The session has expired
        /// </summary>
        SessionExpired = 105,

        /// <summary>
        /// The window handler was not found
        /// </summary>
        WindowHandlerNotFound = 106,

        /// <summary>
        /// There was a error with the session
        /// </summary>
        SessionError = 107,

        /// <summary>
        /// The session was not found
        /// </summary>
        SessionNotFound = 108,

        /// <summary>
        /// The user is not authorized to make this request
        /// </summary>
        Unauthorized = 109,

        /// <summary>
        /// The maximum sessions allowed has been reached
        /// </summary>
        TooManySessions = 110,

        /// <summary>
        /// The requested resource was not found
        /// </summary>
        ResourceNotFound = 111,

        /// <summary>
        /// The Javascript code that was executed threw an error
        /// </summary>
        JavascriptExecutionError = 112,

        /// <summary>
        /// A required parameter was missing
        /// </summary>
        MissingParameter = 113,

        /// <summary>
        /// The given search value for 'by' is invalid.
        /// </summary>
        InvalidSearchValue = 114,

        /// <summary>
        /// The driver was disabled by the administrator
        /// </summary>
        DriverDisabled = 115,
        
        /// <summary>
        /// The element was not found
        /// </summary>
        ElementNotFound = 116
    }
}
