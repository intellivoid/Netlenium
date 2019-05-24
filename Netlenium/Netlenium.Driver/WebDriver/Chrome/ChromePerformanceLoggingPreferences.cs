using System;
using System.Collections.Generic;

namespace Netlenium.Driver.WebDriver.Chrome
{
    /// <summary>
    /// Represents the type-safe options for setting preferences for performance
    /// logging in the Chrome browser.
    /// </summary>
    public class ChromePerformanceLoggingPreferences
    {
        private bool isCollectingNetworkEvents = true;
        private bool isCollectingPageEvents = true;
        private TimeSpan bufferUsageReportingInterval = TimeSpan.FromMilliseconds(1000);
        private List<string> tracingCategories = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether Chrome will collect events from the Network domain.
        /// Defaults to <see langword="true"/>.
        /// </summary>
        public bool IsCollectingNetworkEvents
        {
            get { return isCollectingNetworkEvents; }
            set { isCollectingNetworkEvents = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Chrome will collect events from the Page domain.
        /// Defaults to <see langword="true"/>.
        /// </summary>
        public bool IsCollectingPageEvents
        {
            get { return isCollectingPageEvents; }
            set { isCollectingPageEvents = value; }
        }

        /// <summary>
        /// Gets or sets the interval between Chrome DevTools trace buffer usage events.
        /// Defaults to 1000 milliseconds.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when an attempt is made to set
        /// the value to a time span of less tnan or equal to zero milliseconds.</exception>
        public TimeSpan BufferUsageReportingInterval
        {
            get
            {
                return bufferUsageReportingInterval;
            }

            set
            {
                if (value.TotalMilliseconds <= 0)
                {
                    throw new ArgumentException("Interval must be greater than zero.");
                }

                bufferUsageReportingInterval = value;
            }
        }

        /// <summary>
        /// Gets a comma-separated list of the categories for which tracing is enabled.
        /// </summary>
        public string TracingCategories
        {
            get
            {
                if (tracingCategories.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(",", tracingCategories.ToArray());
            }
        }

        /// <summary>
        /// Adds a single category to the list of Chrome tracing categories for which events should be collected.
        /// </summary>
        /// <param name="category">The category to add.</param>
        public void AddTracingCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentException("category must not be null or empty", "category");
            }

            AddTracingCategories(category);
        }

        /// <summary>
        /// Adds categories to the list of Chrome tracing categories for which events should be collected.
        /// </summary>
        /// <param name="categoriesToAdd">An array of categories to add.</param>
        public void AddTracingCategories(params string[] categoriesToAdd)
        {
            AddTracingCategories(new List<string>(categoriesToAdd));
        }

        /// <summary>
        /// Adds categories to the list of Chrome tracing categories for which events should be collected.
        /// </summary>
        /// <param name="categoriesToAdd">An <see cref="IEnumerable{T}"/> object of categories to add.</param>
        public void AddTracingCategories(IEnumerable<string> categoriesToAdd)
        {
            if (categoriesToAdd == null)
            {
                throw new ArgumentNullException("categoriesToAdd", "categoriesToAdd must not be null");
            }

            // Adding a tracing category automatically turns timeline events off.
            tracingCategories.AddRange(categoriesToAdd);
        }
    }
}
