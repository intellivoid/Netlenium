using System;
using System.IO;
using System.Net;
using Netlenium.Logging;

namespace Netlenium.Driver.Chrome
{
    /// <summary>
    /// Manages the Driver Resources for Chrome
    /// </summary>
    public class DriverManager : IDriverManager
    {
        /// <summary>
        /// The main chrome driver
        /// </summary>
        private readonly Driver driver;
        
        /// <inheritdoc />
        /// <summary>
        /// Determines if the Driver is currently installed
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                var driverDirectoryPath =
                    $"{ApplicationPaths.DriverDirectory}{Path.DirectorySeparatorChar}{Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser)}";

                var versionFilePath = $"{driverDirectoryPath}{Path.DirectorySeparatorChar}version";

                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager",
                    $"Checking if the directory '{driverDirectoryPath}' exists");
                if(Directory.Exists(driverDirectoryPath) == false)
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

                switch(driver.TargetPlatform)
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
                        return "chromedriver.exe";
                        
                    case Platform.Linux32 | Platform.Linux64:
                        return "chromedriver";

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

        /// <inheritdoc />
        /// <summary>
        /// Gets the current version that's installed
        /// Throws an exception if the driver is not installed
        /// </summary>
        /// <returns></returns>
        public Version GetCurrentVersion()
        {
            if(IsInstalled == false)
            {
                throw new DriverNotInstalledException();
            }

            var driverDirectoryPath =
                $"{ApplicationPaths.DriverDirectory}{Path.DirectorySeparatorChar}{Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser)}";

            var versionFilePath = $"{driverDirectoryPath}{Path.DirectorySeparatorChar}version";

            return new Version(File.ReadAllText(versionFilePath));
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the current version that's publicly available from
        /// Google storage content
        /// </summary>
        /// <returns></returns>
        public Version GetLatestVersion()
        {
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating HTTP/S Request to Google Storage for Resource 'LATEST_RELEASE'");

            var resourceContent = WebAPI.Google.Storage.FetchResource("LATEST_RELEASE");

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager",
                $"Access Location => {resourceContent.AccessLocation}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"ETag => {resourceContent.ETag}");
            
            try
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager",
                    $"Creating HTTP/S Request to {resourceContent.AccessLocation}");
                var httpWebClient = new WebClient();
                var response = httpWebClient.DownloadString(resourceContent.AccessLocation);
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"HTTP Resposne: {response}");
                return new Version(response);
            }
            catch(Exception e)
            {
                throw new WebRequestException(e.Message);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Installs the driver if it isn't installed
        /// and updates outdated resources
        /// </summary>
        public void Initialize()
        {
            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Checking driver resources");

            if(IsInstalled == false)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager", "Driver is not installed, attempting to install driver resources");
                InstallLatestDriver();
                return;
            }

            var latestVersion = GetLatestVersion();
            var currentVersion = GetCurrentVersion();

            if(currentVersion.CompareTo(latestVersion) != 0)
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
            WebAPI.Google.Content resource;
            var latestVersion = GetLatestVersion();

            var temporaryFileDownloadPath = string.Format($"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}chromedriver_tmp.zip");
            var driverVersionFilePath = $"{DriverDirectoryPath}{Path.DirectorySeparatorChar}version";
            bool permissionsRequired;

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager",
                $"Requesting Google Storage Resource '{latestVersion}/chromedriver_win32.zip'");
            switch (driver.TargetPlatform)
            {
                case Platform.Windows32:
                    resource = WebAPI.Google.Storage.FetchResource($"{latestVersion}/chromedriver_win32.zip");
                    permissionsRequired = false;
                    break;

                case Platform.Windows64:
                    resource = WebAPI.Google.Storage.FetchResource($"{latestVersion}/chromedriver_win32.zip");
                    permissionsRequired = false;
                    break;

                case Platform.Linux32:
                    resource = WebAPI.Google.Storage.FetchResource($"{latestVersion}/chromedriver_linux32.zip");
                    permissionsRequired = true;
                    break;

                case Platform.Linux64:
                    resource = WebAPI.Google.Storage.FetchResource($"{latestVersion}/chromedriver_linux64.zip");
                    permissionsRequired = true;
                    break;

                default:
                    driver.Logging.WriteEntry(MessageType.Error, "DriverManager",
                        $"The platform '{driver.TargetPlatform}' is not supported for this driver");
                    throw new UnsupportedPlatformException();
            }

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Temporary File Download Path: {temporaryFileDownloadPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Executable Name: {DriverExecutableName}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Directory Path: {DriverDirectoryPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Version File Path: {driverVersionFilePath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Permissions Required (chmod): {permissionsRequired}");

            // Check files before modification
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
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Downloading archive from '{resource.AccessLocation}'");
            webClient.DownloadFile(resource.AccessLocation.ToString(), temporaryFileDownloadPath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Download completed");

            // Extract archive and create version file
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Extracting driver resources from archive '{temporaryFileDownloadPath}'");
            FileArchive.ExtractArchive(temporaryFileDownloadPath, DriverDirectoryPath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating version file");
            File.WriteAllText(driverVersionFilePath, latestVersion.ToString());

            // Cleanup temporary files
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Deleting temporary file '{temporaryFileDownloadPath}'");
            File.Delete(temporaryFileDownloadPath);
            
            // Add missing permissions
            if (permissionsRequired)
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Applying executable permissions to '{DriverExecutablePath}'");
                Utilities.GiveExecutablePermissions(DriverExecutablePath);
            }
            
            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "The driver installation has completed successfully");
        }
        
        
    }
}
