using System;
using System.IO;
using System.Net;
using Ionic.Zip;
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
        private Driver driver;
        
        /// <summary>
        /// Determines if the Driver is currently installed
        /// </summary>
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
                if(Directory.Exists(DriverDirectoryPath) == false)
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

                switch(driver.TargetPlatform)
                {
                    case Platform.Linux32:
                        if (File.Exists(string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, "chromedriver")) == false)
                        {
                            return false;
                        }
                        break;

                    case Platform.Linux64:
                        if (File.Exists(string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, "chromedriver")) == false)
                        {
                            return false;
                        }
                        break;

                    case Platform.Windows:
                        if (File.Exists(string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, "chromedriver.exe")) == false)
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
                    case Platform.Windows:
                        return "chromedriver.exe";

                    case Platform.Linux32:
                        return "chromedriver";

                    case Platform.Linux64:
                        return "chromedriver";

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

        /// <summary>
        /// Gets the current version that's installed
        /// 
        /// Throws an exception if the driver is not installed
        /// </summary>
        /// <returns></returns>
        public Version GetCurrentVersion()
        {
            if(IsInstalled == false)
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

        /// <summary>
        /// Gets the current version that's publicly available from
        /// Google's storage content
        /// </summary>
        /// <returns></returns>
        public Version GetLatestVersion()
        {
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating HTTP/S Request to Google Storage for Resource 'LATEST_RELEASE'");

            var ResourceContent = WebAPI.Google.Storage.FetchResource("LATEST_RELEASE");

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("Access Location => {0}", ResourceContent.AccessLocation.ToString()));
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("ETag => {0}", ResourceContent.ETag));
            
            try
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", string.Format("Creating HTTP/S Request to {0}", ResourceContent.AccessLocation.ToString()));
                var httpWebClient = new WebClient();
                var response = httpWebClient.DownloadString(ResourceContent.AccessLocation);
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", string.Format("HTTP Resposne: {0}", response));
                return new Version(response);
            }
            catch(Exception e)
            {
                throw new WebRequestException(e.Message);
            }
        }

        /// <summary>
        /// Installs the driver if it isn't installed
        /// and updates outdated resources
        /// </summary>
        public void Initalize()
        {
            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Checking driver resources");

            if(IsInstalled == false)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager", "Driver is not installed, attempting to install driver resources");
                InstallLatestDriver();
                return;
            }

            var LatestVersion = GetLatestVersion();
            var CurrentVersion = GetCurrentVersion();

            if(CurrentVersion.CompareTo(LatestVersion) != 0)
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

            var TemporaryFileDownloadPath = string.Format($"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}chromedriver_tmp.zip");
            var DriverVersionFilePath = $"{DriverDirectoryPath}{Path.DirectorySeparatorChar}version";
            var PermissionsRequired = false;

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", string.Format("Requesting Google Storage Resource '{0}'", $"{LatestVersion}/chromedriver_win32.zip"));
            switch (driver.TargetPlatform)
            {
                case Platform.Windows:
                    Resource = WebAPI.Google.Storage.FetchResource($"{LatestVersion}/chromedriver_win32.zip");
                    PermissionsRequired = false;
                    break;

                case Platform.Linux32:
                    Resource = WebAPI.Google.Storage.FetchResource($"{LatestVersion}/chromedriver_linux32.zip");
                    PermissionsRequired = true;
                    break;

                case Platform.Linux64:
                    Resource = WebAPI.Google.Storage.FetchResource($"{LatestVersion}/chromedriver_linux64.zip");
                    PermissionsRequired = true;
                    break;

                default:
                    driver.Logging.WriteEntry(MessageType.Error, "DriverManager", string.Format("The platform '{0}' is not supported for this driver", driver.TargetPlatform));
                    throw new UnsupportedPlatformException();
            }

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Temporary File Download Path: {TemporaryFileDownloadPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Executable Name: {DriverExecutableName}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Directory Path: {DriverDirectoryPath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Driver Version File Path: {DriverVersionFilePath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Permissions Required (chmod): {PermissionsRequired}");

            // Check files before modification
            if(Directory.Exists(DriverDirectoryPath) == true)
            {
                if (File.Exists(TemporaryFileDownloadPath) == true)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"The file '{TemporaryFileDownloadPath}' already exists, this file will be deleted");
                    File.Delete(TemporaryFileDownloadPath);
                }

                if (File.Exists($"{DriverDirectoryPath}{Path.DirectorySeparatorChar}{DriverExecutableName}") == true)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"The file '{DriverDirectoryPath}{Path.DirectorySeparatorChar}{DriverExecutableName}' already exists, this file will be deleted");
                    File.Delete($"{DriverDirectoryPath}{Path.DirectorySeparatorChar}{DriverExecutableName}");
                }

                if (File.Exists(DriverVersionFilePath) == true)
                {
                    driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"The file '{DriverVersionFilePath}' already exists, this file will be deleted");
                    File.Delete(DriverVersionFilePath);
                }
            }
            else
            {
                driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Creating directory '{DriverDirectoryPath}'");
                Directory.CreateDirectory(DriverDirectoryPath);
            }

            // Download archive and extract contents
            var webClient = new WebClient();
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Downloading archive from '{Resource.AccessLocation.ToString()}'");
            webClient.DownloadFile(Resource.AccessLocation.ToString(), TemporaryFileDownloadPath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Download completed");

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Reading archive '{TemporaryFileDownloadPath}'");
            var zip = ZipFile.Read(TemporaryFileDownloadPath);

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Extracting driver resources from archive");
            foreach (var entry in zip)
            {
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Reading '{entry}'");
                if (entry.FileName != DriverExecutableName) continue;
                if (entry.IsDirectory) continue;
                driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Extracting '{entry}'");
                entry.Extract(ApplicationPaths.TemporaryDirectory, ExtractExistingFileAction.OverwriteSilently);
                break;
            }

            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", "Disposing acrhive object");
            zip.Dispose();

            // Copy the files over the destination
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Creating version file");
            File.WriteAllText(DriverVersionFilePath, LatestVersion.ToString());

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Copying '{DriverExecutableName}' to '{DriverDirectoryPath}'");
            var DriverSourcePath = $"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}{DriverExecutableName}";
            var DriverDestinationPath = DriverExecutablePath
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Source Path: {DriverSourcePath}");
            driver.Logging.WriteEntry(MessageType.Debugging, "DriverManager", $"Destination Path: {DriverDestinationPath}");
            File.Copy(DriverSourcePath, DriverDestinationPath);

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Deleting temporary file '{DriverSourcePath}'");
            File.Delete(DriverSourcePath);
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", $"Deleting temporary file '{TemporaryFileDownloadPath}'");
            File.Delete(DriverSourcePath);

            driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "The driver installalation has completed successfully");
        }
        
        
    }
}
