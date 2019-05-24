using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Netlenium.Driver.WebDriver.Html5;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Defines the interface through which the user can manipulate session storage.
    /// </summary>
    public class RemoteSessionStorage : ISessionStorage
    {
        private RemoteWebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSessionStorage"/> class.
        /// </summary>
        /// <param name="driver">The driver instance.</param>
        public RemoteSessionStorage(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Gets the number of items in session storage.
        /// </summary>
        public int Count
        {
            get
            {
                var commandResponse = driver.InternalExecute(DriverCommand.GetSessionStorageSize, null);
                return Convert.ToInt32(commandResponse.Value, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Returns session storage value given a key.
        /// </summary>
        /// <param name="key">The key of the item in storage.</param>
        /// <returns>A session storage <see cref="string"/> value given a key, if present, otherwise return null.</returns>
        public string GetItem(string key)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("key", key);
            var commandResponse = driver.InternalExecute(DriverCommand.GetSessionStorageItem, parameters);
            if (commandResponse.Value == null)
            {
                return null;
            }

            return commandResponse.Value.ToString();
        }

        /// <summary>
        /// Returns a read-only list of session storage keys.
        /// </summary>
        /// <returns>A <see cref="ReadOnlyCollection{T}">read-only list</see> of session storage keys.</returns>
        public ReadOnlyCollection<string> KeySet()
        {
            var result = new List<string>();
            var commandResponse = driver.InternalExecute(DriverCommand.GetSessionStorageKeys, null);
            var keys = commandResponse.Value as object[];
            foreach (string key in keys)
            {
                result.Add(key);
            }

            return result.AsReadOnly();
        }

        /// <summary>
        /// Sets session storage entry using given key/value pair.
        /// </summary>
        /// <param name="key">Session storage key</param>
        /// <param name="value">Session storage value</param>
        public void SetItem(string key, string value)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("key", key);
            parameters.Add("value", value);
            driver.InternalExecute(DriverCommand.SetSessionStorageItem, parameters);
        }

        /// <summary>
        /// Removes session storage entry for the given key.
        /// </summary>
        /// <param name="key">key to be removed from the list</param>
        /// <returns>Response value <see cref="string"/>for the given key.</returns>
        public string RemoveItem(string key)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("key", key);
            var commandResponse = driver.InternalExecute(DriverCommand.RemoveSessionStorageItem, parameters);
            if (commandResponse.Value == null)
            {
                return null;
            }

            return commandResponse.Value.ToString();
        }

        /// <summary>
        /// Removes all entries from the session storage.
        /// </summary>
        public void Clear()
        {
            driver.InternalExecute(DriverCommand.ClearSessionStorage, null);
        }
    }
}
