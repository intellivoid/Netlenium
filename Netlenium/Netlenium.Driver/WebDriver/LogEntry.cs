using System;
using System.Collections.Generic;
using System.Globalization;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Represents an entry in a log from a driver instance.
    /// </summary>
    public class LogEntry
    {
        private LogLevel level = LogLevel.All;
        private DateTime timestamp = DateTime.MinValue;
        private string message = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        private LogEntry()
        {
        }

        /// <summary>
        /// Gets the timestamp value of the log entry.
        /// </summary>
        public DateTime Timestamp
        {
            get { return timestamp; }
        }

        /// <summary>
        /// Gets the logging level of the log entry.
        /// </summary>
        public LogLevel Level
        {
            get { return level; }
        }

        /// <summary>
        /// Gets the message of the log entry.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="LogEntry"/>.
        /// </summary>
        /// <returns>A string that represents the current <see cref="LogEntry"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:yyyy-MM-ddTHH:mm:ssZ}] [{1}] {2}", timestamp, level, message);
        }

        /// <summary>
        /// Creates a <see cref="LogEntry"/> from a dictionary as deserialized from JSON.
        /// </summary>
        /// <param name="entryDictionary">The <see cref="Dictionary{TKey, TValue}"/> from
        /// which to create the <see cref="LogEntry"/>.</param>
        /// <returns>A <see cref="LogEntry"/> with the values in the dictionary.</returns>
        internal static LogEntry FromDictionary(Dictionary<string, object> entryDictionary)
        {
            var entry = new LogEntry();
            if (entryDictionary.ContainsKey("message"))
            {
                entry.message = entryDictionary["message"].ToString();
            }

            if (entryDictionary.ContainsKey("timestamp"))
            {
                var zeroDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var timestampValue = Convert.ToDouble(entryDictionary["timestamp"], CultureInfo.InvariantCulture);
                entry.timestamp = zeroDate.AddMilliseconds(timestampValue);
            }

            if (entryDictionary.ContainsKey("level"))
            {
                var levelValue = entryDictionary["level"].ToString();
                try
                {
                    entry.level = (LogLevel)Enum.Parse(typeof(LogLevel), levelValue, true);
                }
                catch (ArgumentException)
                {
                    // If the requested log level string is not a valid log level,
                    // ignore it and use LogLevel.All.
                }
            }

            return entry;
        }
    }
}
