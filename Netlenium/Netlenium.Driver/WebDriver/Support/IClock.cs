using System;

namespace Netlenium.Driver.WebDriver.Support.UI
{
    /// <summary>
    /// An interface describing time handling functions for timeouts.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Gets the current date and time values.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at a specified offset in the future.
        /// </summary>
        /// <param name="delay">The offset to use.</param>
        /// <returns>The <see cref="DateTime"/> at the specified offset in the future.</returns>
        DateTime LaterBy(TimeSpan delay);

        /// <summary>
        /// Gets a value indicating whether the current date and time is before the specified date and time.
        /// </summary>
        /// <param name="otherDateTime">The date and time values to compare the current date and time values to.</param>
        /// <returns><see langword="true"/> if the current date and time is before the specified date and time; otherwise, <see langword="false"/>.</returns>
        bool IsNowBefore(DateTime otherDateTime);
    }
}