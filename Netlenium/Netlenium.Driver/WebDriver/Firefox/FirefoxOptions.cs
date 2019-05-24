using System;
using System.Collections.Generic;
using System.Globalization;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Class to manage options specific to <see cref="FirefoxDriver"/>
    /// </summary>
    /// <remarks>
    /// Used with the marionette executable wires.exe.
    /// </remarks>
    /// <example>
    /// <code>
    /// FirefoxOptions options = new FirefoxOptions();
    /// </code>
    /// <para></para>
    /// <para>For use with FirefoxDriver:</para>
    /// <para></para>
    /// <code>
    /// FirefoxDriver driver = new FirefoxDriver(options);
    /// </code>
    /// <para></para>
    /// <para>For use with RemoteWebDriver:</para>
    /// <para></para>
    /// <code>
    /// RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
    /// </code>
    /// </example>
    public class FirefoxOptions : DriverOptions
    {
        private const string BrowserNameValue = "firefox";

        private const string IsMarionetteCapability = "marionette";
        private const string FirefoxLegacyProfileCapability = "firefox_profile";
        private const string FirefoxLegacyBinaryCapability = "firefox_binary";
        private const string FirefoxProfileCapability = "profile";
        private const string FirefoxBinaryCapability = "binary";
        private const string FirefoxArgumentsCapability = "args";
        private const string FirefoxLogCapability = "log";
        private const string FirefoxPrefsCapability = "prefs";
        private const string FirefoxOptionsCapability = "moz:firefoxOptions";

        private bool isMarionette = true;
        private string browserBinaryLocation;
        private FirefoxDriverLogLevel logLevel = FirefoxDriverLogLevel.Default;
        private FirefoxProfile profile;
        private List<string> firefoxArguments = new List<string>();
        private Dictionary<string, object> profilePreferences = new Dictionary<string, object>();
        private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
        private Dictionary<string, object> additionalFirefoxOptions = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxOptions"/> class.
        /// </summary>
        public FirefoxOptions()
            : base()
        {
            BrowserName = BrowserNameValue;
            AddKnownCapabilityName(FirefoxOptionsCapability, "current FirefoxOptions class instance");
            AddKnownCapabilityName(IsMarionetteCapability, "UseLegacyImplementation property");
            AddKnownCapabilityName(FirefoxProfileCapability, "Profile property");
            AddKnownCapabilityName(FirefoxBinaryCapability, "BrowserExecutableLocation property");
            AddKnownCapabilityName(FirefoxArgumentsCapability, "AddArguments method");
            AddKnownCapabilityName(FirefoxPrefsCapability, "SetPreference method");
            AddKnownCapabilityName(FirefoxLogCapability, "LogLevel property");
            AddKnownCapabilityName(FirefoxLegacyProfileCapability, "Profile property");
            AddKnownCapabilityName(FirefoxLegacyBinaryCapability, "BrowserExecutableLocation property");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxOptions"/> class for the given profile and binary.
        /// </summary>
        /// <param name="profile">The <see cref="FirefoxProfile"/> to use in the options.</param>
        /// <param name="binary">The <see cref="FirefoxBinary"/> to use in the options.</param>
        /// <param name="capabilities">The <see cref="DesiredCapabilities"/> to copy into the options.</param>
        internal FirefoxOptions(FirefoxProfile profile, FirefoxBinary binary, DesiredCapabilities capabilities)
        {
            BrowserName = BrowserNameValue;
            if (profile != null)
            {
                this.profile = profile;
            }

            if (binary != null)
            {
                browserBinaryLocation = binary.BinaryExecutable.ExecutablePath;
            }

            if (capabilities != null)
            {
                ImportCapabilities(capabilities);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the legacy driver implementation.
        /// </summary>
        public bool UseLegacyImplementation
        {
            get { return !isMarionette; }
            set { isMarionette = !value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="FirefoxProfile"/> object to be used with this instance.
        /// </summary>
        public FirefoxProfile Profile
        {
            get { return profile; }
            set { profile = value; }
        }

        /// <summary>
        /// Gets or sets the path and file name of the Firefox browser executable.
        /// </summary>
        public string BrowserExecutableLocation
        {
            get { return browserBinaryLocation; }
            set { browserBinaryLocation = value; }
        }

        /// <summary>
        /// Gets or sets the logging level of the Firefox driver.
        /// </summary>
        public FirefoxDriverLogLevel LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }

        /// <summary>
        /// Adds an argument to be used in launching the Firefox browser.
        /// </summary>
        /// <param name="argumentName">The argument to add.</param>
        /// <remarks>Arguments must be preceeded by two dashes ("--").</remarks>
        public void AddArgument(string argumentName)
        {
            if (string.IsNullOrEmpty(argumentName))
            {
                throw new ArgumentException("argumentName must not be null or empty", "argumentName");
            }

            AddArguments(argumentName);
        }

        /// <summary>
        /// Adds a list arguments to be used in launching the Firefox browser.
        /// </summary>
        /// <param name="argumentsToAdd">An array of arguments to add.</param>
        /// <remarks>Each argument must be preceeded by two dashes ("--").</remarks>
        public void AddArguments(params string[] argumentsToAdd)
        {
            AddArguments(new List<string>(argumentsToAdd));
        }

        /// <summary>
        /// Adds a list arguments to be used in launching the Firefox browser.
        /// </summary>
        /// <param name="argumentsToAdd">An array of arguments to add.</param>
        /// <remarks>Each argument must be preceeded by two dashes ("--").</remarks>
        public void AddArguments(IEnumerable<string> argumentsToAdd)
        {
            if (argumentsToAdd == null)
            {
                throw new ArgumentNullException("argumentsToAdd", "argumentsToAdd must not be null");
            }

            firefoxArguments.AddRange(argumentsToAdd);
        }

        /// <summary>
        /// Sets a preference in the profile used by Firefox.
        /// </summary>
        /// <param name="preferenceName">Name of the preference to set.</param>
        /// <param name="preferenceValue">Value of the preference to set.</param>
        public void SetPreference(string preferenceName, bool preferenceValue)
        {
            SetPreferenceValue(preferenceName, preferenceValue);
        }

        /// <summary>
        /// Sets a preference in the profile used by Firefox.
        /// </summary>
        /// <param name="preferenceName">Name of the preference to set.</param>
        /// <param name="preferenceValue">Value of the preference to set.</param>
        public void SetPreference(string preferenceName, int preferenceValue)
        {
            SetPreferenceValue(preferenceName, preferenceValue);
        }

        /// <summary>
        /// Sets a preference in the profile used by Firefox.
        /// </summary>
        /// <param name="preferenceName">Name of the preference to set.</param>
        /// <param name="preferenceValue">Value of the preference to set.</param>
        public void SetPreference(string preferenceName, long preferenceValue)
        {
            SetPreferenceValue(preferenceName, preferenceValue);
        }

        /// <summary>
        /// Sets a preference in the profile used by Firefox.
        /// </summary>
        /// <param name="preferenceName">Name of the preference to set.</param>
        /// <param name="preferenceValue">Value of the preference to set.</param>
        public void SetPreference(string preferenceName, double preferenceValue)
        {
            SetPreferenceValue(preferenceName, preferenceValue);
        }

        /// <summary>
        /// Sets a preference in the profile used by Firefox.
        /// </summary>
        /// <param name="preferenceName">Name of the preference to set.</param>
        /// <param name="preferenceValue">Value of the preference to set.</param>
        public void SetPreference(string preferenceName, string preferenceValue)
        {
            SetPreferenceValue(preferenceName, preferenceValue);
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Firefox driver.
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
        /// geckodriver.exe.</remarks>
        public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
        {
            // Add the capability to the FirefoxOptions object by default. This is to handle
            // the 80% case where the geckodriver team adds a new option in geckodriver.exe
            // and the bindings have not yet had a type safe option added.
            AddAdditionalCapability(capabilityName, capabilityValue, false);
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the Firefox driver.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to add.</param>
        /// <param name="capabilityValue">The value of the capability to add.</param>
        /// <param name="isGlobalCapability">Indicates whether the capability is to be set as a global
        /// capability for the driver instead of a Firefox-specific option.</param>
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
                additionalFirefoxOptions[capabilityName] = capabilityValue;
            }
        }

        /// <summary>
        /// Returns DesiredCapabilities for Firefox with these options included as
        /// capabilities. This does not copy the options. Further changes will be
        /// reflected in the returned capabilities.
        /// </summary>
        /// <returns>The DesiredCapabilities for Firefox with these options.</returns>
        public override ICapabilities ToCapabilities()
        {
            var capabilities = GenerateDesiredCapabilities(isMarionette);
            if (isMarionette)
            {
                var firefoxOptions = GenerateFirefoxOptionsDictionary();
                capabilities.SetCapability(FirefoxOptionsCapability, firefoxOptions);
            }
            else
            {
                if (profile != null)
                {
                    if (Proxy != null)
                    {
                        profile.InternalSetProxyPreferences(Proxy);
                    }

                    capabilities.SetCapability(FirefoxProfileCapability, profile.ToBase64String());
                }

                if (!string.IsNullOrEmpty(browserBinaryLocation))
                {
                    capabilities.SetCapability(FirefoxBinaryCapability, browserBinaryLocation);
                }
                else
                {
                    using (var executablePathBinary = new FirefoxBinary())
                    {
                        var executablePath = executablePathBinary.BinaryExecutable.ExecutablePath;
                        capabilities.SetCapability(FirefoxBinaryCapability, executablePath);
                    }
                }
            }

            foreach (var pair in additionalCapabilities)
            {
                capabilities.SetCapability(pair.Key, pair.Value);
            }

            // Should return capabilities.AsReadOnly(), and will in a future release.
            return capabilities;
        }

        private Dictionary<string, object> GenerateFirefoxOptionsDictionary()
        {
            var firefoxOptions = new Dictionary<string, object>();

            if (profile != null)
            {
                // Using Marionette/Geckodriver, so the legacy WebDriver extension
                // is not required.
                profile.RemoveWebDriverExtension();
                firefoxOptions[FirefoxProfileCapability] = profile.ToBase64String();
            }

            if (!string.IsNullOrEmpty(browserBinaryLocation))
            {
                firefoxOptions[FirefoxBinaryCapability] = browserBinaryLocation;
            }
            else
            {
                if (!isMarionette)
                {
                    using (var executablePathBinary = new FirefoxBinary())
                    {
                        var executablePath = executablePathBinary.BinaryExecutable.ExecutablePath;
                        if (!string.IsNullOrEmpty(executablePath))
                        {
                            firefoxOptions[FirefoxBinaryCapability] = executablePath;
                        }
                    }
                }
            }

            if (logLevel != FirefoxDriverLogLevel.Default)
            {
                var logObject = new Dictionary<string, object>();
                logObject["level"] = logLevel.ToString().ToLowerInvariant();
                firefoxOptions[FirefoxLogCapability] = logObject;
            }

            if (firefoxArguments.Count > 0)
            {
                var args = new List<object>();
                foreach (var argument in firefoxArguments)
                {
                    args.Add(argument);
                }

                firefoxOptions[FirefoxArgumentsCapability] = args;
            }

            if (profilePreferences.Count > 0)
            {
                firefoxOptions[FirefoxPrefsCapability] = profilePreferences;
            }

            foreach (var pair in additionalFirefoxOptions)
            {
                firefoxOptions.Add(pair.Key, pair.Value);
            }

            return firefoxOptions;
        }

        private void SetPreferenceValue(string preferenceName, object preferenceValue)
        {
            if (string.IsNullOrEmpty(preferenceName))
            {
                throw new ArgumentException("Preference name may not be null an empty string.", "preferenceName");
            }

            if (!isMarionette)
            {
                throw new ArgumentException("Preferences cannot be set directly when using the legacy FirefoxDriver implementation. Set them in the profile.");
            }

            profilePreferences[preferenceName] = preferenceValue;
        }

        private void ImportCapabilities(DesiredCapabilities capabilities)
        {
            foreach (var pair in capabilities.CapabilitiesDictionary)
            {
                if (pair.Key == CapabilityType.BrowserName)
                {
                }
                else if (pair.Key == CapabilityType.BrowserVersion)
                {
                    BrowserVersion = pair.Value.ToString();
                }
                else if (pair.Key == CapabilityType.PlatformName)
                {
                    PlatformName = pair.Value.ToString();
                }
                else if (pair.Key == CapabilityType.Proxy)
                {
                    Proxy = new Proxy(pair.Value as Dictionary<string, object>);
                }
                else if (pair.Key == CapabilityType.UnhandledPromptBehavior)
                {
                    UnhandledPromptBehavior = (UnhandledPromptBehavior)Enum.Parse(typeof(UnhandledPromptBehavior), pair.Value.ToString(), true);
                }
                else if (pair.Key == CapabilityType.PageLoadStrategy)
                {
                    PageLoadStrategy = (PageLoadStrategy)Enum.Parse(typeof(PageLoadStrategy), pair.Value.ToString(), true);
                }
                else if (pair.Key == FirefoxOptionsCapability)
                {
                    var mozFirefoxOptions = pair.Value as Dictionary<string, object>;
                    foreach (var option in mozFirefoxOptions)
                    {
                        if (option.Key == FirefoxArgumentsCapability)
                        {
                            var args = option.Value as object[];
                            for (var i = 0; i < args.Length; i++)
                            {
                                firefoxArguments.Add(args[i].ToString());
                            }
                        }
                        else if (option.Key == FirefoxPrefsCapability)
                        {
                            profilePreferences = option.Value as Dictionary<string, object>;
                        }
                        else if (option.Key == FirefoxLogCapability)
                        {
                            var logDictionary = option.Value as Dictionary<string, object>;
                            if (logDictionary.ContainsKey("level"))
                            {
                                logLevel = (FirefoxDriverLogLevel)Enum.Parse(typeof(FirefoxDriverLogLevel), logDictionary["level"].ToString(), true);
                            }
                        }
                        else if (option.Key == FirefoxBinaryCapability)
                        {
                            browserBinaryLocation = option.Value.ToString();
                        }
                        else if (option.Key == FirefoxProfileCapability)
                        {
                            profile = FirefoxProfile.FromBase64String(option.Value.ToString());
                        }
                        else
                        {
                            AddAdditionalCapability(option.Key, option.Value);
                        }
                    }
                }
                else
                {
                    AddAdditionalCapability(pair.Key, pair.Value, true);
                }
            }
        }
    }
}
