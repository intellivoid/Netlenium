using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Base class for managing options specific to a browser driver.
    /// </summary>
    public class RemoteSessionSettings : ICapabilities
    {
        private const string FirstMatchCapabilityName = "firstMatch";
        private const string AlwaysMatchCapabilityName = "alwaysMatch";

        private readonly List<string> reservedSettingNames = new List<string>() { FirstMatchCapabilityName, AlwaysMatchCapabilityName };
        private DriverOptions mustMatchDriverOptions;
        private List<DriverOptions> firstMatchOptions = new List<DriverOptions>();
        private Dictionary<string, object> remoteMetadataSettings = new Dictionary<string, object>();

        /// <summary>
        /// Creates a new instance of the <see cref="RemoteSessionSettings"/> class.
        /// </summary>
        public RemoteSessionSettings()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RemoteSessionSettings"/> class,
        /// containing the specified <see cref="DriverOptions"/> to use in the remote
        /// session.
        /// </summary>
        /// <param name="mustMatchDriverOptions">
        /// A <see cref="DriverOptions"/> object that contains values that must be matched
        /// by the remote end to create the remote session.
        /// </param>
        /// <param name="firstMatchDriverOptions">
        /// A list of <see cref="DriverOptions"/> objects that contain values that may be matched
        /// by the remote end to create the remote session.
        /// </param>
        public RemoteSessionSettings(DriverOptions mustMatchDriverOptions, params DriverOptions[] firstMatchDriverOptions)
        {
            this.mustMatchDriverOptions = mustMatchDriverOptions;
            foreach (var firstMatchOption in firstMatchDriverOptions)
            {
                AddFirstMatchDriverOption(firstMatchOption);
            }
        }

        /// <summary>
        /// Gets a value indicating the options that must be matched by the remote end to create a session.
        /// </summary>
        internal DriverOptions MustMatchDriverOptions
        {
            get { return mustMatchDriverOptions; }
        }

        /// <summary>
        /// Gets a value indicating the number of options that may be matched by the remote end to create a session.
        /// </summary>
        internal int FirstMatchOptionsCount
        {
            get { return firstMatchOptions.Count; }
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
                if (capabilityName == AlwaysMatchCapabilityName)
                {
                    return GetAlwaysMatchOptionsAsSerializableDictionary();
                }

                if (capabilityName == FirstMatchCapabilityName)
                {
                    return GetFirstMatchOptionsAsSerializableList();
                }

                if (!remoteMetadataSettings.ContainsKey(capabilityName))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The capability {0} is not present in this set of capabilities", capabilityName));
                }

                return remoteMetadataSettings[capabilityName];
            }

        }

        /// <summary>
        /// Add a metadata setting to this set of remote session settings.
        /// </summary>
        /// <param name="settingName">The name of the setting to set.</param>
        /// <param name="settingValue">The value of the setting.</param>
        /// <remarks>
        /// The value to be set must be serializable to JSON for transmission
        /// across the wire to the remote end. To be JSON-serializable, the value
        /// must be a string, a numeric value, a boolean value, an object that
        /// implmeents <see cref="IEnumerable"/> that contains JSON-serializable
        /// objects, or a <see cref="Dictionary{TKey, TValue}"/> where the keys
        /// are strings and the values are JSON-serializable.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown if the setting name is null, the empty string, or one of the
        /// reserved names of metadata settings; or if the setting value is not
        /// JSON serializable.
        /// </exception>
        public void AddMetadataSetting(string settingName, object settingValue)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentException("Metadata setting name cannot be null or empty", "settingName");
            }

            if (reservedSettingNames.Contains(settingName))
            {
                throw new ArgumentException(string.Format("'{0}' is a reserved name for a metadata setting, and cannot be used as a name.", settingName), "settingName");
            }

            if (!IsJsonSerializable(settingValue))
            {
                throw new ArgumentException("Metadata setting value must be JSON serializable.", "settingValue");
            }

            remoteMetadataSettings[settingName] = settingValue;
        }

        /// <summary>
        /// Adds a <see cref="DriverOptions"/> object to the list of options containing values to be
        /// "first matched" by the remote end.
        /// </summary>
        /// <param name="options">The <see cref="DriverOptions"/> to add to the list of "first matched" options.</param>
        public void AddFirstMatchDriverOption(DriverOptions options)
        {
            if (mustMatchDriverOptions != null)
            {
                var mergeResult = mustMatchDriverOptions.GetMergeResult(options);
                if (mergeResult.IsMergeConflict)
                {
                    var msg = string.Format(CultureInfo.InvariantCulture, "You cannot request the same capability in both must-match and first-match capabilities. You are attempting to add a first-match driver options object that defines a capability, '{0}', that is already defined in the must-match driver options.", mergeResult.MergeConflictOptionName);
                    throw new ArgumentException(msg, "options");
                }
            }

            firstMatchOptions.Add(options);
        }

        /// <summary>
        /// Adds a <see cref="DriverOptions"/> object containing values that must be matched
        /// by the remote end to successfully create a session.
        /// </summary>
        /// <param name="options">The <see cref="DriverOptions"/> that must be matched by
        /// the remote end to successfully create a session.</param>
        public void SetMustMatchDriverOptions(DriverOptions options)
        {
            if (firstMatchOptions.Count > 0)
            {
                var driverOptionIndex = 0;
                foreach (var firstMatchOption in firstMatchOptions)
                {
                    var mergeResult = firstMatchOption.GetMergeResult(options);
                    if (mergeResult.IsMergeConflict)
                    {
                        var msg = string.Format(CultureInfo.InvariantCulture, "You cannot request the same capability in both must-match and first-match capabilities. You are attempting to add a must-match driver options object that defines a capability, '{0}', that is already defined in the first-match driver options with index {1}.", mergeResult.MergeConflictOptionName, driverOptionIndex);
                        throw new ArgumentException(msg, "options");
                    }

                    driverOptionIndex++;
                }
            }

            mustMatchDriverOptions = options;
        }

        /// <summary>
        /// Gets a value indicating whether the browser has a given capability.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <returns>Returns <see langword="true"/> if this set of capabilities has the capability;
        /// otherwise, <see langword="false"/>.</returns>
        public bool HasCapability(string capability)
        {
            if (capability == AlwaysMatchCapabilityName || capability == FirstMatchCapabilityName)
            {
                return true;
            }

            return remoteMetadataSettings.ContainsKey(capability);
        }

        /// <summary>
        /// Gets a capability of the browser.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <returns>An object associated with the capability, or <see langword="null"/>
        /// if the capability is not set in this set of capabilities.</returns>
        public object GetCapability(string capability)
        {
            if (capability == AlwaysMatchCapabilityName)
            {
                return GetAlwaysMatchOptionsAsSerializableDictionary();
            }

            if (capability == FirstMatchCapabilityName)
            {
                return GetFirstMatchOptionsAsSerializableList();
            }

            if (remoteMetadataSettings.ContainsKey(capability))
            {
                return remoteMetadataSettings[capability];
            }

            return null;
        }

        /// <summary>
        /// Return a dictionary representation of this <see cref="RemoteSessionSettings"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> representation of this <see cref="RemoteSessionSettings"/>.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            var capabilitiesDictionary = new Dictionary<string, object>();

            foreach (var remoteMetadataSetting in remoteMetadataSettings)
            {
                capabilitiesDictionary[remoteMetadataSetting.Key] = remoteMetadataSetting.Value;
            }

            if (mustMatchDriverOptions != null)
            {
                capabilitiesDictionary["alwaysMatch"] = GetAlwaysMatchOptionsAsSerializableDictionary();
            }

            if (firstMatchOptions.Count > 0)
            {
                var optionsMatches = GetFirstMatchOptionsAsSerializableList();

                capabilitiesDictionary["firstMatch"] = optionsMatches;
            }

            return capabilitiesDictionary;
        }

        /// <summary>
        /// Return a string representation of the remote session settings to be sent.
        /// </summary>
        /// <returns>String representation of the remote session settings to be sent.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToDictionary(), Formatting.Indented);
        }

        internal DriverOptions GetFirstMatchDriverOptions(int firstMatchIndex)
        {
            if (firstMatchIndex < 0 || firstMatchIndex >= firstMatchOptions.Count)
            {
                throw new ArgumentException("Requested index must be greater than zero and less than the count of firstMatch options added.");
            }

            return firstMatchOptions[firstMatchIndex];
        }

        private Dictionary<string, object> GetAlwaysMatchOptionsAsSerializableDictionary()
        {
            return mustMatchDriverOptions.ToDictionary();
        }

        private List<object> GetFirstMatchOptionsAsSerializableList()
        {
            var optionsMatches = new List<object>();
            foreach (var options in firstMatchOptions)
            {
                optionsMatches.Add(options.ToDictionary());
            }

            return optionsMatches;
        }

        private bool IsJsonSerializable(object arg)
        {
            var argAsEnumerable = arg as IEnumerable;
            var argAsDictionary = arg as IDictionary;

            if (arg is string || arg is float || arg is double || arg is int || arg is long || arg is bool || arg == null)
            {
                return true;
            }
            else if (argAsDictionary != null)
            {
                foreach (var key in argAsDictionary.Keys)
                {
                    if (!(key is string))
                    {
                        return false;
                    }
                }

                foreach (var value in argAsDictionary.Values)
                {
                    if (!IsJsonSerializable(value))
                    {
                        return false;
                    }
                }
            }
            else if (argAsEnumerable != null)
            {
                foreach (var item in argAsEnumerable)
                {
                    if (!IsJsonSerializable(item))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
