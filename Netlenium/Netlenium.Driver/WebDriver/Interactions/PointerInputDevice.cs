using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Netlenium.Driver.WebDriver.Internal;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Represents the origin of the coordinates for mouse movement.
    /// </summary>
    public enum CoordinateOrigin
    {
        /// <summary>
        /// The coordinate origin is the origin of the view port of the browser.
        /// </summary>
        Viewport,

        /// <summary>
        /// The origin of the movement is the most recent pointer location.
        /// </summary>
        Pointer,

        /// <summary>
        /// The origin of the movement is the center of the element specified.
        /// </summary>
        Element
    }

    /// <summary>
    /// Specifies the type of pointer a pointer device represents.
    /// </summary>
    public enum PointerKind
    {
        /// <summary>
        /// The pointer device is a mouse.
        /// </summary>
        Mouse,

        /// <summary>
        /// The pointer device is a pen or stylus.
        /// </summary>
        Pen,

        /// <summary>
        /// The pointer device is a touch screen device.
        /// </summary>
        Touch
    }

    /// <summary>
    /// Specifies the button used during a pointer down or up action.
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        /// The button used is the primary button.
        /// </summary>
        Left = 0,

        /// <summary>
        /// The button used is the middle button or mouse wheel.
        /// </summary>
        Middle = 1,

        /// <summary>
        /// The button used is the secondary button.
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// Represents a pointer input device, such as a stylus, mouse, or finger on a touch screen.
    /// </summary>
    public class PointerInputDevice : InputDevice
    {
        private PointerKind pointerKind;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointerInputDevice"/> class.
        /// </summary>
        public PointerInputDevice()
            : this(PointerKind.Mouse)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointerInputDevice"/> class.
        /// </summary>
        /// <param name="pointerKind">The kind of pointer represented by this input device.</param>
        public PointerInputDevice(PointerKind pointerKind)
            : this(pointerKind, Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointerInputDevice"/> class.
        /// </summary>
        /// <param name="pointerKind">The kind of pointer represented by this input device.</param>
        /// <param name="deviceName">The unique name for this input device.</param>
        public PointerInputDevice(PointerKind pointerKind, string deviceName)
            : base(deviceName)
        {
            this.pointerKind = pointerKind;
        }

        /// <summary>
        /// Gets the type of device for this input device.
        /// </summary>
        public override InputDeviceKind DeviceKind
        {
            get { return InputDeviceKind.Pointer; }
        }

        /// <summary>
        /// Returns a value for this input device that can be transmitted across the wire to a remote end.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> representing this action.</returns>
        public override Dictionary<string, object> ToDictionary()
        {
            var toReturn = new Dictionary<string, object>();

            toReturn["type"] = "pointer";
            toReturn["id"] = DeviceName;

            var parameters = new Dictionary<string, object>();
            parameters["pointerType"] = pointerKind.ToString().ToLowerInvariant();
            toReturn["parameters"] = parameters;

            return toReturn;
        }

        /// <summary>
        /// Creates a pointer down action.
        /// </summary>
        /// <param name="button">The button of the pointer that should be pressed.</param>
        /// <returns>The action representing the pointer down gesture.</returns>
        public Interaction CreatePointerDown(MouseButton button)
        {
            return new PointerDownInteraction(this, button);
        }

        /// <summary>
        /// Creates a pointer up action.
        /// </summary>
        /// <param name="button">The button of the pointer that should be released.</param>
        /// <returns>The action representing the pointer up gesture.</returns>
        public Interaction CreatePointerUp(MouseButton button)
        {
            return new PointerUpInteraction(this, button);
        }

        /// <summary>
        /// Creates a pointer move action to a specific element.
        /// </summary>
        /// <param name="target">The <see cref="IWebElement"/> used as the target for the move.</param>
        /// <param name="xOffset">The horizontal offset from the origin of the move.</param>
        /// <param name="yOffset">The vertical offset from the origin of the move.</param>
        /// <param name="duration">The length of time the move gesture takes to complete.</param>
        /// <returns>The action representing the pointer move gesture.</returns>
        public Interaction CreatePointerMove(IWebElement target, int xOffset, int yOffset, TimeSpan duration)
        {
            return new PointerMoveInteraction(this, target, CoordinateOrigin.Element, xOffset, yOffset, duration);
        }

        /// <summary>
        /// Creates a pointer move action to an absolute coordinate.
        /// </summary>
        /// <param name="origin">The origin of coordinates for the move. Values can be relative to
        /// the view port origin, or the most recent pointer position.</param>
        /// <param name="xOffset">The horizontal offset from the origin of the move.</param>
        /// <param name="yOffset">The vertical offset from the origin of the move.</param>
        /// <param name="duration">The length of time the move gesture takes to complete.</param>
        /// <returns>The action representing the pointer move gesture.</returns>
        /// <exception cref="ArgumentException">Thrown when passing CoordinateOrigin.Element into origin.
        /// Users should us the other CreatePointerMove overload to move to a specific element.</exception>
        public Interaction CreatePointerMove(CoordinateOrigin origin, int xOffset, int yOffset, TimeSpan duration)
        {
            if (origin == CoordinateOrigin.Element)
            {
                throw new ArgumentException("Using a value of CoordinateOrigin.Element without an element is not supported.", "origin");
            }

            return new PointerMoveInteraction(this, null, origin, xOffset, yOffset, duration);
        }

        /// <summary>
        /// Creates a pointer cancel action.
        /// </summary>
        /// <returns>The action representing the pointer cancel gesture.</returns>
        public Interaction CreatePointerCancel()
        {
            return new PointerCancelInteraction(this);
        }

        private class PointerDownInteraction : Interaction
        {
            private MouseButton button;

            public PointerDownInteraction(InputDevice sourceDevice, MouseButton button)
                : base(sourceDevice)
            {
                this.button = button;
            }

            public override Dictionary<string, object> ToDictionary()
            {
                var toReturn = new Dictionary<string, object>();
                toReturn["type"] = "pointerDown";
                toReturn["button"] = Convert.ToInt32(button, CultureInfo.InvariantCulture);

                return toReturn;
            }

            public override string ToString()
            {
                return "Pointer down";
            }
        }

        private class PointerUpInteraction : Interaction
        {
            private MouseButton button;

            public PointerUpInteraction(InputDevice sourceDevice, MouseButton button)
                : base(sourceDevice)
            {
                this.button = button;
            }

            public override Dictionary<string, object> ToDictionary()
            {
                var toReturn = new Dictionary<string, object>();
                toReturn["type"] = "pointerUp";
                toReturn["button"] = Convert.ToInt32(button, CultureInfo.InvariantCulture);

                return toReturn;
            }

            public override string ToString()
            {
                return "Pointer up";
            }
        }

        private class PointerCancelInteraction : Interaction
        {
            public PointerCancelInteraction(InputDevice sourceDevice)
                : base(sourceDevice)
            {
            }

            public override Dictionary<string, object> ToDictionary()
            {
                var toReturn = new Dictionary<string, object>();
                toReturn["type"] = "pointerCancel";
                return toReturn;
            }

            public override string ToString()
            {
                return "Pointer cancel";
            }
        }

        private class PointerMoveInteraction : Interaction
        {
            private IWebElement target;
            private int x = 0;
            private int y = 0;
            private TimeSpan duration = TimeSpan.MinValue;
            private CoordinateOrigin origin = CoordinateOrigin.Pointer;

            public PointerMoveInteraction(InputDevice sourceDevice, IWebElement target, CoordinateOrigin origin, int x, int y, TimeSpan duration)
                : base(sourceDevice)
            {
                if (target != null)
                {
                    this.target = target;
                    this.origin = CoordinateOrigin.Element;
                }
                else
                {
                    if (this.origin != CoordinateOrigin.Element)
                    {
                        this.origin = origin;
                    }
                }

                if (duration != TimeSpan.MinValue)
                {
                    this.duration = duration;
                }

                this.x = x;
                this.y = y;
            }

            public override Dictionary<string, object> ToDictionary()
            {
                var toReturn = new Dictionary<string, object>();

                toReturn["type"] = "pointerMove";
                if (duration != TimeSpan.MinValue)
                {
                    toReturn["duration"] = Convert.ToInt64(duration.TotalMilliseconds);
                }

                if (target != null)
                {
                    toReturn["origin"] = ConvertElement();
                }
                else
                {
                    toReturn["origin"] = origin.ToString().ToLowerInvariant();
                }

                toReturn["x"] = x;
                toReturn["y"] = y;

                return toReturn;
            }

            public override string ToString()
            {
                var originDescription = origin.ToString();
                if (origin == CoordinateOrigin.Element)
                {
                    originDescription = target.ToString();
                }

                return string.Format(CultureInfo.InvariantCulture, "Pointer move [origin: {0}, x offset: {1}, y offset: {2}, duration: {3}ms]", originDescription, x, y, duration.TotalMilliseconds);
            }

            private Dictionary<string, object> ConvertElement()
            {
                var elementReference = target as IWebElementReference;
                if (elementReference == null)
                {
                    var elementWrapper = target as IWrapsElement;
                    if (elementWrapper != null)
                    {
                        elementReference = elementWrapper.WrappedElement as IWebElementReference;
                    }
                }

                if (elementReference == null)
                {
                    throw new ArgumentException("Target element cannot be converted to IWebElementReference");
                }

                var elementDictionary = elementReference.ToDictionary();
                return elementDictionary;
            }
        }
    }
}
