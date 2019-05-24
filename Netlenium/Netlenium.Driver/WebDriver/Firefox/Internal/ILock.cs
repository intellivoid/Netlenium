using System;

namespace Netlenium.Driver.WebDriver.Firefox.Internal
{
    /// <summary>
    /// Defines the interface through which the mutex port for establishing communication
    /// with the WebDriver extension can be locked.
    /// </summary>
    internal interface ILock : IDisposable
    {
        /// <summary>
        /// Locks the mutex port.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> describing the amount of time to wait for
        /// the mutex port to become available.</param>
        void LockObject(TimeSpan timeout);

        /// <summary>
        /// Unlocks the mutex port.
        /// </summary>
        void UnlockObject();
    }
}
