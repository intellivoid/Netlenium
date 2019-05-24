using System;
using System.Collections.Generic;
using System.Globalization;
using Netlenium.Driver.WebDriver.Internal;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Class to Create the capabilities of the browser you require for <see cref="IWebDriver"/>.
    /// If you wish to use default values use the static methods
    /// </summary>
    public class ReadOnlyDesiredCapabilities : ICapabilities, IHasCapabilitiesDictionary
    {
        private readonly Dictionary<string, object> capabilities = new Dictionary<string, object>();

        /// <summary>
        /// Prevents a default instance of the <see cref="ReadOnlyDesiredCapabilities"/> class.
        /// </summary>
        private ReadOnlyDesiredCapabilities()
        {
        }

        internal ReadOnlyDesiredCapabilities(DesiredCapabilities desiredCapabilities)
        {
            var internalDictionary = desiredCapabilities.CapabilitiesDictionary;
            foreach(var keyValuePair in internalDictionary)
            {
                capabilities[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        /// <summary>
        /// Gets the browser name
        /// </summary>
        public string BrowserName
        {
            get
            {
                var name = string.Empty;
                var capabilityValue = GetCapability(CapabilityType.BrowserName);
                if (capabilityValue != null)
                {
                    name = capabilityValue.ToString();
                }

                return name;
            }
        }

        /// <summary>
        /// Gets or sets the platform
        /// </summary>
        public Platform Platform
        {
            get
            {
                return GetCapability(CapabilityType.Platform) as Platform ?? new Platform(PlatformType.Any);
            }
        }

        /// <summary>
        /// Gets the browser version
        /// </summary>
        public string Version
        {
            get
            {
                var browserVersion = string.Empty;
                var capabilityValue = GetCapability(CapabilityType.Version);
                if (capabilityValue != null)
                {
                    browserVersion = capabilityValue.ToString();
                }

                return browserVersion;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the browser accepts SSL certificates.
        /// </summary>
        public bool AcceptInsecureCerts
        {
            get
            {
                var acceptSSLCerts = false;
                var capabilityValue = GetCapability(CapabilityType.AcceptInsecureCertificates);
                if (capabilityValue != null)
                {
                    acceptSSLCerts = (bool)capabilityValue;
                }

                return acceptSSLCerts;
            }
        }

        /// <summary>
        /// Gets the underlying Dictionary for a given set of capabilities.
        /// </summary>
        Dictionary<string, object> IHasCapabilitiesDictionary.CapabilitiesDictionary
        {
            get { return CapabilitiesDictionary; }
        }

        /// <summary>
        /// Gets the underlying Dictionary for a given set of capabilities.
        /// </summary>
        internal Dictionary<string, object> CapabilitiesDictionary
        {
            get { return capabilities; }
        }

        /// <summary>
        /// Gets the capability value with the specified name.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to get.</param>
        /// <returns>The value of the capability.</returns>
        /// <exception cref="ArgumentException">
        /// The specified capability name is not in the set of capabilities.
        /// </exception>
        public object this[string capabilityName]
        {
            get
            {
                if (!capabilities.ContainsKey(capabilityName))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The capability {0} is not present in this set of capabilities", capabilityName));
                }

                return capabilities[capabilityName];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the browser has a given capability.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <returns>Returns <see langword="true"/> if the browser has the capability; otherwise, <see langword="false"/>.</returns>
        public bool HasCapability(string capability)
        {
            return capabilities.ContainsKey(capability);
        }

        /// <summary>
        /// Gets a capability of the browser.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <returns>An object associated with the capability, or <see langword="null"/>
        /// if the capability is not set on the browser.</returns>
        public object GetCapability(string capability)
        {
            object capabilityValue = null;
            if (capabilities.ContainsKey(capability))
            {
                capabilityValue = capabilities[capability];
                var capabilityValueString = capabilityValue as string;
                if (capability == CapabilityType.Platform && capabilityValueString != null)
                {
                    capabilityValue = Platform.FromString(capabilityValue.ToString());
                }
            }

            return capabilityValue;
        }

        /// <summary>
        /// Converts the <see cref="ICapabilities"/> object to a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/> containing the capabilities.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            // CONSIDER: Instead of returning the raw internal member,
            // we might want to copy/clone it instead.
            return capabilities;
        }

        /// <summary>
        /// Return HashCode for the DesiredCapabilities that has been created
        /// </summary>
        /// <returns>Integer of HashCode generated</returns>
        public override int GetHashCode()
        {
            int result;
            result = BrowserName != null ? BrowserName.GetHashCode() : 0;
            result = (31 * result) + (Version != null ? Version.GetHashCode() : 0);
            result = (31 * result) + (Platform != null ? Platform.GetHashCode() : 0);
            return result;
        }

        /// <summary>
        /// Return a string of capabilities being used
        /// </summary>
        /// <returns>String of capabilities being used</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Capabilities [BrowserName={0}, Platform={1}, Version={2}]", BrowserName, Platform.PlatformType.ToString(), Version);
        }

        /// <summary>
        /// Compare two DesiredCapabilities and will return either true or false
        /// </summary>
        /// <param name="obj">DesiredCapabilities you wish to compare</param>
        /// <returns>true if they are the same or false if they are not</returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            var other = obj as DesiredCapabilities;
            if (other == null)
            {
                return false;
            }

            if (BrowserName != null ? BrowserName != other.BrowserName : other.BrowserName != null)
            {
                return false;
            }

            if (!Platform.IsPlatformType(other.Platform.PlatformType))
            {
                return false;
            }

            if (Version != null ? Version != other.Version : other.Version != null)
            {
                return false;
            }

            return true;
        }
    }
}
