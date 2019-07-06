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
            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Checking driver resources");

            if(IsInstalled == false)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager", "Driver is not installed, attempting to install driver resources");
                Install();
                return;
            }

            var LatestVersion = GetLatestVersion();
            var CurrentVersion = GetCurrentVersion();

            if(CurrentVersion.CompareTo(LatestVersion) != 0)
            {
                driver.Logging.WriteEntry(MessageType.Warning, "DriverManager", string.Format("The current driver ({0}) does not match the latest version {1}", CurrentVersion, LatestVersion));
                driver.Logging.WriteEntry(MessageType.Information, "DriverManager", "Attempting to update the driver resources");
                Update();
                return;
            }

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", "Driver resources check passed");
            return;
        }

        /// <summary>
        /// Installs the driver
        /// </summary>
        public void Install()
        {
            WebAPI.Google.Content Resource;
            var LatestVersion = GetLatestVersion();

            var TemporaryFileDownloadPath = string.Format($"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}chromedriver_tmp.zip");
            var DriverExecutableName = string.Empty;
            var DriverDirectoryPath = Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser);
            var DriverVersionFile = $"{DriverDirectoryPath}{Path.DirectorySeparatorChar}version";
            var PermissionsRequired = false;

            driver.Logging.WriteEntry(MessageType.Verbose, "DriverManager", string.Format("Requesting Google Storage Resource '{0}'", $"{LatestVersion}/chromedriver_win32.zip"));
            switch (driver.TargetPlatform)
            {
                case Platform.Windows:
                    Resource = WebAPI.Google.Storage.FetchResource($"{LatestVersion}/chromedriver_win32.zip");
                    DriverExecutableName = "chromedriver.exe";
                    PermissionsRequired = false;
                    break;

                case Platform.Linux32:
                    Resource = WebAPI.Google.Storage.FetchResource($"{LatestVersion}/chromedriver_linux32.zip");
                    DriverExecutableName = "chromedriver.exe";
                    break;

                case Platform.Linux64:
                    Resource = WebAPI.Google.Storage.FetchResource($"{LatestVersion}/chromedriver_linux64.zip");
                    DriverExecutableName = "chromedriver.exe";
                    break;

                default:
                    driver.Logging.WriteEntry(MessageType.Error, "DriverManager", string.Format("The platform '{0}' is not supported for this driver", driver.TargetPlatform));
                    throw new UnsupportedPlatformException();
            }

           
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
