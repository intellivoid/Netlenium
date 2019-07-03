using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver
{
    /// <summary>
    /// Driver Details regarding the current installation and configuration
    /// </summary>
    public class DriverInstallationDetails
    {
        /// <summary>
        /// The target this browser this driver is designed for
        /// </summary>
        public Browser TargetBrowser { get; }

        /// <summary>
        /// Indicates if the driver is currently installed or not
        /// </summary>
        public string IsInstalled { get; }

        /// <summary>
        /// The platform that this current installation is targetting
        /// </summary>
        public Platform TargetPlatform { get; }

        /// <summary>
        /// The current version that's currently installed
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// The directory path which contains the driver executable and it's dependencies
        /// </summary>
        public string DriverDirectoryPath { get; }

        /// <summary>
        /// The path to the executable file of the driver
        /// </summary>
        public string DriverExecutablePath { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetBrowser"></param>
        /// <param name="targetPlatform"></param>
        public DriverInstallationDetails(Browser targetBrowser, Platform targetPlatform)
        {

        }
    }
}
