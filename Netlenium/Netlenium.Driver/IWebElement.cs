﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace Netlenium.Driver
{
    /// <summary>
    /// Web Element returned from the DOM
    /// </summary>
    public interface IWebElement
    {
        /// <summary>
        /// Get the Element's innerText
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the Element's size
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Get the Element's current location
        /// </summary>
        Point Location { get; }
        
        /// <summary>
        /// Determines if the Element is currently selected or not
        /// </summary>
        bool IsSelected { get; }

        /// <summary>
        /// Get's the Element's Tag Name
        /// </summary>
        string TagName { get; }

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
        /// Simulates a MouseClick event on the element
        /// </summary>
        void Click();

        /// <summary>
        /// Moves current view to this element
        /// </summary>
        void MoveTo();
    }
}
