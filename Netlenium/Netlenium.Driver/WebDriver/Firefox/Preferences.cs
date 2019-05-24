using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Represents the preferences used by a profile in Firefox.
    /// </summary>
    internal class Preferences
    {
        private Dictionary<string, string> preferences = new Dictionary<string, string>();
        private Dictionary<string, string> immutablePreferences = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Preferences"/> class.
        /// </summary>
        /// <param name="defaultImmutablePreferences">A set of preferences that cannot be modified once set.</param>
        /// <param name="defaultPreferences">A set of default preferences.</param>
        public Preferences(Dictionary<string, object> defaultImmutablePreferences, Dictionary<string, object> defaultPreferences)
        {
            if (defaultImmutablePreferences != null)
            {
                foreach (var pref in defaultImmutablePreferences)
                {
                    SetPreferenceValue(pref.Key, pref.Value);
                    immutablePreferences.Add(pref.Key, pref.Value.ToString());
                }
            }

            if (defaultPreferences != null)
            {
                foreach (var pref in defaultPreferences)
                {
                    SetPreferenceValue(pref.Key, pref.Value);
                }
            }
        }

        /// <summary>
        /// Sets a preference.
        /// </summary>
        /// <param name="key">The name of the preference to set.</param>
        /// <param name="value">A <see cref="string"/> value give the preference.</param>
        /// <remarks>If the preference already exists in the currently-set list of preferences,
        /// the value will be updated.</remarks>
        internal void SetPreference(string key, string value)
        {
            SetPreferenceValue(key, value);
        }

        /// <summary>
        /// Sets a preference.
        /// </summary>
        /// <param name="key">The name of the preference to set.</param>
        /// <param name="value">A <see cref="int"/> value give the preference.</param>
        /// <remarks>If the preference already exists in the currently-set list of preferences,
        /// the value will be updated.</remarks>
        internal void SetPreference(string key, int value)
        {
            SetPreferenceValue(key, value);
        }

        /// <summary>
        /// Sets a preference.
        /// </summary>
        /// <param name="key">The name of the preference to set.</param>
        /// <param name="value">A <see cref="bool"/> value give the preference.</param>
        /// <remarks>If the preference already exists in the currently-set list of preferences,
        /// the value will be updated.</remarks>
        internal void SetPreference(string key, bool value)
        {
            SetPreferenceValue(key, value);
        }

        /// <summary>
        /// Gets a preference from the list of preferences.
        /// </summary>
        /// <param name="preferenceName">The name of the preference to retrieve.</param>
        /// <returns>The value of the preference, or an empty string if the preference is not set.</returns>
        internal string GetPreference(string preferenceName)
        {
            if (preferences.ContainsKey(preferenceName))
            {
                return preferences[preferenceName];
            }

            return string.Empty;
        }

        /// <summary>
        /// Appends this set of preferences to the specified set of preferences.
        /// </summary>
        /// <param name="preferencesToAdd">A dictionary containing the preferences to which to
        /// append these values.</param>
        /// <remarks>If the preference already exists in <paramref name="preferencesToAdd"/>,
        /// the value will be updated.</remarks>
        internal void AppendPreferences(Dictionary<string, string> preferencesToAdd)
        {
            // This allows the user to add additional preferences, or update ones that already
            // exist.
            foreach (var preferenceToAdd in preferencesToAdd)
            {
                if (IsSettablePreference(preferenceToAdd.Key))
                {
                    preferences[preferenceToAdd.Key] = preferenceToAdd.Value;
                }
            }
        }

        /// <summary>
        /// Writes the preferences to a file.
        /// </summary>
        /// <param name="filePath">The full path to the file to be written.</param>
        internal void WriteToFile(string filePath)
        {
            using (TextWriter writer = File.CreateText(filePath))
            {
                foreach (var preference in preferences)
                {
                    var escapedValue = preference.Value.Replace(@"\", @"\\");
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "user_pref(\"{0}\", {1});", preference.Key, escapedValue));
                }
            }
        }

        private static bool IsWrappedAsString(string value)
        {
            // Assume we a string is stringified (i.e. wrapped in " ") when
            // the first character == " and the last character == "
            return value.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && value.EndsWith("\"", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSettablePreference(string preferenceName)
        {
            return !immutablePreferences.ContainsKey(preferenceName);
        }

        private void SetPreferenceValue(string key, object value)
        {
            if (!IsSettablePreference(key))
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Preference {0} may not be overridden: frozen value={1}, requested value={2}", key, immutablePreferences[key], value.ToString());
                throw new ArgumentException(message);
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                if (IsWrappedAsString(stringValue))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Preference values must be plain strings: {0}: {1}", key, value));
                }

                preferences[key] = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", value);
                return;
            }

            if (value is bool)
            {
                preferences[key] = Convert.ToBoolean(value, CultureInfo.InvariantCulture).ToString().ToLowerInvariant();
                return;
            }

            if (value is int || value is long)
            {
                preferences[key] = Convert.ToInt32(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                return;
            }

            throw new WebDriverException("Value must be string, int or boolean");
        }
    }
}
