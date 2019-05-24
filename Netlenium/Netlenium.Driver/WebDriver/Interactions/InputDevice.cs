using System;
using System.Collections.Generic;
using System.Globalization;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Base class for all input devices for actions.
    /// </summary>
    public abstract class InputDevice
    {
        private string deviceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDevice"/> class.
        /// </summary>
        /// <param name="deviceName">The unique name of the input device represented by this class.</param>
        protected InputDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                throw new ArgumentException("Device name must not be null or empty", "deviceName");
            }

            this.deviceName = deviceName;
        }

        /// <summary>
        /// Gets the unique name of this input device.
        /// </summary>
        public string DeviceName
        {
            get { return deviceName; }
        }

        /// <summary>
        /// Gets the kind of device for this input device.
        /// </summary>
        public abstract InputDeviceKind DeviceKind { get; }

        /// <summary>
        /// Returns a value for this input device that can be transmitted across the wire to a remote end.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> representing this action.</returns>
        public abstract Dictionary<string, object> ToDictionary();

        /// <summary>
        /// Creates a pause action for synchronization with other action sequences.
        /// </summary>
        /// <returns>The <see cref="Interaction"/> representing the action.</returns>
        public Interaction CreatePause()
        {
            return CreatePause(TimeSpan.Zero);
        }

        /// <summary>
        /// Creates a pause action for synchronization with other action sequences.
        /// </summary>
        /// <param name="duration">A <see cref="TimeSpan"/> representing the duration
        /// of the pause. Note that <see cref="TimeSpan.Zero"/> pauses to synchronize
        /// with other action sequences for other devices.</param>
        /// <returns>The <see cref="Interaction"/> representing the action.</returns>
        public Interaction CreatePause(TimeSpan duration)
        {
            return new PauseInteraction(this, duration);
        }

        /// <summary>
        /// Returns a hash code for the current <see cref="InputDevice"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="InputDevice"/>.</returns>
        public override int GetHashCode()
        {
            return deviceName.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="InputDevice"/>.
        /// </summary>
        /// <returns>A string that represents the current <see cref="InputDevice"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} input device [name: {1}]", DeviceKind, deviceName);
        }
    }
}
