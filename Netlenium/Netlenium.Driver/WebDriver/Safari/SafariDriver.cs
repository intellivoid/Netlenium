﻿using System;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Safari
{
    /// <summary>
    /// Provides a way to access Safari to run your tests by creating a SafariDriver instance
    /// </summary>
    /// <remarks>
    /// When the WebDriver object has been instantiated the browser will load. The test can then navigate to the URL under test and
    /// start your test.
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
    ///         driver = new SafariDriver();
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
    ///         driver.Dispose();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class SafariDriver : RemoteWebDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class.
        /// </summary>
        public SafariDriver()
            : this(new SafariOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class using the specified <see cref="SafariOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="SafariOptions"/> to use for this <see cref="SafariDriver"/> instance.</param>
        public SafariDriver(SafariOptions options)
            : this(SafariDriverService.CreateDefaultService(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class using the specified driver service.
        /// </summary>
        /// <param name="service">The <see cref="SafariDriverService"/> used to initialize the driver.</param>
        public SafariDriver(SafariDriverService service)
            : this(service, new SafariOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class using the specified path
        /// to the directory containing ChromeDriver.exe.
        /// </summary>
        /// <param name="safariDriverDirectory">The full path to the directory containing SafariDriver executable.</param>
        public SafariDriver(string safariDriverDirectory)
            : this(safariDriverDirectory, new SafariOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class using the specified path
        /// to the directory containing ChromeDriver.exe and options.
        /// </summary>
        /// <param name="safariDriverDirectory">The full path to the directory containing SafariDriver executable.</param>
        /// <param name="options">The <see cref="SafariOptions"/> to be used with the Safari driver.</param>
        public SafariDriver(string safariDriverDirectory, SafariOptions options)
            : this(safariDriverDirectory, options, DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class using the specified path
        /// to the directory containing ChromeDriver.exe, options, and command timeout.
        /// </summary>
        /// <param name="safariDriverDirectory">The full path to the directory containing SafariDriver executable.</param>
        /// <param name="options">The <see cref="SafariOptions"/> to be used with the Safari driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public SafariDriver(string safariDriverDirectory, SafariOptions options, TimeSpan commandTimeout)
            : this(SafariDriverService.CreateDefaultService(safariDriverDirectory), options, commandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class using the specified
        /// <see cref="SafariDriverService"/> and options.
        /// </summary>
        /// <param name="service">The <see cref="SafariDriverService"/> to use.</param>
        /// <param name="options">The <see cref="SafariOptions"/> used to initialize the driver.</param>
        public SafariDriver(SafariDriverService service, SafariOptions options)
            : this(service, options, DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafariDriver"/> class using the specified <see cref="SafariDriverService"/>.
        /// </summary>
        /// <param name="service">The <see cref="SafariDriverService"/> to use.</param>
        /// <param name="options">The <see cref="SafariOptions"/> to be used with the Safari driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public SafariDriver(SafariDriverService service, SafariOptions options, TimeSpan commandTimeout)
            : base(new DriverServiceCommandExecutor(service, commandTimeout), ConvertOptionsToCapabilities(options))
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="IFileDetector"/> responsible for detecting
        /// sequences of keystrokes representing file paths and names.
        /// </summary>
        /// <remarks>The Safari driver does not allow a file detector to be set,
        /// as the server component of the Safari driver (the Safari extension) only
        /// allows uploads from the local computer environment. Attempting to set
        /// this property has no effect, but does not throw an exception. If you
        /// are attempting to run the Safari driver remotely, use <see cref="RemoteWebDriver"/>
        /// in conjunction with a standalone WebDriver server.</remarks>
        public override IFileDetector FileDetector
        {
            get { return base.FileDetector; }
            set { }
        }

        private static ICapabilities ConvertOptionsToCapabilities(SafariOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options", "options must not be null");
            }

            return options.ToCapabilities();
        }
    }
}
