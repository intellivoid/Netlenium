﻿namespace Netlenium.Driver
{
    /// <summary>
    ///     Window/Tab object from the Driver
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        ///     The ID of the window
        /// </summary>
        string ID { get; }

        /// <summary>
        ///     Switches to this Window
        /// </summary>
        void SwitchTo();
    }
}