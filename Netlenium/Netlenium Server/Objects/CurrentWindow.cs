﻿namespace NetleniumServer.Objects
{
    /// <summary>
    /// The current Window Handle
    /// </summary>
    public class CurrentWindow
    {
        /// <summary>
        /// The Window ID
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// The current URL that the window is loaded on
        /// </summary>
        public string URL { get; private set; }

        /// <summary>
        /// The current of the Window
        /// </summary>
        public string Title { get; private set; }
    }
}
