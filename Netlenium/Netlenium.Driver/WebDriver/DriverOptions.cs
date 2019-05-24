using Newtonsoft.Json;
using Netlenium.Driver.WebDriver.Internal;
using Netlenium.Driver.WebDriver.Remote;
using System;
using System.Collections.Generic;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Specifies the behavior of handling unexpected alerts in the IE driver.
    /// </summary>
    public enum UnhandledPromptBehavior
    {
        /// <summary>
        /// Indicates the behavior is not set.
        /// </summary>
        Default,

        /// <summary>
        /// Ignore unexpected alerts, such that the user must handle them.
        /// </summary>
        Ignore,

        /// <summary>
        /// Accept unexpected alerts.
        /// </summary>
        Accept,

        /// <summary>
        /// Dismiss unexpected alerts.
        /// </summary>
        Dismiss,

        /// <summary>
        /// Accepts unexpected alerts and notifies the user that the alert has
        /// been accepted by throwing an <see cref="UnhandledAlertException"/>
        /// </summary>
        AcceptAndNotify,

        /// <summary>
        /// Dismisses unexpected alerts and notifies the user that the alert has
        /// been dismissed by throwing an <see cref="UnhandledAlertException"/>
        /// </summary>
        DismissAndNotify
    }

    /// <summary>
    /// Specifies the behavior of waiting for page loads in the driver.
    /// </summary>
    public enum PageLoadStrategy
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
    /// Base class for managing options specific to a browser driver.
    /// </summary>
    public abstract class DriverOptions
    {
        private string browserName;
        private string browserVersion;
        private string platformName;
        private Proxy proxy;
        private bool? acceptInsecureCertificates;
        private UnhandledPromptBehavior unhandledPromptBehavior = UnhandledPromptBehavior.Default;
        private PageLoadStrategy pageLoadStrategy = PageLoadStrategy.Default;
        private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
        private Dictionary<string, LogLevel> loggingPreferences = new Dictionary<string, LogLevel>();
        private Dictionary<string, string> knownCapabilityNames = new Dictionary<string, string>();

        protected DriverOptions()
        {
            AddKnownCapabilityName(CapabilityType.BrowserName, "BrowserName property");
            AddKnownCapabilityName(CapabilityType.BrowserVersion, "BrowserVersion property");
            AddKnownCapabilityName(CapabilityType.PlatformName, "PlatformName property");
            AddKnownCapabilityName(CapabilityType.Proxy, "Proxy property");
            AddKnownCapabilityName(CapabilityType.UnhandledPromptBehavior, "UnhandledPromptBehavior property");
            AddKnownCapabilityName(CapabilityType.PageLoadStrategy, "PageLoadStrategy property");
        }

        /// <summary>
        /// Gets or sets the name of the browser.
        /// </summary>
        public string BrowserName
        {
            get { return browserName; }
            protected set { browserName = value; }
        }

        /// <summary>
        /// Gets or sets the version of the browser.
        /// </summary>
        public string BrowserVersion
        {
            get { return browserVersion; }
            set { browserVersion = value; }
        }

        /// <summary>
        /// Gets or sets the name of the platform on which the browser is running.
        /// </summary>
        public string PlatformName
        {
            get { return platformName; }
            set { platformName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the browser should accept self-signed
        /// SSL certificates.
        /// </summary>
        public bool? AcceptInsecureCertificates
        {
            get { return acceptInsecureCertificates; }
            set { acceptInsecureCertificates = value; }
        }

        /// <summary>
        /// Gets or sets the value for describing how unexpected alerts are to be handled in the browser.
        /// Defaults to <see cref="UnhandledPromptBehavior.Default"/>.
        /// </summary>
        public UnhandledPromptBehavior UnhandledPromptBehavior
        {
            get { return unhandledPromptBehavior; }
            set { unhandledPromptBehavior = value; }
        }

        /// <summary>
        /// Gets or sets the value for describing how the browser is to wait for pages to load in the browser.
        /// Defaults to <see cref="PageLoadStrategy.Default"/>.
        /// </summary>
        public PageLoadStrategy PageLoadStrategy
        {
            get { return pageLoadStrategy; }
            set { pageLoadStrategy = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Proxy"/> to be used with this browser.
        /// </summary>
        public Proxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the specific browser driver.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to add.</param>
        /// <param name="capabilityValue">The value of the capability to add.</param>
        /// <exception cref="ArgumentException">
        /// thrown when attempting to add a capability for which there is already a type safe option, or
        /// when <paramref name="capabilityName"/> is <see langword="null"/> or the empty string.
        /// </exception>
        /// <remarks>Calling <see cref="AddAdditionalCapability(string, object)"/>
        /// where <paramref name="capabilityName"/> has already been added will overwrite the
        /// existing value with the new value in <paramref name="capabilityValue"/>.
        /// </remarks>
        public abstract void AddAdditionalCapability(string capabilityName, object capabilityValue);

        /// <summary>
        /// Returns the <see cref="ICapabilities"/> for the specific browser driver with these
        /// options included as capabilities. This does not copy the options. Further
        /// changes will be reflected in the returned capabilities.
        /// </summary>
        /// <returns>The <see cref="ICapabilities"/> for browser driver with these options.</returns>
        public abstract ICapabilities ToCapabilities();

        /// <summary>
        /// Compares this <see cref="DriverOptions"/> object with another to see if there
        /// are merge conflicts between them.
        /// </summary>
        /// <param name="other">The <see cref="DriverOptions"/> object to compare with.</param>
        /// <returns>A <see cref="DriverOptionsMergeResult"/> object containing the status of the attempted merge.</returns>
        public virtual DriverOptionsMergeResult GetMergeResult(DriverOptions other)
        {
            var result = new DriverOptionsMergeResult();
            if (browserName != null && other.BrowserName != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "BrowserName";
                return result;
            }

            if (browserVersion != null && other.BrowserVersion != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "BrowserVersion";
                return result;
            }

            if (platformName != null && other.PlatformName != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "PlatformName";
                return result;
            }

            if (proxy != null && other.Proxy != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "Proxy";
                return result;
            }

            if (unhandledPromptBehavior != UnhandledPromptBehavior.Default && other.UnhandledPromptBehavior != UnhandledPromptBehavior.Default)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "UnhandledPromptBehavior";
                return result;
            }

            if (pageLoadStrategy != PageLoadStrategy.Default && other.PageLoadStrategy != PageLoadStrategy.Default)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "PageLoadStrategy";
                return result;
            }

            return result;
        }

        /// <summary>
        /// Sets the logging preferences for this driver.
        /// </summary>
        /// <param name="logType">The type of log for which to set the preference.
        /// Known log types can be found in the <see cref="LogType"/> class.</param>
        /// <param name="logLevel">The <see cref="LogLevel"/> value to which to set the log level.</param>
        public void SetLoggingPreference(string logType, LogLevel logLevel)
        {
            loggingPreferences[logType] = logLevel;
        }

        /// <summary>
        /// Returns a string representation of this <see cref="DriverOptions"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="DriverOptions"/>.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToDictionary(), Formatting.Indented);
        }

        /// <summary>
        /// Returns the current options as a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>The current options as a <see cref="Dictionary{TKey, TValue}"/>.</returns>
        internal Dictionary<string, object> ToDictionary()
        {
            var desired = ToCapabilities() as IHasCapabilitiesDictionary;
            return desired.CapabilitiesDictionary;
        }

        /// <summary>
        /// Adds a known capability to the list of known capabilities and associates it
        /// with the type-safe property name of the options class to be used instead.
        /// </summary>
        /// <param name="capabilityName">The name of the capability.</param>
        /// <param name="typeSafeOptionName">The name of the option property or method to be used instead.</param>
        protected void AddKnownCapabilityName(string capabilityName, string typeSafeOptionName)
        {
            knownCapabilityNames[capabilityName] = typeSafeOptionName;
        }

        /// <summary>
        /// Gets a value indicating whether the specified capability name is a known capability name which has a type-safe option.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to check.</param>
        /// <returns><see langword="true"/> if the capability name is known; otherwise <see langword="false"/>.</returns>
        protected bool IsKnownCapabilityName(string capabilityName)
        {
            return knownCapabilityNames.ContainsKey(capabilityName);
        }

        /// <summary>
        /// Gets the name of the type-safe option for a given capability name.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to check.</param>
        /// <returns>The name of the type-safe option for the given capability name.</returns>
        protected string GetTypeSafeOptionName(string capabilityName)
        {
            if (IsKnownCapabilityName(capabilityName))
            {
                return string.Empty;
            }

            return knownCapabilityNames[capabilityName];
        }

        /// <summary>
        /// Generates the logging preferences dictionary for transmission as a desired capability.
        /// </summary>
        /// <returns>The dictionary containing the logging preferences.</returns>
        protected Dictionary<string, object> GenerateLoggingPreferencesDictionary()
        {
            if (loggingPreferences.Count == 0)
            {
                return null;
            }

            var loggingPreferenceCapability = new Dictionary<string, object>();
            foreach (var logType in loggingPreferences.Keys)
            {
                loggingPreferenceCapability[logType] = loggingPreferences[logType].ToString().ToUpperInvariant();
            }

            return loggingPreferenceCapability;
        }

        /// <summary>
        /// Generates the current options as a capabilities object for further processing.
        /// </summary>
        /// <param name="isSpecificationCompliant">A value indicating whether to generate capabilities compliant with the W3C WebDriver Specification.</param>
        /// <returns>A <see cref="DesiredCapabilities"/> object representing the current options for further processing.</returns>
        protected DesiredCapabilities GenerateDesiredCapabilities(bool isSpecificationCompliant)
        {
            var capabilities = new DesiredCapabilities();
            if (!string.IsNullOrEmpty(browserName))
            {
                capabilities.SetCapability(CapabilityType.BrowserName, browserName);
            }

            if (!string.IsNullOrEmpty(browserVersion))
            {
                capabilities.SetCapability(CapabilityType.BrowserVersion, browserVersion);
            }

            if (!string.IsNullOrEmpty(platformName))
            {
                capabilities.SetCapability(CapabilityType.PlatformName, platformName);
            }

            if (acceptInsecureCertificates.HasValue)
            {
                capabilities.SetCapability(CapabilityType.AcceptInsecureCertificates, acceptInsecureCertificates);
            }

            if (pageLoadStrategy != PageLoadStrategy.Default)
            {
                var pageLoadStrategySetting = "normal";
                switch (pageLoadStrategy)
                {
                    case PageLoadStrategy.Eager:
                        pageLoadStrategySetting = "eager";
                        break;

                    case PageLoadStrategy.None:
                        pageLoadStrategySetting = "none";
                        break;
                }

                capabilities.SetCapability(CapabilityType.PageLoadStrategy, pageLoadStrategySetting);
            }

            if (UnhandledPromptBehavior != UnhandledPromptBehavior.Default)
            {
                var unhandledPropmtBehaviorSetting = "ignore";
                switch (UnhandledPromptBehavior)
                {
                    case UnhandledPromptBehavior.Accept:
                        unhandledPropmtBehaviorSetting = "accept";
                        break;

                    case UnhandledPromptBehavior.Dismiss:
                        unhandledPropmtBehaviorSetting = "dismiss";
                        break;

                    case UnhandledPromptBehavior.AcceptAndNotify:
                        unhandledPropmtBehaviorSetting = "accept and notify";
                        break;

                    case UnhandledPromptBehavior.DismissAndNotify:
                        unhandledPropmtBehaviorSetting = "dismiss and notify";
                        break;
                }

                capabilities.SetCapability(CapabilityType.UnhandledPromptBehavior, unhandledPropmtBehaviorSetting);
            }

            if (Proxy != null)
            {
                var proxyCapability = Proxy.ToCapability();
                if (!isSpecificationCompliant)
                {
                    proxyCapability = Proxy.ToLegacyCapability();
                }

                if (proxyCapability != null)
                {
                    capabilities.SetCapability(CapabilityType.Proxy, proxyCapability);
                }
            }

            return capabilities;
        }
    }
}
