using System;
using System.Collections.Generic;
using System.Globalization;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Defines the interface through which the user can define timeouts.
    /// </summary>
    internal class RemoteTimeouts : ITimeouts
    {
        private const string ImplicitTimeoutName = "implicit";
        private const string AsyncScriptTimeoutName = "script";
        private const string PageLoadTimeoutName = "pageLoad";
        private const string LegacyPageLoadTimeoutName = "page load";

        private readonly TimeSpan DefaultImplicitWaitTimeout = TimeSpan.FromSeconds(0);
        private readonly TimeSpan DefaultAsyncScriptTimeout = TimeSpan.FromSeconds(30);
        private readonly TimeSpan DefaultPageLoadTimeout = TimeSpan.FromSeconds(300);

        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTimeouts"/> class
        /// </summary>
        /// <param name="driver">The driver that is currently in use</param>
        public RemoteTimeouts(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Gets or sets the implicit wait timeout, which is the  amount of time the
        /// driver should wait when searching for an element if it is not immediately
        /// present.
        /// </summary>
        /// <remarks>
        /// When searching for a single element, the driver should poll the page
        /// until the element has been found, or this timeout expires before throwing
        /// a <see cref="NoSuchElementException"/>. When searching for multiple elements,
        /// the driver should poll the page until at least one element has been found
        /// or this timeout has expired.
        /// <para>
        /// Increasing the implicit wait timeout should be used judiciously as it
        /// will have an adverse effect on test run time, especially when used with
        /// slower location strategies like XPath.
        /// </para>
        /// </remarks>
        public TimeSpan ImplicitWait
        {
            get { return ExecuteGetTimeout(ImplicitTimeoutName); }
            set { ExecuteSetTimeout(ImplicitTimeoutName, value); }
        }

        /// <summary>
        /// Gets or sets the asynchronous script timeout, which is the amount
        /// of time the driver should wait when executing JavaScript asynchronously.
        /// This timeout only affects the <see cref="IJavaScriptExecutor.ExecuteAsyncScript(string, object[])"/>
        /// method.
        /// </summary>
        public TimeSpan AsynchronousJavaScript
        {
            get { return ExecuteGetTimeout(AsyncScriptTimeoutName); }
            set { ExecuteSetTimeout(AsyncScriptTimeoutName, value); }
        }

        /// <summary>
        /// Gets or sets the page load timeout, which is the amount of time the driver
        /// should wait for a page to load when setting the <see cref="IWebDriver.Url"/>
        /// property.
        /// </summary>
        public TimeSpan PageLoad
        {
            get
            {
                var timeoutName = LegacyPageLoadTimeoutName;
                if (driver.IsSpecificationCompliant)
                {
                    timeoutName = PageLoadTimeoutName;
                }

                return ExecuteGetTimeout(timeoutName);
            }

            set
            {
                var timeoutName = LegacyPageLoadTimeoutName;
                if (driver.IsSpecificationCompliant)
                {
                    timeoutName = PageLoadTimeoutName;
                }

                ExecuteSetTimeout(timeoutName, value);
            }
        }

        private TimeSpan ExecuteGetTimeout(string timeoutType)
        {
            if (driver.IsSpecificationCompliant)
            {
                var commandResponse = driver.InternalExecute(DriverCommand.GetTimeouts, null);
                var responseValue = (Dictionary<string, object>)commandResponse.Value;
                if (!responseValue.ContainsKey(timeoutType))
                {
                    throw new WebDriverException("Specified timeout type not defined");
                }

                return TimeSpan.FromMilliseconds(Convert.ToDouble(responseValue[timeoutType], CultureInfo.InvariantCulture));
            }
            else
            {
                throw new NotImplementedException("Driver instance must comply with the W3C specification to support getting timeout values.");
            }
        }

        private void ExecuteSetTimeout(string timeoutType, TimeSpan timeToWait)
        {
            var milliseconds = timeToWait.TotalMilliseconds;
            if (timeToWait == TimeSpan.MinValue)
            {
                if (driver.IsSpecificationCompliant)
                {
                    if (timeoutType == ImplicitTimeoutName)
                    {
                        milliseconds = DefaultImplicitWaitTimeout.TotalMilliseconds;
                    }
                    else if (timeoutType == AsyncScriptTimeoutName)
                    {
                        milliseconds = DefaultAsyncScriptTimeout.TotalMilliseconds;
                    }
                    else
                    {
                        milliseconds = DefaultPageLoadTimeout.TotalMilliseconds;
                    }
                }
                else
                {
                    milliseconds = -1;
                }
            }

            var parameters = new Dictionary<string, object>();
            if (driver.IsSpecificationCompliant)
            {
                parameters.Add(timeoutType, Convert.ToInt64(milliseconds));
            }
            else
            {
                parameters.Add("type", timeoutType);
                parameters.Add("ms", milliseconds);
            }

            driver.InternalExecute(DriverCommand.SetTimeouts, parameters);
        }
    }
}
