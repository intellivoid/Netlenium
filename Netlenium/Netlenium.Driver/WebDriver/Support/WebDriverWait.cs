using System;

namespace Netlenium.Driver.WebDriver.Support.UI
{
    /// <summary>
    /// Provides the ability to wait for an arbitrary condition during test execution.
    /// </summary>
    /// <example>
    /// <code>
    /// IWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3))
    /// IWebElement element = wait.Until(driver => driver.FindElement(By.Name("q")));
    /// </code>
    /// </example>
    public class WebDriverWait : DefaultWait<IWebDriver>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverWait"/> class.
        /// </summary>
        /// <param name="driver">The WebDriver instance used to wait.</param>
        /// <param name="timeout">The timeout value indicating how long to wait for the condition.</param>
        public WebDriverWait(IWebDriver driver, TimeSpan timeout)
            : this(new SystemClock(), driver, timeout, DefaultSleepTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverWait"/> class.
        /// </summary>
        /// <param name="clock">An object implementing the <see cref="IClock"/> interface used to determine when time has passed.</param>
        /// <param name="driver">The WebDriver instance used to wait.</param>
        /// <param name="timeout">The timeout value indicating how long to wait for the condition.</param>
        /// <param name="sleepInterval">A <see cref="TimeSpan"/> value indicating how often to check for the condition to be true.</param>
        public WebDriverWait(IClock clock, IWebDriver driver, TimeSpan timeout, TimeSpan sleepInterval)
            : base(driver, clock)
        {
            Timeout = timeout;
            PollingInterval = sleepInterval;
            IgnoreExceptionTypes(typeof(NotFoundException));
        }

        private static TimeSpan DefaultSleepTimeout
        {
            get { return TimeSpan.FromMilliseconds(500); }
        }
    }
}