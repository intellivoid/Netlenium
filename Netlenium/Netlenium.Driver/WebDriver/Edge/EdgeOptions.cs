using System;
using System.Collections.Generic;
using System.Globalization;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Edge
{
    /// <summary>
    /// Specifies the behavior of waiting for page loads in the Edge driver.
    /// </summary>
    public enum EdgePageLoadStrategy
    {
        /// <summary>
        /// Indicates the behavior is not set.
        /// </summary>
        Default,

        /// <summary>
        /// Waits for pages to load and ready state to be 'complete'.
        /// </summary>
        Normal,

        /// <summary>
        /// Waits for pages to load and for ready state to be 'interactive' or 'complete'.
        /// </summary>
        Eager,

        /// <summary>
        /// Does not wait for pages to load, returning immediately.
        /// </summary>
        None
    }

    /// <summary>
    /// Class to manage options specific to <see cref="EdgeDriver"/>
    /// </summary>
    /// <example>
    /// <code>
    /// EdgeOptions options = new EdgeOptions();
    /// </code>
    /// <para></para>
    /// <para>For use with EdgeDriver:</para>
    /// <para></para>
    /// <code>
    /// EdgeDriver driver = new EdgeDriver(options);
    /// </code>
    /// <para></para>
    /// <para>For use with RemoteWebDriver:</para>
    /// <para></para>
    /// <code>
    /// RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
    /// </code>
    /// </example>
    public class EdgeOptions : DriverOptions
    {
        private const string BrowserNameValue = "MicrosoftEdge";
        private const string UseInPrivateBrowsingCapability = "ms:inPrivate";
        private const string ExtensionPathsCapability = "ms:extensionPaths";
        private const string StartPageCapability = "ms:startPage";

        //private EdgePageLoadStrategy pageLoadStrategy = EdgePageLoadStrategy.Default;
        private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
        private bool useInPrivateBrowsing;
        private string startPage;
        private List<string> extensionPaths = new List<string>();

        public EdgeOptions() : base()
        {
            BrowserName = BrowserNameValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the browser should be launched using
        /// InPrivate browsing.
        /// </summary>
        public bool UseInPrivateBrowsing
        {
            get { return useInPrivateBrowsing; }
            set { useInPrivateBrowsing = value; }
        }

        /// <summary>
        /// Gets or sets the URL of the page with which the browser will be navigated to on launch.
        /// </summary>
        public string StartPage
        {
            get { return startPage; }
            set { startPage = value; }
        }


        /// <summary>
        /// Adds a path to an extension that is to be used with the Edge driver.
        /// </summary>
        /// <param name="extensionPath">The full path and file name of the extension.</param>
        public void AddExtensionPath(string extensionPath)
        {
            if (string.IsNullOrEmpty(extensionPath))
            {
                throw new ArgumentException("extensionPath must not be null or empty", "extensionPath");
            }

            AddExtensionPaths(extensionPath);
        }

        /// <summary>
        /// Adds a list of paths to an extensions that are to be used with the Edge driver.
        /// </summary>
        /// <param name="extensionPathsToAdd">An array of full paths with file names of extensions to add.</param>
        public void AddExtensionPaths(params string[] extensionPathsToAdd)
        {
            AddExtensionPaths(new List<string>(extensionPathsToAdd));
        }

        /// <summary>
        /// Adds a list of paths to an extensions that are to be used with the Edge driver.
        /// </summary>
        /// <param name="extensionPathsToAdd">An <see cref="IEnumerable{T}"/> of full paths with file names of extensions to add.</param>
        public void AddExtensionPaths(IEnumerable<string> extensionPathsToAdd)
        {
            if (extensionPathsToAdd == null)
            {
                throw new ArgumentNullException("extensionPathsToAdd", "extensionPathsToAdd must not be null");
            }

            extensionPaths.AddRange(extensionPathsToAdd);
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Edge driver.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to add.</param>
        /// <param name="capabilityValue">The value of the capability to add.</param>
        /// <exception cref="ArgumentException">
        /// thrown when attempting to add a capability for which there is already a type safe option, or
        /// when <paramref name="capabilityName"/> is <see langword="null"/> or the empty string.
        /// </exception>
        /// <remarks>Calling <see cref="AddAdditionalCapability"/> where <paramref name="capabilityName"/>
        /// has already been added will overwrite the existing value with the new value in <paramref name="capabilityValue"/></remarks>
        public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
        {
            if (string.IsNullOrEmpty(capabilityName))
            {
                throw new ArgumentException("Capability name may not be null an empty string.", "capabilityName");
            }

            additionalCapabilities[capabilityName] = capabilityValue;
        }

        /// <summary>
        /// Returns DesiredCapabilities for Edge with these options included as
        /// capabilities. This copies the options. Further changes will not be
        /// reflected in the returned capabilities.
        /// </summary>
        /// <returns>The DesiredCapabilities for Edge with these options.</returns>
        public override ICapabilities ToCapabilities()
        {
            var capabilities = GenerateDesiredCapabilities(false);

            if (useInPrivateBrowsing)
            {
                capabilities.SetCapability(UseInPrivateBrowsingCapability, true);
            }

            if (!string.IsNullOrEmpty(startPage))
            {
                capabilities.SetCapability(StartPageCapability, startPage);
            }

            if (extensionPaths.Count > 0)
            {
                capabilities.SetCapability(ExtensionPathsCapability, extensionPaths);
            }

            foreach (var pair in additionalCapabilities)
            {
                capabilities.SetCapability(pair.Key, pair.Value);
            }

            // Should return capabilities.AsReadOnly(), and will in a future release.
            return capabilities;
        }
    }
}
