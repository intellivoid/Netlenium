namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Interface for location context
    /// </summary>
    public interface ILocationContext
    {
        /// <summary>
        /// Gets or sets a value indicating the physical location of the browser.
        /// </summary>
        Location PhysicalLocation { get; set; }
    }
}
