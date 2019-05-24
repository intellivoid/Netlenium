using System.Globalization;

namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Represents the physical location of the browser.
    /// </summary>
    public class Location
    {
        private readonly double latitude;
        private readonly double longitude;
        private readonly double altitude;

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        /// <param name="latitude">latitude for current location</param>
        /// <param name="longitude">longitude for current location</param>
        /// <param name="altitude">altitude for current location</param>
        public Location(double latitude, double longitude, double altitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;
        }

        /// <summary>
        /// Gets the latitude of the current location.
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
        }

        /// <summary>
        /// Gets the longitude of the current location.
        /// </summary>
        public double Longitude
        {
            get { return longitude; }
        }

        /// <summary>
        /// Gets the altitude of the current location.
        /// </summary>
        public double Altitude
        {
            get { return altitude; }
        }

        /// <summary>
        /// Retuns string represenation for current location.
        /// </summary>
        /// <returns>Returns <see cref="string">string</see> reprsentation for current location.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Latitude: {0}, Longitude: {1}, Altitude: {2}", latitude, longitude, altitude);
        }
    }
}
