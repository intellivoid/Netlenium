using System;

namespace Netlenium.Objects
{
    /// <summary>
    /// The current Window Handle
    /// </summary>
    [Serializable]
    public class CurrentWindow
    {
        /// <summary>
        /// The Window ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The current URL that the window is loaded on
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The current of the Window
        /// </summary>
        public string Title { get; set; }
    }
}
