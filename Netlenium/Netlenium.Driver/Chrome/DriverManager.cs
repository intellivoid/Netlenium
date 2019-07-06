using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netlenium.Logging;

namespace Netlenium.Driver.Chrome
{
    public class DriverManager : IDriverManager
    {
        private Driver driver;

        private Service logging;
        public Service Logging => driver.Logging;

        public bool IsInstalled
        {
            get
            {
                var DriverDirectoryPath = string.Format("{0}{1}{2}",
                    ApplicationPaths.DriverDirectory, Path.DirectorySeparatorChar,
                    Utilities.GetDriverDirectoryName(driver.TargetPlatform, driver.TargetBrowser)
                );

                if(Directory.Exists(DriverDirectoryPath) == false)
                {
                    return false;
                }

                if(File.Exists(string.Format("{0}{1}{2}", DriverDirectoryPath, Path.DirectorySeparatorChar, "version")) == false)
                {
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

        public DriverManager(Driver driver)
        {
            this.driver = driver;
        }

        public Version GetCurrentVersion()
        {
            throw new NotImplementedException();
        }

        public Version GetLatestVersion()
        {
            throw new NotImplementedException();
        }

        public void Initalize()
        {
            throw new NotImplementedException();
        }
    }
}
