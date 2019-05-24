namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Enumerated values for the kinds of devices available.
    /// </summary>
    public enum InputDeviceKind
    {
        /// <summary>
        /// Represents the null device.
        /// </summary>
        None,

        /// <summary>
        /// Represents a key-based device, primarily for entering text.
        /// </summary>
        Key,

        /// <summary>
        /// Represents a pointer-based device, such as a mouse, pen, or stylus.
        /// </summary>
        Pointer
    }
}
