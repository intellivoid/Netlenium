using System;
using System.Collections.Generic;
using System.IO;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Provides a way to access Firefox to run tests.
    /// </summary>
    /// <remarks>
    /// When the FirefoxDriver object has been instantiated the browser will load. The test can then navigate to the URL under test and
    /// start your test.
    /// <para>
    /// In the case of the FirefoxDriver, you can specify a named profile to be used, or you can let the
    /// driver create a temporary, anonymous profile. A custom extension allowing the driver to communicate
    /// to the browser will be installed into the profile.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [TestFixture]
    /// public class Testing
    /// {
    ///     private IWebDriver driver;
    ///     <para></para>
    ///     [SetUp]
    ///     public void SetUp()
    ///     {
    ///         driver = new FirefoxDriver();
    ///     }
    ///     <para></para>
    ///     [Test]
    ///     public void TestGoogle()
    ///     {
    ///         driver.Navigate().GoToUrl("http://www.google.co.uk");
    ///         /*
    ///         *   Rest of the test
    ///         */
    ///     }
    ///     <para></para>
    ///     [TearDown]
    ///     public void TearDown()
    ///     {
    ///         driver.Quit();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class FirefoxDriver : RemoteWebDriver
    {
        /// <summary>
        /// The name of the ICapabilities setting to use to define a custom Firefox profile.
        /// </summary>
        public static readonly string ProfileCapabilityName = "firefox_profile";

        /// <summary>
        /// The name of the ICapabilities setting to use to define a custom location for the
        /// Firefox executable.
        /// </summary>
        public static readonly string BinaryCapabilityName = "firefox_binary";

        /// <summary>
        /// The default port on which to communicate with the Firefox extension.
        /// </summary>
        public static readonly int DefaultPort = 7055;

        /// <summary>
        /// Indicates whether native events is enabled by default for this platform.
        /// </summary>
        public static readonly bool DefaultEnableNativeEvents = Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows);

        /// <summary>
        /// Indicates whether the driver will accept untrusted SSL certificates.
        /// </summary>
        public static readonly bool AcceptUntrustedCertificates = true;

        /// <summary>
        /// Indicates whether the driver assume the issuer of untrusted certificates is untrusted.
        /// </summary>
        public static readonly bool AssumeUntrustedCertificateIssuer = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class.
        /// </summary>
        public FirefoxDriver()
            : this(new FirefoxOptions(null, null, null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified options. Uses the Mozilla-provided Marionette driver implementation.
        /// </summary>
        /// <param name="options">The <see cref="FirefoxOptions"/> to be used with the Firefox driver.</param>
        public FirefoxDriver(FirefoxOptions options)
            : this(CreateService(options), options, DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified driver service. Uses the Mozilla-provided Marionette driver implementation.
        /// </summary>
        /// <param name="service">The <see cref="FirefoxDriverService"/> used to initialize the driver.</param>
        public FirefoxDriver(FirefoxDriverService service)
            : this(service, new FirefoxOptions(), DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified path
        /// to the directory containing geckodriver.exe.
        /// </summary>
        /// <param name="geckoDriverDirectory">The full path to the directory containing geckodriver.exe.</param>
        public FirefoxDriver(string geckoDriverDirectory)
            : this(geckoDriverDirectory, new FirefoxOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified path
        /// to the directory containing geckodriver.exe and options.
        /// </summary>
        /// <param name="geckoDriverDirectory">The full path to the directory containing geckodriver.exe.</param>
        /// <param name="options">The <see cref="FirefoxOptions"/> to be used with the Firefox driver.</param>
        public FirefoxDriver(string geckoDriverDirectory, FirefoxOptions options)
            : this(geckoDriverDirectory, options, DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified path
        /// to the directory containing geckodriver.exe, options, and command timeout.
        /// </summary>
        /// <param name="geckoDriverDirectory">The full path to the directory containing geckodriver.exe.</param>
        /// <param name="options">The <see cref="FirefoxOptions"/> to be used with the Firefox driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public FirefoxDriver(string geckoDriverDirectory, FirefoxOptions options, TimeSpan commandTimeout)
            : this(FirefoxDriverService.CreateDefaultService(geckoDriverDirectory), options, commandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified options, driver service, and timeout. Uses the Mozilla-provided Marionette driver implementation.
        /// </summary>
        /// <param name="service">The <see cref="FirefoxDriverService"/> to use.</param>
        /// <param name="options">The <see cref="FirefoxOptions"/> to be used with the Firefox driver.</param>
        public FirefoxDriver(FirefoxDriverService service, FirefoxOptions options)
            : this(service, options, DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified options, driver service, and timeout. Uses the Mozilla-provided Marionette driver implementation.
        /// </summary>
        /// <param name="service">The <see cref="FirefoxDriverService"/> to use.</param>
        /// <param name="options">The <see cref="FirefoxOptions"/> to be used with the Firefox driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public FirefoxDriver(FirefoxDriverService service, FirefoxOptions options, TimeSpan commandTimeout)
            : base(CreateExecutor(service, options, commandTimeout), ConvertOptionsToCapabilities(options))
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="IFileDetector"/> responsible for detecting
        /// sequences of keystrokes representing file paths and names.
        /// </summary>
        /// <remarks>The Firefox driver does not allow a file detector to be set,
        /// as the server component of the Firefox driver only allows uploads from
        /// the local computer environment. Attempting to set this property has no
        /// effect, but does not throw an exception. If you  are attempting to run
        /// the Firefox driver remotely, use <see cref="RemoteWebDriver"/> in
        /// conjunction with a standalone WebDriver server.</remarks>
        public override IFileDetector FileDetector
        {
            get { return base.FileDetector; }
            set { }
        }

        /// <summary>
        /// Gets a value indicating whether the Firefox driver instance uses
        /// Mozilla's Marionette implementation. This is a temporary property
        /// and will be removed when Marionette is available for the release
        /// channel of Firefox.
        /// </summary>
        public bool IsMarionette
        {
            get { return IsSpecificationCompliant; }
        }

        /// <summary>
        /// In derived classes, the <see cref="PrepareEnvironment"/> method prepares the environment for test execution.
        /// </summary>
        protected virtual void PrepareEnvironment()
        {
            // Does nothing, but provides a hook for subclasses to do "stuff"
        }

        private static ICommandExecutor CreateExecutor(FirefoxDriverService service, FirefoxOptions options, TimeSpan commandTimeout)
        {
            ICommandExecutor executor = null;
            if (options.UseLegacyImplementation)
            {
                // Note: If BrowserExecutableLocation is null or empty, the legacy driver
                // will still do the right thing, and find Firefox in the default location.
                var binary = new FirefoxBinary(options.BrowserExecutableLocation);

                var profile = options.Profile;
                if (profile == null)
                {
                    profile = new FirefoxProfile();
                }

                executor = CreateExtensionConnection(binary, profile, commandTimeout);
            }
            else
            {
                if (service == null)
                {
                    throw new ArgumentNullException("service", "You requested a service-based implementation, but passed in a null service object.");
                }

                return new DriverServiceCommandExecutor(service, commandTimeout);
            }

            return executor;
        }

        private static ICommandExecutor CreateExtensionConnection(FirefoxBinary binary, FirefoxProfile profile, TimeSpan commandTimeout)
        {
            var profileToUse = profile;

            var suggestedProfile = Environment.GetEnvironmentVariable("webdriver.firefox.profile");
            if (profileToUse == null && suggestedProfile != null)
            {
                profileToUse = new FirefoxProfileManager().GetProfile(suggestedProfile);
            }
            else if (profileToUse == null)
            {
                profileToUse = new FirefoxProfile();
            }

            var executor = new FirefoxDriverCommandExecutor(binary, profileToUse, "localhost", commandTimeout);
            return executor;
        }

        private static ICapabilities ConvertOptionsToCapabilities(FirefoxOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options", "options must not be null");
            }

            var capabilities = options.ToCapabilities();
            if (options.UseLegacyImplementation)
            {
                capabilities = RemoveUnneededCapabilities(capabilities);
            }

            return capabilities;
        }

        private static ICapabilities RemoveUnneededCapabilities(ICapabilities capabilities)
        {
            var caps = capabilities as DesiredCapabilities;
            caps.CapabilitiesDictionary.Remove(ProfileCapabilityName);
            caps.CapabilitiesDictionary.Remove(BinaryCapabilityName);
            return caps;
        }

        private static FirefoxOptions CreateOptionsFromCapabilities(ICapabilities capabilities)
        {
            // This is awkward and hacky. To be removed when the legacy driver is retired.
            var binary = ExtractBinary(capabilities);
            var profile = ExtractProfile(capabilities);
            var desiredCaps = RemoveUnneededCapabilities(capabilities) as DesiredCapabilities;

            var options = new FirefoxOptions(profile, binary, desiredCaps);
            return options;
        }

        private static FirefoxBinary ExtractBinary(ICapabilities capabilities)
        {
            if (capabilities.GetCapability(BinaryCapabilityName) != null)
            {
                var file = capabilities.GetCapability(BinaryCapabilityName).ToString();
                return new FirefoxBinary(file);
            }

            return new FirefoxBinary();
        }

        private static FirefoxProfile ExtractProfile(ICapabilities capabilities)
        {
            var profile = new FirefoxProfile();
            if (capabilities.GetCapability(ProfileCapabilityName) != null)
            {
                var raw = capabilities.GetCapability(ProfileCapabilityName);
                var rawAsProfile = raw as FirefoxProfile;
                var rawAsString = raw as string;
                if (rawAsProfile != null)
                {
                    profile = rawAsProfile;
                }
                else if (rawAsString != null)
                {
                    try
                    {
                        profile = FirefoxProfile.FromBase64String(rawAsString);
                    }
                    catch (IOException e)
                    {
                        throw new WebDriverException("Unable to create profile from specified string", e);
                    }
                }
            }

            if (capabilities.GetCapability(CapabilityType.Proxy) != null)
            {
                Proxy proxy = null;
                var raw = capabilities.GetCapability(CapabilityType.Proxy);
                var rawAsProxy = raw as Proxy;
                var rawAsMap = raw as Dictionary<string, object>;
                if (rawAsProxy != null)
                {
                    proxy = rawAsProxy;
                }
                else if (rawAsMap != null)
                {
                    proxy = new Proxy(rawAsMap);
                }

                profile.SetProxyPreferences(proxy);
            }

            if (capabilities.GetCapability(CapabilityType.AcceptSslCertificates) != null)
            {
                var acceptCerts = (bool)capabilities.GetCapability(CapabilityType.AcceptSslCertificates);
                profile.AcceptUntrustedCertificates = acceptCerts;
            }

            return profile;
        }

        private static FirefoxDriverService CreateService(FirefoxOptions options)
        {
            if (options != null && options.UseLegacyImplementation)
            {
                return null;
            }

            return FirefoxDriverService.CreateDefaultService();
        }
    }
}
