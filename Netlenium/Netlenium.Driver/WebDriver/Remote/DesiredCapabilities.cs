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
    //[Obsolete("Use of DesiredCapabilities has been deprecated in favor of browser-specific Options classes")]
    public class DesiredCapabilities : ICapabilities, IHasCapabilitiesDictionary
    {
        private readonly Dictionary<string, object> capabilities = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredCapabilities"/> class
        /// </summary>
        /// <param name="browser">Name of the browser e.g. firefox, internet explorer, safari</param>
        /// <param name="version">Version of the browser</param>
        /// <param name="platform">The platform it works on</param>
        //[Obsolete("Use of DesiredCapabilities has been deprecated in favor of browser-specific Options classes")]
        public DesiredCapabilities(string browser, string version, Platform platform)
        {
            SetCapability(CapabilityType.BrowserName, browser);
            SetCapability(CapabilityType.Version, version);
            SetCapability(CapabilityType.Platform, platform);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredCapabilities"/> class
        /// </summary>
        //[Obsolete("Use of DesiredCapabilities has been deprecated in favor of browser-specific Options classes")]
        public DesiredCapabilities()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredCapabilities"/> class
        /// </summary>
        /// <param name="rawMap">Dictionary of items for the remote driver</param>
        /// <example>
        /// <code>
        /// DesiredCapabilities capabilities = new DesiredCapabilities(new Dictionary<![CDATA[<string,object>]]>(){["browserName","firefox"],["version",string.Empty],["javaScript",true]});
        /// </code>
        /// </example>
        //[Obsolete("Use of DesiredCapabilities has been deprecated in favor of browser-specific Options classes")]
        public DesiredCapabilities(Dictionary<string, object> rawMap)
        {
            if (rawMap != null)
            {
                foreach (var key in rawMap.Keys)
                {
                    if (key == CapabilityType.Platform)
                    {
                        var raw = rawMap[CapabilityType.Platform];
                        var rawAsString = raw as string;
                        var rawAsPlatform = raw as Platform;
                        if (rawAsString != null)
                        {
                            SetCapability(CapabilityType.Platform, Platform.FromString(rawAsString));
                        }
                        else if (rawAsPlatform != null)
                        {
                            SetCapability(CapabilityType.Platform, rawAsPlatform);
                        }
                    }
                    else
                    {
                        SetCapability(key, rawMap[key]);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredCapabilities"/> class
        /// </summary>
        /// <param name="browser">Name of the browser e.g. firefox, internet explorer, safari</param>
        /// <param name="version">Version of the browser</param>
        /// <param name="platform">The platform it works on</param>
        /// <param name="isSpecCompliant">Sets a value indicating whether the capabilities are
        /// compliant with the W3C WebDriver specification.</param>
        internal DesiredCapabilities(string browser, string version, Platform platform, bool isSpecCompliant)
        {
            SetCapability(CapabilityType.BrowserName, browser);
            SetCapability(CapabilityType.Version, version);
            SetCapability(CapabilityType.Platform, platform);
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
            get => GetCapability(CapabilityType.Platform) as Platform ?? new Platform(PlatformType.Any);

            set => SetCapability(CapabilityType.Platform, value);
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

            set
            {
                SetCapability(CapabilityType.AcceptInsecureCertificates, value);
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

            set
            {
                capabilities[capabilityName] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the browser has a given capability.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <returns>Returns <see langword="true"/> if the browser has the capability; otherwise, <see langword="false"/>.</returns>
        //[Obsolete("Use of DesiredCapabilities has been deprecated in favor of browser-specific Options classes")]
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
        //[Obsolete("Use of DesiredCapabilities has been deprecated in favor of browser-specific Options classes")]
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
        /// Sets a capability of the browser.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <param name="capabilityValue">The value for the capability.</param>
        //[Obsolete("Use of DesiredCapabilities has been deprecated in favor of browser-specific Options classes")]
        public void SetCapability(string capability, object capabilityValue)
        {
            // Handle the special case of Platform objects. These should
            // be stored in the underlying dictionary as their protocol
            // string representation.
            var platformCapabilityValue = capabilityValue as Platform;
            if (platformCapabilityValue != null)
            {
                capabilities[capability] = platformCapabilityValue.ProtocolPlatformType;
            }
            else
            {
                capabilities[capability] = capabilityValue;
            }
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

        /// <summary>
        /// Returns a read-only version of this capabilities object.
        /// </summary>
        /// <returns>A read-only version of this capabilities object.</returns>
        internal ReadOnlyDesiredCapabilities AsReadOnly()
        {
            var readOnlyCapabilities = new ReadOnlyDesiredCapabilities(this);
            return readOnlyCapabilities;
        }
    }
}
