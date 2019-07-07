using System;

namespace Netlenium.Driver
{
    /// <summary>
    /// Actions to control the Browser
    /// </summary>
    public interface IActions
    {
        /// <summary>
        /// Load a URL into the document
        /// </summary>
        /// <param name="location"></param>
        void LoadURI(Uri location);

        /// <summary>
        /// Load a URL into the document
        /// </summary>
        /// <param name="location"></param>
        void LoadURI(string location);

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

        /// <summary>
        /// Executes Javascript code in the current document
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string ExecuteJavascript(string code);
    }
}
