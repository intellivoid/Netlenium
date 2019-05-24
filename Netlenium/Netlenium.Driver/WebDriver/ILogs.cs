using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Interface allowing handling of driver logs.
    /// </summary>
    public interface ILogs
    {
        /// <summary>
        /// Gets the list of available log types for this driver.
        /// </summary>
        ReadOnlyCollection<string> AvailableLogTypes { get; }

        /// <summary>
        /// Gets the set of <see cref="LogEntry"/> objects for a specified log.
        /// </summary>
        /// <param name="logKind">The log for which to retrieve the log entries.
        /// Log types can be found in the <see cref="LogType"/> class.</param>
        /// <returns>The list of <see cref="LogEntry"/> objects for the specified log.</returns>
        ReadOnlyCollection<LogEntry> GetLog(string logKind);
    }
}
