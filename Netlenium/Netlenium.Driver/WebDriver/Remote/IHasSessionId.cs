namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Interface indicating the driver has a Session ID.
    /// </summary>
    public interface IHasSessionId
    {
        /// <summary>
        /// Gets the session ID of the current session.
        /// </summary>
        SessionId SessionId { get; }
    }
}
