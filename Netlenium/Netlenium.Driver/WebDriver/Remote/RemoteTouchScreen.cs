using System;
using System.Collections.Generic;
using Netlenium.Driver.WebDriver.Interactions.Internal;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Defines the interface through which the user can execute advanced touch screen interactions.
    /// </summary>
    public class RemoteTouchScreen : ITouchScreen
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTouchScreen"/> class.
        /// </summary>
        /// <param name="driver">The <see cref="RemoteWebDriver"/> for which the touch screen will be managed.</param>
        public RemoteTouchScreen(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Allows the execution of single tap on the screen, analogous to click using a Mouse.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen,
        /// usually an <see cref="IWebElement"/>.</param>
        public void SingleTap(ICoordinates where)
        {
            if (where == null)
            {
                throw new ArgumentNullException("where", "where coordinates cannot be null");
            }

            var elementId = where.AuxiliaryLocator.ToString();
            var parameters = new Dictionary<string, object>();
            parameters.Add("element", elementId);
            driver.InternalExecute(DriverCommand.TouchSingleTap, parameters);
        }

        /// <summary>
        /// Allows the execution of the gesture 'down' on the screen. It is typically the first of a
        /// sequence of touch gestures.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        public void Down(int locationX, int locationY)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("x", locationX);
            parameters.Add("y", locationY);
            driver.InternalExecute(DriverCommand.TouchPress, parameters);
        }

        /// <summary>
        /// Allows the execution of the gesture 'up' on the screen. It is typically the last of a
        /// sequence of touch gestures.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        public void Up(int locationX, int locationY)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("x", locationX);
            parameters.Add("y", locationY);
            driver.InternalExecute(DriverCommand.TouchRelease, parameters);
        }

        /// <summary>
        /// Allows the execution of the gesture 'move' on the screen.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        public void Move(int locationX, int locationY)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("x", locationX);
            parameters.Add("y", locationY);
            driver.InternalExecute(DriverCommand.TouchMove, parameters);
        }

        /// <summary>
        /// Creates a scroll gesture that starts on a particular screen location.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen
        /// where the scroll starts, usually an <see cref="IWebElement"/>.</param>
        /// <param name="offsetX">The x coordinate relative to the view port.</param>
        /// <param name="offsetY">The y coordinate relative to the view port.</param>
        public void Scroll(ICoordinates where, int offsetX, int offsetY)
        {
            if (where == null)
            {
                throw new ArgumentNullException("where", "where coordinates cannot be null");
            }

            var elementId = where.AuxiliaryLocator.ToString();
            var parameters = new Dictionary<string, object>();
            parameters.Add("element", elementId);
            parameters.Add("xoffset", offsetX);
            parameters.Add("yoffset", offsetY);
            driver.InternalExecute(DriverCommand.TouchScroll, parameters);
        }

        /// <summary>
        /// Creates a scroll gesture for a particular x and y offset.
        /// </summary>
        /// <param name="offsetX">The horizontal offset relative to the view port.</param>
        /// <param name="offsetY">The vertical offset relative to the view port.</param>
        public void Scroll(int offsetX, int offsetY)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("xoffset", offsetX);
            parameters.Add("yoffset", offsetY);
            driver.InternalExecute(DriverCommand.TouchScroll, parameters);
        }

        /// <summary>
        /// Allows the execution of double tap on the screen, analogous to click using a Mouse.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen,
        /// usually an <see cref="IWebElement"/>.</param>
        public void DoubleTap(ICoordinates where)
        {
            if (where == null)
            {
                throw new ArgumentNullException("where", "where coordinates cannot be null");
            }

            var elementId = where.AuxiliaryLocator.ToString();
            var parameters = new Dictionary<string, object>();
            parameters.Add("element", elementId);
            driver.InternalExecute(DriverCommand.TouchDoubleTap, parameters);
        }

        /// <summary>
        /// Allows the execution of a long press gesture on the screen.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen,
        /// usually an <see cref="IWebElement"/>.</param>
        public void LongPress(ICoordinates where)
        {
            if (where == null)
            {
                throw new ArgumentNullException("where", "where coordinates cannot be null");
            }

            var elementId = where.AuxiliaryLocator.ToString();
            var parameters = new Dictionary<string, object>();
            parameters.Add("element", elementId);
            driver.InternalExecute(DriverCommand.TouchLongPress, parameters);
        }

        /// <summary>
        /// Creates a flick gesture for the current view.
        /// </summary>
        /// <param name="speedX">The horizontal speed in pixels per second.</param>
        /// <param name="speedY">The vertical speed in pixels per second.</param>
        public void Flick(int speedX, int speedY)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("xspeed", speedX);
            parameters.Add("yspeed", speedY);
            driver.InternalExecute(DriverCommand.TouchFlick, parameters);
        }

        /// <summary>
        /// Creates a flick gesture for the current view starting at a specific location.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen
        /// where the scroll starts, usually an <see cref="IWebElement"/>.</param>
        /// <param name="offsetX">The x offset relative to the viewport.</param>
        /// <param name="offsetY">The y offset relative to the viewport.</param>
        /// <param name="speed">The speed in pixels per second.</param>
        public void Flick(ICoordinates where, int offsetX, int offsetY, int speed)
        {
            if (where == null)
            {
                throw new ArgumentNullException("where", "where coordinates cannot be null");
            }

            var elementId = where.AuxiliaryLocator.ToString();
            var parameters = new Dictionary<string, object>();
            parameters.Add("element", elementId);
            parameters.Add("xoffset", offsetX);
            parameters.Add("yoffset", offsetY);
            parameters.Add("speed", speed);
            driver.InternalExecute(DriverCommand.TouchFlick, parameters);
        }
    }
}
