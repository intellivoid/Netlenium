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
        /// Intializes the driver by updating or fetching any missing resources
        /// </summary>
        void Initalize();

    }
}
