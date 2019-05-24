using System;
using System.Collections.Generic;
using System.Globalization;
using Netlenium.Driver.WebDriver.Html5;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Defines the interface through which the user can manipulate browser location.
    /// </summary>
    public class RemoteLocationContext : ILocationContext
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteLocationContext"/> class.
        /// </summary>
        /// <param name="driver">The <see cref="RemoteWebDriver"/> for which the application cache will be managed.</param>
        public RemoteLocationContext(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Gets or sets a value indicating the physical location of the browser.
        /// </summary>
        public Location PhysicalLocation
        {
            get
            {
                var commandResponse = driver.InternalExecute(DriverCommand.GetLocation, null);
                var location = commandResponse.Value as Dictionary<string, object>;
                if (location != null)
                {
                    return new Location(double.Parse(location["latitude"].ToString(), CultureInfo.InvariantCulture), double.Parse(location["longitude"].ToString(), CultureInfo.InvariantCulture), double.Parse(location["altitude"].ToString(), CultureInfo.InvariantCulture));
                }

                return null;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "value cannot be null");
                }

                var loc = new Dictionary<string, object>();
                loc.Add("latitude", value.Latitude);
                loc.Add("longitude", value.Longitude);
                loc.Add("altitude", value.Altitude);

                var parameters = new Dictionary<string, object>();
                parameters.Add("location", loc);
                driver.InternalExecute(DriverCommand.SetLocation, parameters);
            }
        }
    }
}
