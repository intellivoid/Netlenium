using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver
{
    public interface IWebElement
    {
        /// <summary>
        /// Simulates typing text into the element
        /// </summary>
        /// <param name="input"></param>
        void SendKeys(string input);

        /// <summary>
        /// Sets the value of an attribute on the specified element. If the attribute already exists, the value
        /// is updated; otherwise a new attribute is added with the specified name and value.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        void SetAttribute(string attributeName, string value);

        /// <summary>
        /// Returns the value of a specified attribute on the element
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        string GetAttribute(string attributeName);

        /// <summary>
        /// Moves current view to this element
        /// </summary>
        void MoveTo();
    }
}
