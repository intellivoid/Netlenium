using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Describes the kind of proxy.
    /// </summary>
    /// <remarks>
    /// Keep these in sync with the Firefox preferences numbers:
    /// http://kb.mozillazine.org/Network.proxy.type
    /// </remarks>
    public enum ProxyKind
    {
        /// <summary>
        ///  Direct connection, no proxy (default on Windows).
        /// </summary>
        Direct = 0,

        /// <summary>
        /// Manual proxy settings (e.g., for httpProxy).
        /// </summary>
        Manual,

        /// <summary>
        /// Proxy automatic configuration from URL.
        /// </summary>
        ProxyAutoConfigure,

        /// <summary>
        /// Use proxy automatic detection.
        /// </summary>
        AutoDetect = 4,

        /// <summary>
        /// Use the system values for proxy settings (default on Linux).
        /// </summary>
        System,

        /// <summary>
        /// No proxy type is specified.
        /// </summary>
        Unspecified
    }

    /// <summary>
    /// Describes proxy settings to be used with a driver instance.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Proxy
    {
        private ProxyKind proxyKind = ProxyKind.Unspecified;
        private bool isAutoDetect;
        private string ftpProxyLocation;
        private string httpProxyLocation;
        private string proxyAutoConfigUrl;
        private string sslProxyLocation;
        private string socksProxyLocation;
        private string socksUserName;
        private string socksPassword;
        private List<string> noProxyAddresses = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Proxy"/> class.
        /// </summary>
        public Proxy()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Proxy"/> class with the given proxy settings.
        /// </summary>
        /// <param name="settings">A dictionary of settings to use with the proxy.</param>
        public Proxy(Dictionary<string, object> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings", "settings dictionary cannot be null");
            }

            if (settings.ContainsKey("proxyType"))
            {
                var rawType = (ProxyKind)Enum.Parse(typeof(ProxyKind), settings["proxyType"].ToString(), true);
                Kind = rawType;
            }

            if (settings.ContainsKey("ftpProxy"))
            {
                FtpProxy = settings["ftpProxy"].ToString();
            }

            if (settings.ContainsKey("httpProxy"))
            {
                HttpProxy = settings["httpProxy"].ToString();
            }

            if (settings.ContainsKey("noProxy"))
            {
                var bypassAddresses = new List<string>();
                var addressesAsString = settings["noProxy"] as string;
                if (addressesAsString != null)
                {
                    bypassAddresses.AddRange(addressesAsString.Split(';'));
                }
                else
                {
                    var addressesAsArray = settings["noProxy"] as object[];
                    if (addressesAsArray != null)
                    {
                        foreach (var address in addressesAsArray)
                        {
                            bypassAddresses.Add(address.ToString());
                        }
                    }
                }

                AddBypassAddresses(bypassAddresses);
            }

            if (settings.ContainsKey("proxyAutoconfigUrl"))
            {
                ProxyAutoConfigUrl = settings["proxyAutoconfigUrl"].ToString();
            }

            if (settings.ContainsKey("sslProxy"))
            {
                SslProxy = settings["sslProxy"].ToString();
            }

            if (settings.ContainsKey("socksProxy"))
            {
                SocksProxy = settings["socksProxy"].ToString();
            }

            if (settings.ContainsKey("socksUsername"))
            {
                SocksUserName = settings["socksUsername"].ToString();
            }

            if (settings.ContainsKey("socksPassword"))
            {
                SocksPassword = settings["socksPassword"].ToString();
            }

            if (settings.ContainsKey("autodetect"))
            {
                IsAutoDetect = (bool)settings["autodetect"];
            }
        }

        /// <summary>
        /// Gets or sets the type of proxy.
        /// </summary>
        [JsonIgnore]
        public ProxyKind Kind
        {
            get
            {
                return proxyKind;
            }

            set
            {
                VerifyProxyTypeCompatilibily(value);
                proxyKind = value;
            }
        }

        /// <summary>
        /// Gets the type of proxy as a string for JSON serialization.
        /// </summary>
        [JsonProperty("proxyType")]
        public string SerializableProxyKind
        {
            get
            {
                if (proxyKind == ProxyKind.ProxyAutoConfigure)
                {
                    return "PAC";
                }

                return proxyKind.ToString().ToUpperInvariant();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the proxy uses automatic detection.
        /// </summary>
        [JsonIgnore]
        public bool IsAutoDetect
        {
            get
            {
                return isAutoDetect;
            }

            set
            {
                if (isAutoDetect == value)
                {
                    return;
                }

                VerifyProxyTypeCompatilibily(ProxyKind.AutoDetect);
                proxyKind = ProxyKind.AutoDetect;
                isAutoDetect = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the FTP protocol.
        /// </summary>
        [JsonProperty("ftpProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string FtpProxy
        {
            get
            {
                return ftpProxyLocation;
            }

            set
            {
                VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                proxyKind = ProxyKind.Manual;
                ftpProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the HTTP protocol.
        /// </summary>
        [JsonProperty("httpProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string HttpProxy
        {
            get
            {
                return httpProxyLocation;
            }

            set
            {
                VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                proxyKind = ProxyKind.Manual;
                httpProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value for bypass proxy addresses.
        /// </summary>
        [Obsolete("Add addresses to bypass with the proxy by using the AddBypassAddress method.")]
        public string NoProxy
        {
            get
            {
                return BypassProxyAddresses;
            }

            set
            {
                AddBypassAddress(value);
            }
        }

        /// <summary>
        /// Gets the semicolon delimited list of address for which to bypass the proxy.
        /// </summary>
        [JsonProperty("noProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string BypassProxyAddresses
        {
            get
            {
                if (noProxyAddresses.Count == 0)
                {
                    return null;
                }

                return string.Join(";", noProxyAddresses.ToArray());
            }
        }

        /// <summary>
        /// Gets or sets the URL used for proxy automatic configuration.
        /// </summary>
        [JsonProperty("proxyAutoconfigUrl", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyAutoConfigUrl
        {
            get
            {
                return proxyAutoConfigUrl;
            }

            set
            {
                VerifyProxyTypeCompatilibily(ProxyKind.ProxyAutoConfigure);
                proxyKind = ProxyKind.ProxyAutoConfigure;
                proxyAutoConfigUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the SSL protocol.
        /// </summary>
        [JsonProperty("sslProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SslProxy
        {
            get
            {
                return sslProxyLocation;
            }

            set
            {
                VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                proxyKind = ProxyKind.Manual;
                sslProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the SOCKS protocol.
        /// </summary>
        [JsonProperty("socksProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SocksProxy
        {
            get
            {
                return socksProxyLocation;
            }

            set
            {
                VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                proxyKind = ProxyKind.Manual;
                socksProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of username for the SOCKS proxy.
        /// </summary>
        [JsonProperty("socksUsername", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SocksUserName
        {
            get
            {
                return socksUserName;
            }

            set
            {
                VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                proxyKind = ProxyKind.Manual;
                socksUserName = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of password for the SOCKS proxy.
        /// </summary>
        [JsonProperty("socksPassword", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SocksPassword
        {
            get
            {
                return socksPassword;
            }

            set
            {
                VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                proxyKind = ProxyKind.Manual;
                socksPassword = value;
            }
        }

        /// <summary>
        /// Adds a single address to the list of addresses against which the proxy will not be used.
        /// </summary>
        /// <param name="address">The address to add.</param>
        public void AddBypassAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("address must not be null or empty", "address");
            }

            AddBypassAddresses(address);
        }

        /// <summary>
        /// Adds addresses to the list of addresses against which the proxy will not be used.
        /// </summary>
        /// <param name="addressesToAdd">An array of addresses to add.</param>
        public void AddBypassAddresses(params string[] addressesToAdd)
        {
            AddBypassAddresses(new List<string>(addressesToAdd));
        }

        /// <summary>
        /// Adds addresses to the list of addresses against which the proxy will not be used.
        /// </summary>
        /// <param name="addressesToAdd">An <see cref="IEnumerable{T}"/> object of arguments to add.</param>
        public void AddBypassAddresses(IEnumerable<string> addressesToAdd)
        {
            if (addressesToAdd == null)
            {
                throw new ArgumentNullException("addressesToAdd", "addressesToAdd must not be null");
            }

            VerifyProxyTypeCompatilibily(ProxyKind.Manual);
            proxyKind = ProxyKind.Manual;
            noProxyAddresses.AddRange(addressesToAdd);
        }

        /// <summary>
        /// Returns a dictionary suitable for serializing to the W3C Specification
        /// dialect of the wire protocol.
        /// </summary>
        /// <returns>A dictionary suitable for serializing to the W3C Specification
        /// dialect of the wire protocol.</returns>
        internal Dictionary<string, object> ToCapability()
        {
            return AsDictionary(true);
        }

        /// <summary>
        /// Returns a dictionary suitable for serializing to the OSS dialect of the
        /// wire protocol.
        /// </summary>
        /// <returns>A dictionary suitable for serializing to the OSS dialect of the
        /// wire protocol.</returns>
        internal Dictionary<string, object> ToLegacyCapability()
        {
            return AsDictionary(false);
        }

        private Dictionary<string, object> AsDictionary(bool isSpecCompliant)
        {
            Dictionary<string, object> serializedDictionary = null;
            if (proxyKind != ProxyKind.Unspecified)
            {
                serializedDictionary = new Dictionary<string, object>();
                if (proxyKind == ProxyKind.ProxyAutoConfigure)
                {
                    serializedDictionary["proxyType"] = "pac";
                    if (!string.IsNullOrEmpty(proxyAutoConfigUrl))
                    {
                        serializedDictionary["proxyAutoconfigUrl"] = proxyAutoConfigUrl;
                    }
                }
                else
                {
                    serializedDictionary["proxyType"] = proxyKind.ToString().ToLowerInvariant();
                }

                if (!string.IsNullOrEmpty(httpProxyLocation))
                {
                    serializedDictionary["httpProxy"] = httpProxyLocation;
                }

                if (!string.IsNullOrEmpty(sslProxyLocation))
                {
                    serializedDictionary["sslProxy"] = sslProxyLocation;
                }

                if (!string.IsNullOrEmpty(ftpProxyLocation))
                {
                    serializedDictionary["ftpProxy"] = ftpProxyLocation;
                }

                if (!string.IsNullOrEmpty(socksProxyLocation))
                {
                    var socksAuth = string.Empty;
                    if (!string.IsNullOrEmpty(socksUserName) && !string.IsNullOrEmpty(socksPassword))
                    {
                        // TODO: this is probably inaccurate as to how this is supposed
                        // to look.
                        socksAuth = socksUserName + ":" + socksPassword + "@";
                    }

                    serializedDictionary["socksProxy"] = socksAuth + socksProxyLocation;
                }

                if (noProxyAddresses.Count > 0)
                {
                    serializedDictionary["noProxy"] = GetNoProxyAddressList(isSpecCompliant);
                }
            }

            return serializedDictionary;
        }

        private object GetNoProxyAddressList(bool isSpecCompliant)
        {
            object addresses = null;
            if (isSpecCompliant)
            {
                var addressList = new List<object>();
                foreach (var address in noProxyAddresses)
                {
                    addressList.Add(address);
                }

                addresses = addressList;
            }
            else
            {
                addresses = BypassProxyAddresses;
            }

            return addresses;
        }

        private void VerifyProxyTypeCompatilibily(ProxyKind compatibleProxy)
        {
            if (proxyKind != ProxyKind.Unspecified && proxyKind != compatibleProxy)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Specified proxy type {0} is not compatible with current setting {1}",
                    compatibleProxy.ToString().ToUpperInvariant(),
                    proxyKind.ToString().ToUpperInvariant());

                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}
