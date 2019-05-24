using System.Drawing;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Provides methods for getting and setting the size and position of the browser window.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Gets or sets the position of the browser window relative to the upper-left corner of the screen.
        /// </summary>
        /// <remarks>When setting this property, it should act as the JavaScript window.moveTo() method.</remarks>
        Point Position { get; set; }

        /// <summary>
        /// Gets or sets the size of the outer browser window, including title bars and window borders.
        /// </summary>
        /// <remarks>When setting this property, it should act as the JavaScript window.resizeTo() method.</remarks>
        Size Size { get; set; }

        /// <summary>
        /// Maximizes the current window if it is not already maximized.
        /// </summary>
        void Maximize();

        /// <summary>
        /// Minimizes the current window if it is not already maximized.
        /// </summary>
        void Minimize();

        /// <summary>
        /// Sets the current window to full screen if it is not already in that state.
        /// </summary>
        void FullScreen();
    }
}
