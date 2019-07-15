using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public bool IsInstalled => throw new NotImplementedException();

        public string DriverExecutableName => throw new NotImplementedException();

        public string DriverDirectoryPath => throw new NotImplementedException();

        public string DriverExecutablePath => throw new NotImplementedException();

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
