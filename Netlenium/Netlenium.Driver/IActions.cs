using System;
using System.Collections.Generic;

namespace Netlenium.Driver
{
    /// <summary>
    /// Actions to control the Browser
    /// </summary>
    public interface IActions
    {
        /// <summary>
        /// Gets the current window handle
        /// </summary>
        IWindow CurrentWindow { get; }

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
        /// Returns a collection of Windows that are currently opened
        /// </summary>
        /// <returns></returns>
        List<IWindow> GetWindows();

        /// <summary>
        /// Gets a Window Handle from a ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        IWindow GetWindow(string Id);
        
        /// <summary>
        /// Executes Javascript code in the current document
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string ExecuteJavascript(string code);

        /// <summary>
        /// Closes the current window handle and switches back to an active handle
        /// </summary>
        void Close();
    }
}
