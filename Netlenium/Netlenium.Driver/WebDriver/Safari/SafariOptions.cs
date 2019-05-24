using System;
using System.Collections.Generic;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Safari
{
    /// <summary>
    /// Class to manage options specific to <see cref="SafariDriver"/>
    /// </summary>
    /// <example>
    /// <code>
    /// SafariOptions options = new SafariOptions();
    /// options.SkipExtensionInstallation = true;
    /// </code>
    /// <para></para>
    /// <para>For use with SafariDriver:</para>
    /// <para></para>
    /// <code>
    /// SafariDriver driver = new SafariDriver(options);
    /// </code>
    /// <para></para>
    /// <para>For use with RemoteWebDriver:</para>
    /// <para></para>
    /// <code>
    /// RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
    /// </code>
    /// </example>
    public class SafariOptions : DriverOptions
    {
        private const string BrowserNameValue = "safari";
        private const string TechPreviewBrowserNameValue = "safari technology preview";
        private const string EnableAutomaticInspectionSafariOption = "safari:automaticInspection";
        private const string EnableAutomticProfilingSafariOption = "safari:automaticProfiling";

        private bool enableAutomaticInspection = false;
        private bool enableAutomaticProfiling = false;
        private bool isTechnologyPreview = false;
        private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariOptions"/> class.
        /// </summary>
        public SafariOptions() : base()
        {
            BrowserName = BrowserNameValue;
            AddKnownCapabilityName(EnableAutomaticInspectionSafariOption, "EnableAutomaticInspection property");
            AddKnownCapabilityName(EnableAutomticProfilingSafariOption, "EnableAutomaticProfiling property");
        }

        /// <summary>
        /// Gets or sets a value indicating whether to have the driver preload the
        /// Web Inspector and JavaScript debugger in the background.
        /// </summary>
        public bool EnableAutomaticInspection
        {
            get { return enableAutomaticInspection; }
            set { enableAutomaticInspection = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to have the driver preload the
        /// Web Inspector and start a timeline recording in the background.
        /// </summary>
        public bool EnableAutomaticProfiling
        {
            get { return enableAutomaticProfiling; }
            set { enableAutomaticProfiling = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the browser is the technology preview.
        /// </summary>
        [Obsolete("This property will be removed once the driver for the Safari Technology Preview properly supports the browser name of 'safari'.")]
        public bool IsTechnologyPreview
        {
            get { return isTechnologyPreview; }
            set { isTechnologyPreview = value; }
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Safari driver.
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
        /// Returns ICapabilities for Safari with these options included as
        /// capabilities. This copies the options. Further changes will not be
        /// reflected in the returned capabilities.
        /// </summary>
        /// <returns>The ICapabilities for Safari with these options.</returns>
        public override ICapabilities ToCapabilities()
        {
            if (isTechnologyPreview)
            {
                BrowserName = TechPreviewBrowserNameValue;
            }

            var capabilities = GenerateDesiredCapabilities(true);
            if (enableAutomaticInspection)
            {
                capabilities.SetCapability(EnableAutomaticInspectionSafariOption, true);
            }

            if (enableAutomaticProfiling)
            {
                capabilities.SetCapability(EnableAutomticProfilingSafariOption, true);
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
