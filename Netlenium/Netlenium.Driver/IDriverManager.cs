namespace Netlenium.Driver
{
    /// <summary>
    /// Driver Manager Interface
    /// </summary>
    public interface IDriverManager
    {
        /// <summary>
        /// The platform that the Driver Manager will target
        /// </summary>
        Platform TargetPlatform { get; set; }

        /// <summary>
        /// Gets the latest version of the driver that's available
        /// </summary>
        string GetLatestVersion();

        /// <summary>
        /// Determines
        /// </summary>
        bool IsInstalled { get; set; }

    }
}
