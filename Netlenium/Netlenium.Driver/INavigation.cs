using System;

namespace Netlenium.Driver
{
    /// <summary>
    /// Navigation Methods for the Browser
    /// </summary>
    public interface INavigation
    {
        /// <summary>
        /// Load a URL into the document
        /// </summary>
        /// <param name="location"></param>
        void LoadURI(Uri location);

        /// <summary>
        /// Go back one page in the history.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Go forward one page in the history.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Reloads the document in the browser element on which you call this method.
        /// </summary>
        void Reload();
    }
}
