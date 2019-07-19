using Netlenium.Logging;
using System;
using System.IO;
using System.Net;

namespace Netlenium.Driver.Firefox
{
    /// <summary>
    /// Firefox Driver Manager
    /// </summary>
    public class DriverManager : IDriverManager
    {
        /// <summary>
        /// The main gecko driver
        /// </summary>
        private readonly Driver driver;
        
        public bool IsInstalled
        {
            get
            {
                var driverDirectoryPath =
                    $"{ApplicationPaths.DriverDirectory}{Path.DirectorySeparatorChar}{Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser)}";

                var versionFilePath = $"{driverDirectoryPath}{Path.DirectorySeparatorChar}version";

                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager",
                    $"Checking if the directory '{driverDirectoryPath}' exists");
                if (Directory.Exists(driverDirectoryPath) == false)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager",
                        $"Directory '{driverDirectoryPath}' does not exists");
                    return false;
                }

                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager",
                    $"Checking if the file '{versionFilePath}' exists");
                if (File.Exists(versionFilePath) == false)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager",
                        $"File '{versionFilePath}' does not exists");
                    return false;
                }

                switch (driver.TargetPlatform)
                {
                    case Platform.Linux32 | Platform.Linux64:
                        if (File.Exists($"{driverDirectoryPath}{Path.DirectorySeparatorChar}{DriverExecutableName}") == false)
                        {
                            return false;
                        }
                        break;

                    case Platform.Windows32 | Platform.Windows64:
                        if (File.Exists($"{driverDirectoryPath}{Path.DirectorySeparatorChar}{DriverExecutableName}") == false)
                        {
                            return false;
                        }
                        break;

                    default:
                        throw new UnsupportedPlatformException();
                }

                return true;
            }
        }

        public string DriverExecutableName
        {
            get
            {
                switch (driver.TargetPlatform)
                {
                    case Platform.Windows32 | Platform.Windows64:
                        return "geckodriver.exe";

                    case Platform.Linux32 | Platform.Linux64:
                        return "geckodriver";

                    default:
                        driver.Logging.WriteEntry(MessageType.Error, "DriverManager",
                            $"The platform '{driver.TargetPlatform}' is not supported for this driver");
                        throw new UnsupportedPlatformException();
                }
            }
        }

        public string DriverDirectoryPath
        {
            get
            {
                var directoryName = Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser);
                return $"{ApplicationPaths.DriverDirectory}{Path.DirectorySeparatorChar}{directoryName}";
            }
        }

        public string DriverExecutablePath => $"{DriverDirectoryPath}{Path.DirectorySeparatorChar}{DriverExecutableName}";

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="driver"></param>
        public DriverManager(Driver driver)
        {
            this.driver = driver;
        }

        public Version GetCurrentVersion()
        {
            if (IsInstalled == false)
            {
                throw new DriverNotInstalledException();
            }

            var driverDirectoryPath =
                $"{ApplicationPaths.DriverDirectory}{Path.DirectorySeparatorChar}{Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser)}";

            var versionFilePath = $"{driverDirectoryPath}{Path.DirectorySeparatorChar}version";

            return new Version(File.ReadAllText(versionFilePath));
        }

        public Version GetLatestVersion()
        {
            try
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating HTTP/S Request to GitHub API For the latest Release on 'mozilla/geckodriver'");

                var release = WebAPI.GitHub.Releases.GetLatestRelease("mozilla", "geckodriver");

                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"ID => {release.Id}");
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Tag Name => {release.TagName}");
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"NodeID => {release.NodeId}");
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Created At => {release.CreatedAt}");
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager",
                    $"Published At => {release.PublishedAt}");
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Release URL => {release.HtmlUrl}");

                return new Version(release.TagName.Substring(1));
            }
            catch (Exception e)
            {
                throw new WebRequestException(e.Message);
            }
        }

        public void Initialize()
        {
            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Checking driver resources");

            if (IsInstalled == false)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager", "Driver is not installed, attempting to install driver resources");
                InstallLatestDriver();
                return;
            }

            var latestVersion = GetLatestVersion();
            var currentVersion = GetCurrentVersion();

            if (currentVersion.CompareTo(latestVersion) != 0)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager",
                    $"The current driver ({currentVersion}) does not match the latest version {latestVersion}");
                driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Attempting to update the driver resources");
                InstallLatestDriver();
                return;
            }

            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Driver resources are up to date");
        }

        /// <summary>
        /// Installs the latest driver
        /// </summary>
        private void InstallLatestDriver()
        {
            var latestVersion = GetLatestVersion();

            var driverVersionFilePath = $"{DriverDirectoryPath}{Path.DirectorySeparatorChar}version";
            const bool permissionsRequired = false;

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating HTTP/S Request to GitHub API For the latest Release on 'mozilla/geckodriver'");
            var release = WebAPI.GitHub.Releases.GetLatestRelease("mozilla", "geckodriver");

            WebAPI.GitHub.Asset targetAsset = null;
            foreach(var asset in release.Assets)
            {
                if (Utilities.DetectTargetPlatformFromArchive(asset.Name) != driver.TargetPlatform) continue;
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Asset found '{asset.Name}'");
                targetAsset = asset;
                break;
            }

            if(targetAsset == null)
            {
                throw new PlatformNotSupportedException($"No driver for the platform '{driver.TargetPlatform}' could be found");
            }

            var temporaryFileDownloadPath = string.Format($"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}{targetAsset.Name}");

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Temporary File Download Path: {temporaryFileDownloadPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Executable Name: {DriverExecutableName}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Directory Path: {DriverDirectoryPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Version File Path: {driverVersionFilePath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Permissions Required (chmod): {permissionsRequired}");

            if (File.Exists(temporaryFileDownloadPath))
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"The file '{temporaryFileDownloadPath}' already exists, this file will be deleted");
                File.Delete(temporaryFileDownloadPath);
            }

            // Check files before modification
            if (Directory.Exists(DriverDirectoryPath))
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"The directory '{DriverDirectoryPath}' already exists, this directory will be deleted");
                Directory.Delete(DriverDirectoryPath, true);
            }

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Creating directory '{DriverDirectoryPath}'");
            Directory.CreateDirectory(DriverDirectoryPath);

            // Download the archive
            var webClient = new WebClient();
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Downloading archive from '{targetAsset.BrowserDownloadUrl}'");
            webClient.DownloadFile(targetAsset.BrowserDownloadUrl.ToString(), temporaryFileDownloadPath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Download completed");

            // Extract archive and create version file
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Extracting driver resources from archive '{temporaryFileDownloadPath}'");
            FileArchive.ExtractArchive(temporaryFileDownloadPath, DriverDirectoryPath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating version file");
            File.WriteAllText(driverVersionFilePath, latestVersion.ToString());

            // Cleanup temporary files
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Deleting temporary file '{temporaryFileDownloadPath}'");
            File.Delete(temporaryFileDownloadPath);

            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "The driver installation has completed successfully");
        }
    }
}
