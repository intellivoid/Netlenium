using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Netlenium.Driver.WebDriver.Html5
{
    /// <summary>
    /// Represents the local storage for the site currently opened in the browser.
    /// Each site has its own separate storage area.
    /// </summary>
    public interface ILocalStorage
    {
        /// <summary>
        /// Gets the number of items in local storage.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns value of the local storage given a key.
        /// </summary>
        /// <param name="key">key to for a local storage entry</param>
        /// <returns>Value of the local storage entry as <see cref="string"/> given a key.</returns>
        string GetItem(string key);

        /// <summary>
        /// Returns the set of keys associated with local storage.
        /// </summary>
        /// <returns>Returns the set of keys associated with local storage as <see cref="HashSet{T}"/>.</returns>
        ReadOnlyCollection<string> KeySet();

        /// <summary>
        /// Adds key/value pair to local storage.
        /// </summary>
        /// <param name="key">storage key</param>
        /// <param name="value">storage value</param>
        void SetItem(string key, string value);

        /// <summary>
        /// Removes key/value pair from local storage.
        /// </summary>
        /// <param name="key">key to remove from storage</param>
        /// <returns>Value from local storage as <see cref="string">string</see> for the given key.</returns>
        string RemoveItem(string key);

        /// <summary>
        /// Clears local storage.
        /// </summary>
        void Clear();
    }
}
