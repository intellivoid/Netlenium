using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Represents a key input device, such as a keyboard.
    /// </summary>
    public class KeyInputDevice : InputDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInputDevice"/> class.
        /// </summary>
        public KeyInputDevice()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInputDevice"/> class, given the device's name.
        /// </summary>
        /// <param name="deviceName">The unique name of this input device.</param>
        public KeyInputDevice(string deviceName)
            : base(deviceName)
        {
        }

        /// <summary>
        /// Gets the type of device for this input device.
        /// </summary>
        public override InputDeviceKind DeviceKind
        {
            get { return InputDeviceKind.Key; }
        }

        /// <summary>
        /// Converts this input device into an object suitable for serializing across the wire.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> representing this input device.</returns>
        public override Dictionary<string, object> ToDictionary()
        {
            var toReturn = new Dictionary<string, object>();

            toReturn["type"] = "key";
            toReturn["id"] = DeviceName;

            return toReturn;
        }

        /// <summary>
        /// Creates a key-down action for simulating a press of a key.
        /// </summary>
        /// <param name="codePoint">The unicode character to be sent.</param>
        /// <returns>The <see cref="Interaction"/> representing the action.</returns>
        public Interaction CreateKeyDown(char codePoint)
        {
            return new KeyDownInteraction(this, codePoint);
        }

        /// <summary>
        /// Creates a key-up action for simulating a release of a key.
        /// </summary>
        /// <param name="codePoint">The unicode character to be sent.</param>
        /// <returns>The <see cref="Interaction"/> representing the action.</returns>
        public Interaction CreateKeyUp(char codePoint)
        {
            return new KeyUpInteraction(this, codePoint);
        }

        private class KeyDownInteraction : TypingInteraction
        {
            public KeyDownInteraction(InputDevice sourceDevice, char codePoint)
                : base(sourceDevice, "keyDown", codePoint)
            {
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "Key down [key: {0}]", Keys.GetDescription(Value));
            }
        }

        private class KeyUpInteraction : TypingInteraction
        {
            public KeyUpInteraction(InputDevice sourceDevice, char codePoint)
                : base(sourceDevice, "keyUp", codePoint)
            {
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "Key up [key: {0}]", Keys.GetDescription(Value));
            }
        }

        private class TypingInteraction : Interaction
        {
            private string type;
            private string value;

            public TypingInteraction(InputDevice sourceDevice, string type, char codePoint)
                : base(sourceDevice)
            {
                this.type = type;
                value = codePoint.ToString();
            }

            protected string Value
            {
                get { return value; }
            }

            public override Dictionary<string, object> ToDictionary()
            {
                var toReturn = new Dictionary<string, object>();

                toReturn["type"] = type;
                toReturn["value"] = value;

                return toReturn;
            }
        }
    }
}
