using Netlenium.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver.Firefox
{
    public class DriverManager : IDriverManager
    {
        /// <summary>
        /// The main gecko driver
        /// </summary>
        private Driver driver;
        
        public bool IsInstalled
        {
            get
            {
                var DriverDirectoryPath = string.Format("{0}{1}{2}",
                    ApplicationPaths.DriverDirectory, Path.DirectorySeparatorChar,
                    Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser)
                );

                var VersionFilePath = string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, "version");

                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", string.Format("Checking if the directory '{0}' exists", DriverDirectoryPath));
                if (Directory.Exists(DriverDirectoryPath) == false)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", string.Format("Directory '{0}' does not exists", DriverDirectoryPath));
                    return false;
                }

                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", string.Format("Checking if the file '{0}' exists", VersionFilePath));
                if (File.Exists(VersionFilePath) == false)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", string.Format("File '{0}' does not exists", VersionFilePath));
                    return false;
                }

                switch (driver.TargetPlatform)
                {
                    case Platform.Linux32 | Platform.Linux64:
                        if (File.Exists(string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, DriverExecutableName)) == false)
                        {
                            return false;
                        }
                        break;

                    case Platform.Windows32 | Platform.Windows64:
                        if (File.Exists(string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, DriverExecutableName)) == false)
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
                        driver.Logging.WriteEntry(MessageType.Error, "DriverManager", string.Format("The platform '{0}' is not supported for this driver", driver.TargetPlatform));
                        throw new UnsupportedPlatformException();
                }
            }
        }

        public string DriverDirectoryPath
        {
            get
            {
                var DirectoryName = Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser);
                return $"{ApplicationPaths.DriverDirectory}{Path.DirectorySeparatorChar}{DirectoryName}";
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

            var DriverDirectoryPath = string.Format("{0}{1}{2}",
                   ApplicationPaths.DriverDirectory, Path.DirectorySeparatorChar,
                   Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser)
               );

            var VersionFilePath = string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, "version");

            return new Version(File.ReadAllText(VersionFilePath));
        }

        public Version GetLatestVersion()
        {
            try
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating HTTP/S Request to GitHub API For the latest Release on 'mozilla/geckodriver'");

                var Release = WebAPI.GitHub.Releases.GetLatestRelease("mozilla", "geckodriver");

                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("ID => {0}", Release.ID));
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("Tag Name => {0}", Release.TagName));
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("NodeID => {0}", Release.NodeID));
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("Created At => {0}", Release.CreatedAt));
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("Published At => {0}", Release.PublishedAt));
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("Release URL => {0}", Release.HtmlURL));

                return new Version(Release.TagName.Substring(1));
            }
            catch (Exception e)
            {
                throw new WebRequestException(e.Message);
            }
        }

        public void Initalize()
        {
            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Checking driver resources");

            if (IsInstalled == false)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager", "Driver is not installed, attempting to install driver resources");
                InstallLatestDriver();
                return;
            }

            var LatestVersion = GetLatestVersion();
            var CurrentVersion = GetCurrentVersion();

            if (CurrentVersion.CompareTo(LatestVersion) != 0)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager", string.Format("The current driver ({0}) does not match the latest version {1}", CurrentVersion, LatestVersion));
                driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Attempting to update the driver resources");
                InstallLatestDriver();
                return;
            }

            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Driver resources are up to date");
            return;
        }

        /// <summary>
        /// Installs the latest driver
        /// </summary>
        public void InstallLatestDriver()
        {
            WebAPI.Google.Content Resource;
            var LatestVersion = GetLatestVersion();

            var DriverVersionFilePath = $"{DriverDirectoryPath}{Path.DirectorySeparatorChar}version";
            var PermissionsRequired = false;

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating HTTP/S Request to GitHub API For the latest Release on 'mozilla/geckodriver'");
            var Release = WebAPI.GitHub.Releases.GetLatestRelease("mozilla", "geckodriver");

            WebAPI.GitHub.Asset TargetAsset = null;
            foreach(WebAPI.GitHub.Asset asset in Release.Assets)
            {
                if(Utilities.DetectTargetPlatformFromArchive(asset.Name) == driver.TargetPlatform)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Asset found '{asset.Name}'");
                    TargetAsset = asset;
                    break;
                }
            }

            if(TargetAsset == null)
            {
                throw new PlatformNotSupportedException($"No driver for the platform '{driver.TargetPlatform}' could be found");
            }

            var TemporaryFileDownloadPath = string.Format($"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}{TargetAsset.Name}");

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Temporary File Download Path: {TemporaryFileDownloadPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Executable Name: {DriverExecutableName}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Directory Path: {DriverDirectoryPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Version File Path: {DriverVersionFilePath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Permissions Required (chmod): {PermissionsRequired}");

            if (File.Exists(TemporaryFileDownloadPath) == true)
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"The file '{TemporaryFileDownloadPath}' already exists, this file will be deleted");
                File.Delete(TemporaryFileDownloadPath);
            }

            // Check files before modification
            if (Directory.Exists(DriverDirectoryPath) == true)
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"The directory '{DriverDirectoryPath}' already exists, this directory will be deleted");
                Directory.Delete(DriverDirectoryPath, true);
            }

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Creating directory '{DriverDirectoryPath}'");
            Directory.CreateDirectory(DriverDirectoryPath);

            // Download the archive
            var webClient = new WebClient();
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Downloading archive from '{TargetAsset.BrowserDownloadURL.ToString()}'");
            webClient.DownloadFile(TargetAsset.BrowserDownloadURL.ToString(), TemporaryFileDownloadPath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Download completed");

            // Extract archive and create version file
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Extracting driver resources from archive '{TemporaryFileDownloadPath}'");
            FileArchive.ExtractArchive(TemporaryFileDownloadPath, DriverDirectoryPath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating version file");
            File.WriteAllText(DriverVersionFilePath, LatestVersion.ToString());

            // Cleanup temporary files
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Deleting temporary file '{TemporaryFileDownloadPath}'");
            File.Delete(TemporaryFileDownloadPath);

            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "The driver installalation has completed successfully");
        }
    }
}
