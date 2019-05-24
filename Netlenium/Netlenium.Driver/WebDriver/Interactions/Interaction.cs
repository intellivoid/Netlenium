using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Represents a single interaction for a given input device.
    /// </summary>
    public abstract class Interaction
    {
        private InputDevice sourceDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        /// <param name="sourceDevice">The input device which performs this action.</param>
        protected Interaction(InputDevice sourceDevice)
        {
            if (sourceDevice == null)
            {
                throw new ArgumentNullException("sourceDevice", "Source device cannot be null");
            }

            this.sourceDevice = sourceDevice;
        }

        /// <summary>
        /// Gets the device for which this action is intended.
        /// </summary>
        public InputDevice SourceDevice
        {
            get { return sourceDevice; }
        }

        /// <summary>
        /// Returns a value for this action that can be transmitted across the wire to a remote end.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> representing this action.</returns>
        public abstract Dictionary<string, object> ToDictionary();

        /// <summary>
        /// Gets a value indicating whether this action is valid for the specified type of input device.
        /// </summary>
        /// <param name="sourceDeviceKind">The type of device to check.</param>
        /// <returns><see langword="true"/> if the action is valid for the specified type of input device;
        /// otherwise, <see langword="false"/>.</returns>
        public virtual bool IsValidFor(InputDeviceKind sourceDeviceKind)
        {
            return sourceDevice.DeviceKind == sourceDeviceKind;
        }
    }
}
