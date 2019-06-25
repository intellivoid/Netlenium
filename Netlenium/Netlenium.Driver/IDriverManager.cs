namespace Netlenium.Driver
{
    public interface IDriverManager
    {
        /// <summary>
        /// The platform that the Driver Manager will target
        /// </summary>
        PlatformType TargetPlatform { get; set; }

        /// <summary>
        /// The latest version of the driver for the selected platform
        /// </summary>
        string LatestVersion { get; }

        /// <summary>
        /// Determines if a driver is currently installed or not
        /// </summary>
        bool IsInstalled { get; }

        /// <summary>
        /// If the driver is currently installed, the current version that's installed will be returned.
        /// </summary>
        string CurrentInstallationVersion { get; }

        /// <summary>
        /// The path where all the driver contents are available at
        /// </summary>
        string DriverPath { get; }

        /// <summary>
        /// The name of the executable driver name
        /// </summary>
        string DriverExecutableName { get; }

        /// <summary>
        /// The full path to the Driver Executable file
        /// </summary>
        string DriverExecutablePath { get; }

        /// <summary>
        /// Downloads the latest drivers and installs it
        /// </summary>
        void InstallLatestDriver();

        /// <summary>
        /// Uninstalls the current driver
        /// </summary>
        /// <param name="force">Ignores possible errors and proceeds with the full operation</param>
        void UninstallDriver(bool force);

        /// <summary>
        /// Checks if the driver is installed, if so check if it's up to date
        /// </summary>
        void Initialize();
    }
}
