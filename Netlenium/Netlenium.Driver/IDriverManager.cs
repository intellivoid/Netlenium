using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver
{
    public interface IDriverManager
    {
        /// <summary>
        /// Gets the latest version of the driver
        /// </summary>
        /// <returns></returns>
        Version GetLatestVersion();

        /// <summary>
        /// Gets the current version that's currently installed
        /// 
        /// Throws an exception if the driver is not installed
        /// </summary>
        /// <returns></returns>
        Version GetCurrentVersion();

        /// <summary>
        /// Determines if the Driver is properly installed
        /// </summary>
        bool IsInstalled { get; }

        /// <summary>
        /// Gets the file name of the executable driver
        /// </summary>
        string DriverExecutableName { get; }

        /// <summary>
        /// The directory path which contains all the driver resources
        /// </summary>
        string DriverDirectoryPath { get; }

        /// <summary>
        /// The full path to the driver executable file
        /// </summary>
        string DriverExecutablePath { get; }

        /// <summary>
        /// Initialize the driver by updating or fetching any missing resources
        /// </summary>
        void Initialize();

    }
}
