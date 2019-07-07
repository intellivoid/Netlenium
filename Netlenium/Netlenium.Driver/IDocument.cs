using System.Collections.Generic;

namespace Netlenium.Driver
{
    /// <summary>
    /// The current document that's being displayed in the browser
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Searches for elements in the document and returns a List of IWebElement objects
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        List<IWebElement> GetElements(SearchBy searchBy, string value);

        /// <summary>
        /// Searches for elements in the document and returns the first IWebElement that's found
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IWebElement GetElement(SearchBy searchBy, string value);

        /// <summary>
        /// Gets the current document's title
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the current URL that's currently loaded
        /// </summary>
        string Uri { get; }
        
    }
}
