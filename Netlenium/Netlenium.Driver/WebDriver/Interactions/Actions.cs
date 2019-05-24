using System;
using System.Collections.Generic;
using System.Drawing;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Provides values that indicate from where element offsets for MoveToElement
    /// are calculated.
    /// </summary>
    public enum MoveToElementOffsetOrigin
    {
        /// <summary>
        /// Offsets are calculated from the top-left corner of the element.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Offsets are calcuated from the center of the element.
        /// </summary>
        Center
    }

    /// <summary>
    /// Provides a mechanism for building advanced interactions with the browser.
    /// </summary>
    public class Actions : IAction
    {
        private readonly TimeSpan DefaultMouseMoveDuration = TimeSpan.FromMilliseconds(250);
        private ActionBuilder actionBuilder = new ActionBuilder();
        private PointerInputDevice defaultMouse = new PointerInputDevice(PointerKind.Mouse, "default mouse");
        private KeyInputDevice defaultKeyboard = new KeyInputDevice("default keyboard");

        private IKeyboard keyboard;
        private IMouse mouse;
        private IActionExecutor actionExecutor;
        private CompositeAction action = new CompositeAction();

        /// <summary>
        /// Initializes a new instance of the <see cref="Actions"/> class.
        /// </summary>
        /// <param name="driver">The <see cref="IWebDriver"/> object on which the actions built will be performed.</param>
        public Actions(IWebDriver driver)
        {
            //this.driver = driver;
            var inputDevicesDriver = GetDriverAs<IHasInputDevices>(driver);
            if (inputDevicesDriver == null)
            {
                throw new ArgumentException("The IWebDriver object must implement or wrap a driver that implements IHasInputDevices.", "driver");
            }

            var actionExecutor = GetDriverAs<IActionExecutor>(driver);
            if (actionExecutor == null)
            {
                throw new ArgumentException("The IWebDriver object must implement or wrap a driver that implements IActionExecutor.", "driver");
            }

            keyboard = inputDevicesDriver.Keyboard;
            mouse = inputDevicesDriver.Mouse;
            this.actionExecutor = actionExecutor;
        }

    /// <summary>
    /// Sends a modifier key down message to the browser.
    /// </summary>
    /// <param name="theKey">The key to be sent.</param>
    /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
    /// <exception cref="ArgumentException">If the key sent is not is not one
    /// of <see cref="Keys.Shift"/>, <see cref="Keys.Control"/>, <see cref="Keys.Alt"/>,
    /// <see cref="Keys.Meta"/>, <see cref="Keys.Command"/>,<see cref="Keys.LeftAlt"/>,
    /// <see cref="Keys.LeftControl"/>,<see cref="Keys.LeftShift"/>.</exception>
    public Actions KeyDown(string theKey)
        {
            return KeyDown(null, theKey);
        }

        /// <summary>
        /// Sends a modifier key down message to the specified element in the browser.
        /// </summary>
        /// <param name="element">The element to which to send the key command.</param>
        /// <param name="theKey">The key to be sent.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        /// <exception cref="ArgumentException">If the key sent is not is not one
        /// of <see cref="Keys.Shift"/>, <see cref="Keys.Control"/>, <see cref="Keys.Alt"/>,
        /// <see cref="Keys.Meta"/>, <see cref="Keys.Command"/>,<see cref="Keys.LeftAlt"/>,
        /// <see cref="Keys.LeftControl"/>,<see cref="Keys.LeftShift"/>.</exception>
        public Actions KeyDown(IWebElement element, string theKey)
        {
            if (string.IsNullOrEmpty(theKey))
            {
                throw new ArgumentException("The key value must not be null or empty", "theKey");
            }

            var target = GetLocatableFromElement(element);
            action.AddAction(new KeyDownAction(keyboard, mouse, target, theKey));
            if (element != null)
            {
                actionBuilder.AddAction(defaultMouse.CreatePointerMove(element, 0, 0, DefaultMouseMoveDuration));
                actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Left));
                actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Left));
            }

            actionBuilder.AddAction(defaultKeyboard.CreateKeyDown(theKey[0]));
            actionBuilder.AddAction(new PauseInteraction(defaultKeyboard, TimeSpan.FromMilliseconds(100)));
            return this;
        }

    /// <summary>
    /// Sends a modifier key up message to the browser.
    /// </summary>
    /// <param name="theKey">The key to be sent.</param>
    /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
    /// <exception cref="ArgumentException">If the key sent is not is not one
    /// of <see cref="Keys.Shift"/>, <see cref="Keys.Control"/>, <see cref="Keys.Alt"/>,
    /// <see cref="Keys.Meta"/>, <see cref="Keys.Command"/>,<see cref="Keys.LeftAlt"/>,
    /// <see cref="Keys.LeftControl"/>,<see cref="Keys.LeftShift"/>.</exception>
    public Actions KeyUp(string theKey)
        {
            return KeyUp(null, theKey);
        }

        /// <summary>
        /// Sends a modifier up down message to the specified element in the browser.
        /// </summary>
        /// <param name="element">The element to which to send the key command.</param>
        /// <param name="theKey">The key to be sent.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        /// <exception cref="ArgumentException">If the key sent is not is not one
        /// of <see cref="Keys.Shift"/>, <see cref="Keys.Control"/>, <see cref="Keys.Alt"/>,
        /// <see cref="Keys.Meta"/>, <see cref="Keys.Command"/>,<see cref="Keys.LeftAlt"/>,
        /// <see cref="Keys.LeftControl"/>,<see cref="Keys.LeftShift"/>.</exception>
        public Actions KeyUp(IWebElement element, string theKey)
        {
            if (string.IsNullOrEmpty(theKey))
            {
                throw new ArgumentException("The key value must not be null or empty", "theKey");
            }

            var target = GetLocatableFromElement(element);
            action.AddAction(new KeyUpAction(keyboard, mouse, target, theKey));
            if (element != null)
            {
                actionBuilder.AddAction(defaultMouse.CreatePointerMove(element, 0, 0, DefaultMouseMoveDuration));
                actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Left));
                actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Left));
            }

            actionBuilder.AddAction(defaultKeyboard.CreateKeyUp(theKey[0]));
            return this;
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the browser.
        /// </summary>
        /// <param name="keysToSend">The keystrokes to send to the browser.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions SendKeys(string keysToSend)
        {
            return SendKeys(null, keysToSend);
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the specified element in the browser.
        /// </summary>
        /// <param name="element">The element to which to send the keystrokes.</param>
        /// <param name="keysToSend">The keystrokes to send to the browser.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions SendKeys(IWebElement element, string keysToSend)
        {
            if (string.IsNullOrEmpty(keysToSend))
            {
                throw new ArgumentException("The key value must not be null or empty", "keysToSend");
            }

            var target = GetLocatableFromElement(element);
            action.AddAction(new SendKeysAction(keyboard, mouse, target, keysToSend));
            if (element != null)
            {
                actionBuilder.AddAction(defaultMouse.CreatePointerMove(element, 0, 0, DefaultMouseMoveDuration));
                actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Left));
                actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Left));
            }

            foreach (var key in keysToSend)
            {
                actionBuilder.AddAction(defaultKeyboard.CreateKeyDown(key));
                actionBuilder.AddAction(defaultKeyboard.CreateKeyUp(key));
            }

            return this;
        }

        /// <summary>
        /// Clicks and holds the mouse button down on the specified element.
        /// </summary>
        /// <param name="onElement">The element on which to click and hold.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions ClickAndHold(IWebElement onElement)
        {
            MoveToElement(onElement).ClickAndHold();
            return this;
        }

        /// <summary>
        /// Clicks and holds the mouse button at the last known mouse coordinates.
        /// </summary>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions ClickAndHold()
        {
            action.AddAction(new ClickAndHoldAction(mouse, null));
            actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Left));
            return this;
        }

        /// <summary>
        /// Releases the mouse button on the specified element.
        /// </summary>
        /// <param name="onElement">The element on which to release the button.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions Release(IWebElement onElement)
        {
            MoveToElement(onElement).Release();
            return this;
        }

        /// <summary>
        /// Releases the mouse button at the last known mouse coordinates.
        /// </summary>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions Release()
        {
            action.AddAction(new ButtonReleaseAction(mouse, null));
            actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Left));
            return this;
        }

        /// <summary>
        /// Clicks the mouse on the specified element.
        /// </summary>
        /// <param name="onElement">The element on which to click.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions Click(IWebElement onElement)
        {
            MoveToElement(onElement).Click();
            return this;
        }

        /// <summary>
        /// Clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions Click()
        {
            action.AddAction(new ClickAction(mouse, null));
            actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Left));
            actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Left));
            return this;
        }

        /// <summary>
        /// Double-clicks the mouse on the specified element.
        /// </summary>
        /// <param name="onElement">The element on which to double-click.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions DoubleClick(IWebElement onElement)
        {
            MoveToElement(onElement).DoubleClick();
            return this;
        }

        /// <summary>
        /// Double-clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions DoubleClick()
        {
            action.AddAction(new DoubleClickAction(mouse, null));
            actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Left));
            actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Left));
            actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Left));
            actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Left));
            return this;
        }

        /// <summary>
        /// Moves the mouse to the specified element.
        /// </summary>
        /// <param name="toElement">The element to which to move the mouse.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions MoveToElement(IWebElement toElement)
        {
            if (toElement == null)
            {
                throw new ArgumentException("MoveToElement cannot move to a null element with no offset.", "toElement");
            }

            var target = GetLocatableFromElement(toElement);
            action.AddAction(new MoveMouseAction(mouse, target));
            actionBuilder.AddAction(defaultMouse.CreatePointerMove(toElement, 0, 0, DefaultMouseMoveDuration));
            return this;
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the top-left corner of the specified element.
        /// </summary>
        /// <param name="toElement">The element to which to move the mouse.</param>
        /// <param name="offsetX">The horizontal offset to which to move the mouse.</param>
        /// <param name="offsetY">The vertical offset to which to move the mouse.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions MoveToElement(IWebElement toElement, int offsetX, int offsetY)
        {
            return MoveToElement(toElement, offsetX, offsetY, MoveToElementOffsetOrigin.TopLeft);
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the top-left corner of the specified element.
        /// </summary>
        /// <param name="toElement">The element to which to move the mouse.</param>
        /// <param name="offsetX">The horizontal offset to which to move the mouse.</param>
        /// <param name="offsetY">The vertical offset to which to move the mouse.</param>
        /// <param name="offsetOrigin">The <see cref="MoveToElementOffsetOrigin"/> value from which to calculate the offset.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions MoveToElement(IWebElement toElement, int offsetX, int offsetY, MoveToElementOffsetOrigin offsetOrigin)
        {
            var target = GetLocatableFromElement(toElement);
            var elementSize = toElement.Size;
            var elementLocation = toElement.Location;
            if (offsetOrigin == MoveToElementOffsetOrigin.TopLeft)
            {
                var modifiedOffsetX = offsetX - (elementSize.Width / 2);
                var modifiedOffsetY = offsetY - (elementSize.Height / 2);
                action.AddAction(new MoveToOffsetAction(mouse, target, offsetX, offsetY));
                actionBuilder.AddAction(defaultMouse.CreatePointerMove(toElement, modifiedOffsetX, modifiedOffsetY, DefaultMouseMoveDuration));
            }
            else
            {
                var modifiedOffsetX = offsetX + (elementSize.Width / 2);
                var modifiedOffsetY = offsetY + (elementSize.Height / 2);
                action.AddAction(new MoveToOffsetAction(mouse, target, modifiedOffsetX, modifiedOffsetY));
                actionBuilder.AddAction(defaultMouse.CreatePointerMove(toElement, offsetX, offsetY, DefaultMouseMoveDuration));
            }
            return this;
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the last known mouse coordinates.
        /// </summary>
        /// <param name="offsetX">The horizontal offset to which to move the mouse.</param>
        /// <param name="offsetY">The vertical offset to which to move the mouse.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions MoveByOffset(int offsetX, int offsetY)
        {
            action.AddAction(new MoveToOffsetAction(mouse, null, offsetX, offsetY));
            actionBuilder.AddAction(defaultMouse.CreatePointerMove(CoordinateOrigin.Pointer, offsetX, offsetY, DefaultMouseMoveDuration));
            return this;
        }

        /// <summary>
        /// Right-clicks the mouse on the specified element.
        /// </summary>
        /// <param name="onElement">The element on which to right-click.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions ContextClick(IWebElement onElement)
        {
            MoveToElement(onElement).ContextClick();
            return this;
        }

        /// <summary>
        /// Right-clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions ContextClick()
        {
            action.AddAction(new ContextClickAction(mouse, null));
            actionBuilder.AddAction(defaultMouse.CreatePointerDown(MouseButton.Right));
            actionBuilder.AddAction(defaultMouse.CreatePointerUp(MouseButton.Right));
            return this;
        }

        /// <summary>
        /// Performs a drag-and-drop operation from one element to another.
        /// </summary>
        /// <param name="source">The element on which the drag operation is started.</param>
        /// <param name="target">The element on which the drop is performed.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions DragAndDrop(IWebElement source, IWebElement target)
        {
            ClickAndHold(source).MoveToElement(target).Release(target);
            return this;
        }

        /// <summary>
        /// Performs a drag-and-drop operation on one element to a specified offset.
        /// </summary>
        /// <param name="source">The element on which the drag operation is started.</param>
        /// <param name="offsetX">The horizontal offset to which to move the mouse.</param>
        /// <param name="offsetY">The vertical offset to which to move the mouse.</param>
        /// <returns>A self-reference to this <see cref="Actions"/>.</returns>
        public Actions DragAndDropToOffset(IWebElement source, int offsetX, int offsetY)
        {
            ClickAndHold(source).MoveByOffset(offsetX, offsetY).Release();
            return this;
        }

        /// <summary>
        /// Builds the sequence of actions.
        /// </summary>
        /// <returns>A composite <see cref="IAction"/> which can be used to perform the actions.</returns>
        public IAction Build()
        {
            return this;
        }

        /// <summary>
        /// Performs the currently built action.
        /// </summary>
        public void Perform()
        {
            if (actionExecutor.IsActionExecutor)
            {
                actionExecutor.PerformActions(actionBuilder.ToActionSequenceList());
            }
            else
            {
                action.Perform();
            }
        }

        /// <summary>
        /// Gets the <see cref="ILocatable"/> instance of the specified <see cref="IWebElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="IWebElement"/> to get the location of.</param>
        /// <returns>The <see cref="ILocatable"/> of the <see cref="IWebElement"/>.</returns>
        protected static ILocatable GetLocatableFromElement(IWebElement element)
        {
            if (element == null)
            {
                return null;
            }

            ILocatable target = null;
            var wrapper = element as IWrapsElement;
            while (wrapper != null)
            {
                target = wrapper.WrappedElement as ILocatable;
                wrapper = wrapper.WrappedElement as IWrapsElement;
            }

            if (target == null)
            {
                target = element as ILocatable;
            }

            if (target == null)
            {
                throw new ArgumentException("The IWebElement object must implement or wrap an element that implements ILocatable.", "element");
            }

            return target;
        }

        /// <summary>
        /// Adds an action to current list of actions to be performed.
        /// </summary>
        /// <param name="actionToAdd">The <see cref="IAction"/> to be added.</param>
        protected void AddAction(IAction actionToAdd)
        {
            action.AddAction(actionToAdd);
        }

        private T GetDriverAs<T>(IWebDriver driver) where T : class
        {
            var driverAsType = driver as T;
            if (driverAsType == null)
            {
                var wrapper = driver as IWrapsDriver;
                while (wrapper != null)
                {
                    driverAsType = wrapper.WrappedDriver as T;
                    if (driverAsType != null)
                    {
                        driver = wrapper.WrappedDriver;
                        break;
                    }

                    wrapper = wrapper.WrappedDriver as IWrapsDriver;
                }
            }

            return driverAsType;
        }
    }
}
