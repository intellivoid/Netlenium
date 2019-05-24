using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Chrome
{
    /// <summary>
    /// Class to manage options specific to <see cref="ChromeDriver"/>
    /// </summary>
    /// <remarks>
    /// Used with ChromeDriver.exe v17.0.963.0 and higher.
    /// </remarks>
    /// <example>
    /// <code>
    /// ChromeOptions options = new ChromeOptions();
    /// options.AddExtensions("\path\to\extension.crx");
    /// options.BinaryLocation = "\path\to\chrome";
    /// </code>
    /// <para></para>
    /// <para>For use with ChromeDriver:</para>
    /// <para></para>
    /// <code>
    /// ChromeDriver driver = new ChromeDriver(options);
    /// </code>
    /// <para></para>
    /// <para>For use with RemoteWebDriver:</para>
    /// <para></para>
    /// <code>
    /// RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
    /// </code>
    /// </example>
    public class ChromeOptions : DriverOptions
    {
        /// <summary>
        /// Gets the name of the capability used to store Chrome options in
        /// a <see cref="DesiredCapabilities"/> object.
        /// </summary>
        public static readonly string Capability = "goog:chromeOptions";

        // TODO: Remove this if block when chromedriver bug 2371 is fixed
        // (https://bugs.chromium.org/p/chromedriver/issues/detail?id=2371)
        internal static readonly string ForceAlwaysMatchCapabilityName = "se:forceAlwaysMatch";

        private const string BrowserNameValue = "chrome";

        private const string ArgumentsChromeOption = "args";
        private const string BinaryChromeOption = "binary";
        private const string ExtensionsChromeOption = "extensions";
        private const string LocalStateChromeOption = "localState";
        private const string PreferencesChromeOption = "prefs";
        private const string DetachChromeOption = "detach";
        private const string DebuggerAddressChromeOption = "debuggerAddress";
        private const string ExcludeSwitchesChromeOption = "excludeSwitches";
        private const string MinidumpPathChromeOption = "minidumpPath";
        private const string MobileEmulationChromeOption = "mobileEmulation";
        private const string PerformanceLoggingPreferencesChromeOption = "perfLoggingPrefs";
        private const string WindowTypesChromeOption = "windowTypes";
        private const string UseSpecCompliantProtocolOption = "w3c";

        private bool leaveBrowserRunning;
        private bool useSpecCompliantProtocol;
        private string binaryLocation;
        private string debuggerAddress;
        private string minidumpPath;
        private List<string> arguments = new List<string>();
        private List<string> extensionFiles = new List<string>();
        private List<string> encodedExtensions = new List<string>();
        private List<string> excludedSwitches = new List<string>();
        private List<string> windowTypes = new List<string>();
        private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
        private Dictionary<string, object> additionalChromeOptions = new Dictionary<string, object>();
        private Dictionary<string, object> userProfilePreferences;
        private Dictionary<string, object> localStatePreferences;

        private string mobileEmulationDeviceName;
        private ChromeMobileEmulationDeviceSettings mobileEmulationDeviceSettings;
        private ChromePerformanceLoggingPreferences perfLoggingPreferences;

        public ChromeOptions() : base()
        {
            BrowserName = BrowserNameValue;
            AddKnownCapabilityName(Capability, "current ChromeOptions class instance");
            AddKnownCapabilityName(CapabilityType.LoggingPreferences, "SetLoggingPreference method");
            AddKnownCapabilityName(ArgumentsChromeOption, "AddArguments method");
            AddKnownCapabilityName(BinaryChromeOption, "BinaryLocation property");
            AddKnownCapabilityName(ExtensionsChromeOption, "AddExtensions method");
            AddKnownCapabilityName(LocalStateChromeOption, "AddLocalStatePreference method");
            AddKnownCapabilityName(PreferencesChromeOption, "AddUserProfilePreference method");
            AddKnownCapabilityName(DetachChromeOption, "LeaveBrowserRunning property");
            AddKnownCapabilityName(DebuggerAddressChromeOption, "DebuggerAddress property");
            AddKnownCapabilityName(ExcludeSwitchesChromeOption, "AddExcludedArgument property");
            AddKnownCapabilityName(MinidumpPathChromeOption, "MinidumpPath property");
            AddKnownCapabilityName(MobileEmulationChromeOption, "EnableMobileEmulation method");
            AddKnownCapabilityName(PerformanceLoggingPreferencesChromeOption, "PerformanceLoggingPreferences property");
            AddKnownCapabilityName(WindowTypesChromeOption, "AddWindowTypes method");
            AddKnownCapabilityName(UseSpecCompliantProtocolOption, "UseSpecCompliantProtocol property");
            AddKnownCapabilityName(ForceAlwaysMatchCapabilityName, "");
        }

        /// <summary>
        /// Gets or sets the location of the Chrome browser's binary executable file.
        /// </summary>
        public string BinaryLocation
        {
            get { return binaryLocation; }
            set { binaryLocation = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Chrome should be left running after the
        /// ChromeDriver instance is exited. Defaults to <see langword="false"/>.
        /// </summary>
        public bool LeaveBrowserRunning
        {
            get { return leaveBrowserRunning; }
            set { leaveBrowserRunning = value; }
        }

        /// <summary>
        /// Gets the list of arguments appended to the Chrome command line as a string array.
        /// </summary>
        public ReadOnlyCollection<string> Arguments
        {
            get { return arguments.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the list of extensions to be installed as an array of base64-encoded strings.
        /// </summary>
        public ReadOnlyCollection<string> Extensions
        {
            get
            {
                var allExtensions = new List<string>(encodedExtensions);
                foreach (var extensionFile in extensionFiles)
                {
                    var extensionByteArray = File.ReadAllBytes(extensionFile);
                    var encodedExtension = Convert.ToBase64String(extensionByteArray);
                    allExtensions.Add(encodedExtension);
                }

                return allExtensions.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the address of a Chrome debugger server to connect to.
        /// Should be of the form "{hostname|IP address}:port".
        /// </summary>
        public string DebuggerAddress
        {
            get { return debuggerAddress; }
            set { debuggerAddress = value; }
        }

        /// <summary>
        /// Gets or sets the directory in which to store minidump files.
        /// </summary>
        public string MinidumpPath
        {
            get { return minidumpPath; }
            set { minidumpPath = value; }
        }

        /// <summary>
        /// Gets or sets the performance logging preferences for the driver.
        /// </summary>
        public ChromePerformanceLoggingPreferences PerformanceLoggingPreferences
        {
            get { return perfLoggingPreferences; }
            set { perfLoggingPreferences = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChromeDriver"/> instance
        /// should use the legacy OSS protocol dialect or a dialect compliant with the W3C
        /// WebDriver Specification.
        /// </summary>
        public bool UseSpecCompliantProtocol
        {
            get { return useSpecCompliantProtocol; }
            set { useSpecCompliantProtocol = value; }
        }

        /// <summary>
        /// Adds a single argument to the list of arguments to be appended to the Chrome.exe command line.
        /// </summary>
        /// <param name="argument">The argument to add.</param>
        public void AddArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("argument must not be null or empty", "argument");
            }

            AddArguments(argument);
        }

        /// <summary>
        /// Adds arguments to be appended to the Chrome.exe command line.
        /// </summary>
        /// <param name="argumentsToAdd">An array of arguments to add.</param>
        public void AddArguments(params string[] argumentsToAdd)
        {
            AddArguments(new List<string>(argumentsToAdd));
        }

        /// <summary>
        /// Adds arguments to be appended to the Chrome.exe command line.
        /// </summary>
        /// <param name="argumentsToAdd">An <see cref="IEnumerable{T}"/> object of arguments to add.</param>
        public void AddArguments(IEnumerable<string> argumentsToAdd)
        {
            if (argumentsToAdd == null)
            {
                throw new ArgumentNullException("argumentsToAdd", "argumentsToAdd must not be null");
            }

            arguments.AddRange(argumentsToAdd);
        }

        /// <summary>
        /// Adds a single argument to be excluded from the list of arguments passed by default
        /// to the Chrome.exe command line by chromedriver.exe.
        /// </summary>
        /// <param name="argument">The argument to exclude.</param>
        public void AddExcludedArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("argument must not be null or empty", "argument");
            }

            AddExcludedArguments(argument);
        }

        /// <summary>
        /// Adds arguments to be excluded from the list of arguments passed by default
        /// to the Chrome.exe command line by chromedriver.exe.
        /// </summary>
        /// <param name="argumentsToExclude">An array of arguments to exclude.</param>
        public void AddExcludedArguments(params string[] argumentsToExclude)
        {
            AddExcludedArguments(new List<string>(argumentsToExclude));
        }

        /// <summary>
        /// Adds arguments to be excluded from the list of arguments passed by default
        /// to the Chrome.exe command line by chromedriver.exe.
        /// </summary>
        /// <param name="argumentsToExclude">An <see cref="IEnumerable{T}"/> object of arguments to exclude.</param>
        public void AddExcludedArguments(IEnumerable<string> argumentsToExclude)
        {
            if (argumentsToExclude == null)
            {
                throw new ArgumentNullException("argumentsToExclude", "argumentsToExclude must not be null");
            }

            excludedSwitches.AddRange(argumentsToExclude);
        }

        /// <summary>
        /// Adds a path to a packed Chrome extension (.crx file) to the list of extensions
        /// to be installed in the instance of Chrome.
        /// </summary>
        /// <param name="pathToExtension">The full path to the extension to add.</param>
        public void AddExtension(string pathToExtension)
        {
            if (string.IsNullOrEmpty(pathToExtension))
            {
                throw new ArgumentException("pathToExtension must not be null or empty", "pathToExtension");
            }

            AddExtensions(pathToExtension);
        }

        /// <summary>
        /// Adds a list of paths to packed Chrome extensions (.crx files) to be installed
        /// in the instance of Chrome.
        /// </summary>
        /// <param name="extensions">An array of full paths to the extensions to add.</param>
        public void AddExtensions(params string[] extensions)
        {
            AddExtensions(new List<string>(extensions));
        }

        /// <summary>
        /// Adds a list of paths to packed Chrome extensions (.crx files) to be installed
        /// in the instance of Chrome.
        /// </summary>
        /// <param name="extensions">An <see cref="IEnumerable{T}"/> of full paths to the extensions to add.</param>
        public void AddExtensions(IEnumerable<string> extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions", "extensions must not be null");
            }

            foreach (var extension in extensions)
            {
                if (!File.Exists(extension))
                {
                    throw new FileNotFoundException("No extension found at the specified path", extension);
                }

                extensionFiles.Add(extension);
            }
        }

        /// <summary>
        /// Adds a base64-encoded string representing a Chrome extension to the list of extensions
        /// to be installed in the instance of Chrome.
        /// </summary>
        /// <param name="extension">A base64-encoded string representing the extension to add.</param>
        public void AddEncodedExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("extension must not be null or empty", "extension");
            }

            AddEncodedExtensions(extension);
        }

        /// <summary>
        /// Adds a list of base64-encoded strings representing Chrome extensions to the list of extensions
        /// to be installed in the instance of Chrome.
        /// </summary>
        /// <param name="extensions">An array of base64-encoded strings representing the extensions to add.</param>
        public void AddEncodedExtensions(params string[] extensions)
        {
            AddEncodedExtensions(new List<string>(extensions));
        }

        /// <summary>
        /// Adds a list of base64-encoded strings representing Chrome extensions to be installed
        /// in the instance of Chrome.
        /// </summary>
        /// <param name="extensions">An <see cref="IEnumerable{T}"/> of base64-encoded strings
        /// representing the extensions to add.</param>
        public void AddEncodedExtensions(IEnumerable<string> extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions", "extensions must not be null");
            }

            foreach (var extension in extensions)
            {
                // Run the extension through the base64 converter to test that the
                // string is not malformed.
                try
                {
                    Convert.FromBase64String(extension);
                }
                catch (FormatException ex)
                {
                    throw new WebDriverException("Could not properly decode the base64 string", ex);
                }

                encodedExtensions.Add(extension);
            }
        }

        /// <summary>
        /// Adds a preference for the user-specific profile or "user data directory."
        /// If the specified preference already exists, it will be overwritten.
        /// </summary>
        /// <param name="preferenceName">The name of the preference to set.</param>
        /// <param name="preferenceValue">The value of the preference to set.</param>
        public void AddUserProfilePreference(string preferenceName, object preferenceValue)
        {
            if (userProfilePreferences == null)
            {
                userProfilePreferences = new Dictionary<string, object>();
            }

            userProfilePreferences[preferenceName] = preferenceValue;
        }

        /// <summary>
        /// Adds a preference for the local state file in the user's data directory for Chrome.
        /// If the specified preference already exists, it will be overwritten.
        /// </summary>
        /// <param name="preferenceName">The name of the preference to set.</param>
        /// <param name="preferenceValue">The value of the preference to set.</param>
        public void AddLocalStatePreference(string preferenceName, object preferenceValue)
        {
            if (localStatePreferences == null)
            {
                localStatePreferences = new Dictionary<string, object>();
            }

            localStatePreferences[preferenceName] = preferenceValue;
        }

        /// <summary>
        /// Allows the Chrome browser to emulate a mobile device.
        /// </summary>
        /// <param name="deviceName">The name of the device to emulate. The device name must be a
        /// valid device name from the Chrome DevTools Emulation panel.</param>
        /// <remarks>Specifying an invalid device name will not throw an exeption, but
        /// will generate an error in Chrome when the driver starts. To unset mobile
        /// emulation, call this method with <see langword="null"/> as the argument.</remarks>
        public void EnableMobileEmulation(string deviceName)
        {
            mobileEmulationDeviceSettings = null;
            mobileEmulationDeviceName = deviceName;
        }

        /// <summary>
        /// Allows the Chrome browser to emulate a mobile device.
        /// </summary>
        /// <param name="deviceSettings">The <see cref="ChromeMobileEmulationDeviceSettings"/>
        /// object containing the settings of the device to emulate.</param>
        /// <exception cref="ArgumentException">Thrown if the device settings option does
        /// not have a user agent string set.</exception>
        /// <remarks>Specifying an invalid device name will not throw an exeption, but
        /// will generate an error in Chrome when the driver starts. To unset mobile
        /// emulation, call this method with <see langword="null"/> as the argument.</remarks>
        public void EnableMobileEmulation(ChromeMobileEmulationDeviceSettings deviceSettings)
        {
            mobileEmulationDeviceName = null;
            if (deviceSettings != null && string.IsNullOrEmpty(deviceSettings.UserAgent))
            {
                throw new ArgumentException("Device settings must include a user agent string.", "deviceSettings");
            }

            mobileEmulationDeviceSettings = deviceSettings;
        }

        /// <summary>
        /// Adds a type of window that will be listed in the list of window handles
        /// returned by the Chrome driver.
        /// </summary>
        /// <param name="windowType">The name of the window type to add.</param>
        /// <remarks>This method can be used to allow the driver to access {webview}
        /// elements by adding "webview" as a window type.</remarks>
        public void AddWindowType(string windowType)
        {
            if (string.IsNullOrEmpty(windowType))
            {
                throw new ArgumentException("windowType must not be null or empty", "windowType");
            }

            AddWindowTypes(windowType);
        }

        /// <summary>
        /// Adds a list of window types that will be listed in the list of window handles
        /// returned by the Chrome driver.
        /// </summary>
        /// <param name="windowTypesToAdd">An array of window types to add.</param>
        public void AddWindowTypes(params string[] windowTypesToAdd)
        {
            AddWindowTypes(new List<string>(windowTypesToAdd));
        }

        /// <summary>
        /// Adds a list of window types that will be listed in the list of window handles
        /// returned by the Chrome driver.
        /// </summary>
        /// <param name="windowTypesToAdd">An <see cref="IEnumerable{T}"/> of window types to add.</param>
        public void AddWindowTypes(IEnumerable<string> windowTypesToAdd)
        {
            if (windowTypesToAdd == null)
            {
                throw new ArgumentNullException("windowTypesToAdd", "windowTypesToAdd must not be null");
            }

            windowTypes.AddRange(windowTypesToAdd);
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Chrome driver.
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
        /// chromedriver.exe.</remarks>
        public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
        {
            // Add the capability to the chromeOptions object by default. This is to handle
            // the 80% case where the chromedriver team adds a new option in chromedriver.exe
            // and the bindings have not yet had a type safe option added.
            AddAdditionalCapability(capabilityName, capabilityValue, false);
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Chrome driver.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to add.</param>
        /// <param name="capabilityValue">The value of the capability to add.</param>
        /// <param name="isGlobalCapability">Indicates whether the capability is to be set as a global
        /// capability for the driver instead of a Chrome-specific option.</param>
        /// <exception cref="ArgumentException">
        /// thrown when attempting to add a capability for which there is already a type safe option, or
        /// when <paramref name="capabilityName"/> is <see langword="null"/> or the empty string.
        /// </exception>
        /// <remarks>Calling <see cref="AddAdditionalCapability(string, object, bool)"/>
        /// where <paramref name="capabilityName"/> has already been added will overwrite the
        /// existing value with the new value in <paramref name="capabilityValue"/></remarks>
        public void AddAdditionalCapability(string capabilityName, object capabilityValue, bool isGlobalCapability)
        {
            if (IsKnownCapabilityName(capabilityName))
            {
                var typeSafeOptionName = GetTypeSafeOptionName(capabilityName);
                var message = string.Format(CultureInfo.InvariantCulture, "There is already an option for the {0} capability. Please use the {1} instead.", capabilityName, typeSafeOptionName);

                // TODO: Remove this if block when chromedriver bug 2371 is fixed
                // (https://bugs.chromium.org/p/chromedriver/issues/detail?id=2371)
                if (capabilityName == ForceAlwaysMatchCapabilityName)
                {
                    message = string.Format(CultureInfo.InvariantCulture, "The {0} capability is internal to the driver, and not intended to be set from users' code. Do not attempt to set this capability.", capabilityName);
                }

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
                additionalChromeOptions[capabilityName] = capabilityValue;
            }
        }

        /// <summary>
        /// Returns DesiredCapabilities for Chrome with these options included as
        /// capabilities. This does not copy the options. Further changes will be
        /// reflected in the returned capabilities.
        /// </summary>
        /// <returns>The DesiredCapabilities for Chrome with these options.</returns>
        public override ICapabilities ToCapabilities()
        {
            var chromeOptions = BuildChromeOptionsDictionary();

            var capabilities = GenerateDesiredCapabilities(false);
            capabilities.SetCapability(Capability, chromeOptions);

            var loggingPreferences = GenerateLoggingPreferencesDictionary();
            if (loggingPreferences != null)
            {
                capabilities.SetCapability(CapabilityType.LoggingPreferences, loggingPreferences);
            }

            foreach (var pair in additionalCapabilities)
            {
                capabilities.SetCapability(pair.Key, pair.Value);
            }

            return capabilities.AsReadOnly();
        }

        private Dictionary<string, object> BuildChromeOptionsDictionary()
        {
            var chromeOptions = new Dictionary<string, object>();
            if (Arguments.Count > 0)
            {
                chromeOptions[ArgumentsChromeOption] = Arguments;
            }

            if (!string.IsNullOrEmpty(binaryLocation))
            {
                chromeOptions[BinaryChromeOption] = binaryLocation;
            }

            var extensions = Extensions;
            if (extensions.Count > 0)
            {
                chromeOptions[ExtensionsChromeOption] = extensions;
            }

            if (localStatePreferences != null && localStatePreferences.Count > 0)
            {
                chromeOptions[LocalStateChromeOption] = localStatePreferences;
            }

            if (userProfilePreferences != null && userProfilePreferences.Count > 0)
            {
                chromeOptions[PreferencesChromeOption] = userProfilePreferences;
            }

            if (leaveBrowserRunning)
            {
                chromeOptions[DetachChromeOption] = leaveBrowserRunning;
            }

            if (useSpecCompliantProtocol)
            {
                chromeOptions[UseSpecCompliantProtocolOption] = useSpecCompliantProtocol;
            }

            if (!string.IsNullOrEmpty(debuggerAddress))
            {
                chromeOptions[DebuggerAddressChromeOption] = debuggerAddress;
            }

            if (excludedSwitches.Count > 0)
            {
                chromeOptions[ExcludeSwitchesChromeOption] = excludedSwitches;
            }

            if (!string.IsNullOrEmpty(minidumpPath))
            {
                chromeOptions[MinidumpPathChromeOption] = minidumpPath;
            }

            if (!string.IsNullOrEmpty(mobileEmulationDeviceName) || mobileEmulationDeviceSettings != null)
            {
                chromeOptions[MobileEmulationChromeOption] = GenerateMobileEmulationSettingsDictionary();
            }

            if (perfLoggingPreferences != null)
            {
                chromeOptions[PerformanceLoggingPreferencesChromeOption] = GeneratePerformanceLoggingPreferencesDictionary();
            }

            if (windowTypes.Count > 0)
            {
                chromeOptions[WindowTypesChromeOption] = windowTypes;
            }

            foreach (var pair in additionalChromeOptions)
            {
                chromeOptions.Add(pair.Key, pair.Value);
            }

            return chromeOptions;
        }

        private Dictionary<string, object> GeneratePerformanceLoggingPreferencesDictionary()
        {
            var perfLoggingPrefsDictionary = new Dictionary<string, object>();
            perfLoggingPrefsDictionary["enableNetwork"] = perfLoggingPreferences.IsCollectingNetworkEvents;
            perfLoggingPrefsDictionary["enablePage"] = perfLoggingPreferences.IsCollectingPageEvents;

            var tracingCategories = perfLoggingPreferences.TracingCategories;
            if (!string.IsNullOrEmpty(tracingCategories))
            {
                perfLoggingPrefsDictionary["traceCategories"] = tracingCategories;
            }

            perfLoggingPrefsDictionary["bufferUsageReportingInterval"] = Convert.ToInt64(perfLoggingPreferences.BufferUsageReportingInterval.TotalMilliseconds);

            return perfLoggingPrefsDictionary;
        }

        private Dictionary<string, object> GenerateMobileEmulationSettingsDictionary()
        {
            var mobileEmulationSettings = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(mobileEmulationDeviceName))
            {
                mobileEmulationSettings["deviceName"] = mobileEmulationDeviceName;
            }
            else if (mobileEmulationDeviceSettings != null)
            {
                mobileEmulationSettings["userAgent"] = mobileEmulationDeviceSettings.UserAgent;
                var deviceMetrics = new Dictionary<string, object>();
                deviceMetrics["width"] = mobileEmulationDeviceSettings.Width;
                deviceMetrics["height"] = mobileEmulationDeviceSettings.Height;
                deviceMetrics["pixelRatio"] = mobileEmulationDeviceSettings.PixelRatio;
                if (!mobileEmulationDeviceSettings.EnableTouchEvents)
                {
                    deviceMetrics["touch"] = mobileEmulationDeviceSettings.EnableTouchEvents;
                }

                mobileEmulationSettings["deviceMetrics"] = deviceMetrics;
            }

            return mobileEmulationSettings;
        }
    }
}
