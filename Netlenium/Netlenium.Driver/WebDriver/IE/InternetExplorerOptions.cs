using System;
using System.Collections.Generic;
using System.Globalization;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.IE
{
    /// <summary>
    /// Specifies the scroll behavior of elements scrolled into view in the IE driver.
    /// </summary>
    public enum InternetExplorerElementScrollBehavior
    {
        /// <summary>
        /// Indicates the behavior is unspecified.
        /// </summary>
        Default,

        /// <summary>
        /// Scrolls elements to align with the top of the viewport.
        /// </summary>
        Top,

        /// <summary>
        /// Scrolls elements to align with the bottom of the viewport.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Class to manage options specific to <see cref="InternetExplorerDriver"/>
    /// </summary>
    /// <example>
    /// <code>
    /// InternetExplorerOptions options = new InternetExplorerOptions();
    /// options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
    /// </code>
    /// <para></para>
    /// <para>For use with InternetExplorerDriver:</para>
    /// <para></para>
    /// <code>
    /// InternetExplorerDriver driver = new InternetExplorerDriver(options);
    /// </code>
    /// <para></para>
    /// <para>For use with RemoteWebDriver:</para>
    /// <para></para>
    /// <code>
    /// RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
    /// </code>
    /// </example>
    public class InternetExplorerOptions : DriverOptions
    {
        /// <summary>
        /// Gets the name of the capability used to store IE options in
        /// a <see cref="DesiredCapabilities"/> object.
        /// </summary>
        public static readonly string Capability = "se:ieOptions";

        private const string BrowserNameValue = "internet explorer";

        private const string IgnoreProtectedModeSettingsCapability = "ignoreProtectedModeSettings";
        private const string IgnoreZoomSettingCapability = "ignoreZoomSetting";
        private const string InitialBrowserUrlCapability = "initialBrowserUrl";
        private const string EnablePersistentHoverCapability = "enablePersistentHover";
        private const string ElementScrollBehaviorCapability = "elementScrollBehavior";
        private const string RequireWindowFocusCapability = "requireWindowFocus";
        private const string BrowserAttachTimeoutCapability = "browserAttachTimeout";
        private const string BrowserCommandLineSwitchesCapability = "ie.browserCommandLineSwitches";
        private const string ForceCreateProcessApiCapability = "ie.forceCreateProcessApi";
        private const string UsePerProcessProxyCapability = "ie.usePerProcessProxy";
        private const string EnsureCleanSessionCapability = "ie.ensureCleanSession";
        private const string ForceShellWindowsApiCapability = "ie.forceShellWindowsApi";
        private const string FileUploadDialogTimeoutCapability = "ie.fileUploadDialogTimeout";
        private const string EnableFullPageScreenshotCapability = "ie.enableFullPageScreenshot";

        private bool ignoreProtectedModeSettings;
        private bool ignoreZoomLevel;
        private bool enableNativeEvents = true;
        private bool requireWindowFocus;
        private bool enablePersistentHover = true;
        private bool forceCreateProcessApi;
        private bool forceShellWindowsApi;
        private bool usePerProcessProxy;
        private bool ensureCleanSession;
        //private bool validateCookieDocumentType = true;
        private bool enableFullPageScreenshot = true;
        private TimeSpan browserAttachTimeout = TimeSpan.MinValue;
        private TimeSpan fileUploadDialogTimeout = TimeSpan.MinValue;
        private string initialBrowserUrl = string.Empty;
        private string browserCommandLineArguments = string.Empty;
        private InternetExplorerElementScrollBehavior elementScrollBehavior = InternetExplorerElementScrollBehavior.Default;
        private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
        private Dictionary<string, object> additionalInternetExplorerOptions = new Dictionary<string, object>();

        public InternetExplorerOptions() : base()
        {
            BrowserName = BrowserNameValue;
            PlatformName = "windows";
            AddKnownCapabilityName(Capability, "current InterentExplorerOptions class instance");
            AddKnownCapabilityName(IgnoreProtectedModeSettingsCapability, "IntroduceInstabilityByIgnoringProtectedModeSettings property");
            AddKnownCapabilityName(IgnoreZoomSettingCapability, "IgnoreZoomLevel property");
            AddKnownCapabilityName(CapabilityType.HasNativeEvents, "EnableNativeEvents property");
            AddKnownCapabilityName(InitialBrowserUrlCapability, "InitialBrowserUrl property");
            AddKnownCapabilityName(ElementScrollBehaviorCapability, "ElementScrollBehavior property");
            AddKnownCapabilityName(CapabilityType.UnexpectedAlertBehavior, "UnhandledPromptBehavior property");
            AddKnownCapabilityName(EnablePersistentHoverCapability, "EnablePersistentHover property");
            AddKnownCapabilityName(RequireWindowFocusCapability, "RequireWindowFocus property");
            AddKnownCapabilityName(BrowserAttachTimeoutCapability, "BrowserAttachTimeout property");
            AddKnownCapabilityName(ForceCreateProcessApiCapability, "ForceCreateProcessApi property");
            AddKnownCapabilityName(ForceShellWindowsApiCapability, "ForceShellWindowsApi property");
            AddKnownCapabilityName(BrowserCommandLineSwitchesCapability, "BrowserComaandLineArguments property");
            AddKnownCapabilityName(UsePerProcessProxyCapability, "UsePerProcessProxy property");
            AddKnownCapabilityName(EnsureCleanSessionCapability, "EnsureCleanSession property");
            AddKnownCapabilityName(FileUploadDialogTimeoutCapability, "FileUploadDialogTimeout property");
            AddKnownCapabilityName(EnableFullPageScreenshotCapability, "EnableFullPageScreenshot property");
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the settings of the Internet Explorer Protected Mode.
        /// </summary>
        public bool IntroduceInstabilityByIgnoringProtectedModeSettings
        {
            get { return ignoreProtectedModeSettings; }
            set { ignoreProtectedModeSettings = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the zoom level of Internet Explorer .
        /// </summary>
        public bool IgnoreZoomLevel
        {
            get { return ignoreZoomLevel; }
            set { ignoreZoomLevel = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use native events in interacting with elements.
        /// </summary>
        public bool EnableNativeEvents
        {
            get { return enableNativeEvents; }
            set { enableNativeEvents = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to require the browser window to have focus before interacting with elements.
        /// </summary>
        public bool RequireWindowFocus
        {
            get { return requireWindowFocus; }
            set { requireWindowFocus = value; }
        }

        /// <summary>
        /// Gets or sets the initial URL displayed when IE is launched. If not set, the browser launches
        /// with the internal startup page for the WebDriver server.
        /// </summary>
        /// <remarks>
        /// By setting the  <see cref="IntroduceInstabilityByIgnoringProtectedModeSettings"/> to <see langword="true"/>
        /// and this property to a correct URL, you can launch IE in the Internet Protected Mode zone. This can be helpful
        /// to avoid the flakiness introduced by ignoring the Protected Mode settings. Nevertheless, setting Protected Mode
        /// zone settings to the same value in the IE configuration is the preferred method.
        /// </remarks>
        public string InitialBrowserUrl
        {
            get { return initialBrowserUrl; }
            set { initialBrowserUrl = value; }
        }

        /// <summary>
        /// Gets or sets the value for describing how elements are scrolled into view in the IE driver. Defaults
        /// to scrolling the element to the top of the viewport.
        /// </summary>
        public InternetExplorerElementScrollBehavior ElementScrollBehavior
        {
            get { return elementScrollBehavior; }
            set { elementScrollBehavior = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable persistently sending WM_MOUSEMOVE messages
        /// to the IE window during a mouse hover.
        /// </summary>
        public bool EnablePersistentHover
        {
            get { return enablePersistentHover; }
            set { enablePersistentHover = value; }
        }

        /// <summary>
        /// Gets or sets the amount of time the driver will attempt to look for a newly launched instance
        /// of Internet Explorer.
        /// </summary>
        public TimeSpan BrowserAttachTimeout
        {
            get { return browserAttachTimeout; }
            set { browserAttachTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the amount of time the driver will attempt to look for the file selection
        /// dialog when attempting to upload a file.
        /// </summary>
        public TimeSpan FileUploadDialogTimeout
        {
            get { return fileUploadDialogTimeout; }
            set { fileUploadDialogTimeout = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force the use of the Windows CreateProcess API
        /// when launching Internet Explorer. The default value is <see langword="false"/>.
        /// </summary>
        public bool ForceCreateProcessApi
        {
            get { return forceCreateProcessApi; }
            set { forceCreateProcessApi = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force the use of the Windows ShellWindows API
        /// when attaching to Internet Explorer. The default value is <see langword="false"/>.
        /// </summary>
        public bool ForceShellWindowsApi
        {
            get { return forceShellWindowsApi; }
            set { forceShellWindowsApi = value; }
        }

        /// <summary>
        /// Gets or sets the command line arguments used in launching Internet Explorer when the
        /// Windows CreateProcess API is used. This property only has an effect when the
        /// <see cref="ForceCreateProcessApi"/> is <see langword="true"/>.
        /// </summary>
        public string BrowserCommandLineArguments
        {
            get { return browserCommandLineArguments; }
            set { browserCommandLineArguments = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the supplied <see cref="Proxy"/>
        /// settings on a per-process basis, not updating the system installed proxy setting.
        /// This property is only valid when setting a <see cref="Proxy"/>, where the
        /// <see cref="Netlenium.Driver.WebDriver.Proxy.Kind"/> property is either <see cref="ProxyKind.Direct"/>,
        /// <see cref="ProxyKind.System"/>, or <see cref="ProxyKind.Manual"/>, and is
        /// otherwise ignored. Defaults to <see langword="false"/>.
        /// </summary>
        public bool UsePerProcessProxy
        {
            get { return usePerProcessProxy; }
            set { usePerProcessProxy = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to clear the Internet Explorer cache
        /// before launching the browser. When set to <see langword="true"/>, clears the
        /// system cache for all instances of Internet Explorer, even those already running
        /// when the driven instance is launched. Defaults to <see langword="false"/>.
        /// </summary>
        public bool EnsureCleanSession
        {
            get { return ensureCleanSession; }
            set { ensureCleanSession = value; }
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Internet Explorer driver.
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
        /// Also, by default, calling this method adds capabilities to the options object passed to
        /// IEDriverServer.exe.</remarks>
        public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
        {
            // Add the capability to the ieOptions object by default. This is to handle
            // the 80% case where the IE driver adds a new option in IEDriverServer.exe
            // and the bindings have not yet had a type safe option added.
            AddAdditionalCapability(capabilityName, capabilityValue, false);
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Internet Explorer driver.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to add.</param>
        /// <param name="capabilityValue">The value of the capability to add.</param>
        /// <param name="isGlobalCapability">Indicates whether the capability is to be set as a global
        /// capability for the driver instead of a IE-specific option.</param>
        /// <exception cref="ArgumentException">
        /// thrown when attempting to add a capability for which there is already a type safe option, or
        /// when <paramref name="capabilityName"/> is <see langword="null"/> or the empty string.
        /// </exception>
        /// <remarks>Calling <see cref="AddAdditionalCapability(string, object, bool)"/> where <paramref name="capabilityName"/>
        /// has already been added will overwrite the existing value with the new value in <paramref name="capabilityValue"/></remarks>
        public void AddAdditionalCapability(string capabilityName, object capabilityValue, bool isGlobalCapability)
        {
            if (IsKnownCapabilityName(capabilityName))
            {
                var typeSafeOptionName = GetTypeSafeOptionName(capabilityName);
                var message = string.Format(CultureInfo.InvariantCulture, "There is already an option for the {0} capability. Please use the {1} instead.", capabilityName, typeSafeOptionName);
                throw new ArgumentException(message, "capabilityName");
            }

            if (string.IsNullOrEmpty(capabilityName))
            {
                throw new ArgumentException("Capability name may not be null an empty string.", "capabilityName");
            }

            if (isGlobalCapability)
            {
                additionalCapabilities[capabilityName] = capabilityValue;
            }
            else
            {
                additionalInternetExplorerOptions[capabilityName] = capabilityValue;
            }
        }

        /// <summary>
        /// Returns DesiredCapabilities for IE with these options included as
        /// capabilities. This copies the options. Further changes will not be
        /// reflected in the returned capabilities.
        /// </summary>
        /// <returns>The DesiredCapabilities for IE with these options.</returns>
        public override ICapabilities ToCapabilities()
        {
            var capabilities = GenerateDesiredCapabilities(true);

            var internetExplorerOptions = BuildInternetExplorerOptionsDictionary();
            capabilities.SetCapability(Capability, internetExplorerOptions);

            foreach (var pair in additionalCapabilities)
            {
                capabilities.SetCapability(pair.Key, pair.Value);
            }

            // Should return capabilities.AsReadOnly(), and will in a future release.
            return capabilities;
        }

        private Dictionary<string, object> BuildInternetExplorerOptionsDictionary()
        {
            var internetExplorerOptionsDictionary = new Dictionary<string, object>();
            internetExplorerOptionsDictionary[CapabilityType.HasNativeEvents] = enableNativeEvents;
            internetExplorerOptionsDictionary[EnablePersistentHoverCapability] = enablePersistentHover;

            if (requireWindowFocus)
            {
                internetExplorerOptionsDictionary[RequireWindowFocusCapability] = true;
            }

            if (ignoreProtectedModeSettings)
            {
                internetExplorerOptionsDictionary[IgnoreProtectedModeSettingsCapability] = true;
            }

            if (ignoreZoomLevel)
            {
                internetExplorerOptionsDictionary[IgnoreZoomSettingCapability] = true;
            }

            if (!string.IsNullOrEmpty(initialBrowserUrl))
            {
                internetExplorerOptionsDictionary[InitialBrowserUrlCapability] = initialBrowserUrl;
            }

            if (elementScrollBehavior != InternetExplorerElementScrollBehavior.Default)
            {
                if (elementScrollBehavior == InternetExplorerElementScrollBehavior.Bottom)
                {
                    internetExplorerOptionsDictionary[ElementScrollBehaviorCapability] = 1;
                }
                else
                {
                    internetExplorerOptionsDictionary[ElementScrollBehaviorCapability] = 0;
                }
            }

            if (browserAttachTimeout != TimeSpan.MinValue)
            {
                internetExplorerOptionsDictionary[BrowserAttachTimeoutCapability] = Convert.ToInt32(browserAttachTimeout.TotalMilliseconds);
            }

            if (fileUploadDialogTimeout != TimeSpan.MinValue)
            {
                internetExplorerOptionsDictionary[FileUploadDialogTimeoutCapability] = Convert.ToInt32(fileUploadDialogTimeout.TotalMilliseconds);
            }

            if (forceCreateProcessApi)
            {
                internetExplorerOptionsDictionary[ForceCreateProcessApiCapability] = true;
                if (!string.IsNullOrEmpty(browserCommandLineArguments))
                {
                    internetExplorerOptionsDictionary[BrowserCommandLineSwitchesCapability] = browserCommandLineArguments;
                }
            }

            if (forceShellWindowsApi)
            {
                internetExplorerOptionsDictionary[ForceShellWindowsApiCapability] = true;
            }

            if (Proxy != null)
            {
                internetExplorerOptionsDictionary[UsePerProcessProxyCapability] = usePerProcessProxy;
            }

            if (ensureCleanSession)
            {
                internetExplorerOptionsDictionary[EnsureCleanSessionCapability] = true;
            }

            if (!enableFullPageScreenshot)
            {
                internetExplorerOptionsDictionary[EnableFullPageScreenshotCapability] = false;
            }

            foreach (var pair in additionalInternetExplorerOptions)
            {
                internetExplorerOptionsDictionary[pair.Key] = pair.Value;
            }

            return internetExplorerOptionsDictionary;
        }
    }
}
