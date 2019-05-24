using System;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Interface implemented by each driver that allows access to touch screen capabilities.
    /// </summary>
    //[Obsolete("Use the TouchActions or ActionBuilder class to simulate touch input.")]
    public interface IHasTouchScreen
    {
        /// <summary>
        /// Gets the device representing the touch screen.
        /// </summary>
        ITouchScreen TouchScreen { get; }
    }
}
