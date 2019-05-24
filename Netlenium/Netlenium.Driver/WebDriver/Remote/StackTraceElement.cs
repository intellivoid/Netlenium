using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Gives properties to get a stack trace
    /// </summary>
    public class StackTraceElement
    {
        private string fileName = string.Empty;
        private string className = string.Empty;
        private int lineNumber;
        private string methodName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackTraceElement"/> class.
        /// </summary>
        public StackTraceElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackTraceElement"/> class using the given property values.
        /// </summary>
        /// <param name="elementAttributes">A <see cref="Dictionary{K, V}"/> containing the names and values for the properties of this <see cref="StackTraceElement"/>.</param>
        public StackTraceElement(Dictionary<string, object> elementAttributes)
        {
            if (elementAttributes != null)
            {
                if (elementAttributes.ContainsKey("className") && elementAttributes["className"] != null)
                {
                    className = elementAttributes["className"].ToString();
                }

                if (elementAttributes.ContainsKey("methodName") && elementAttributes["methodName"] != null)
                {
                    methodName = elementAttributes["methodName"].ToString();
                }

                if (elementAttributes.ContainsKey("lineNumber"))
                {
                    var line = 0;
                    if (int.TryParse(elementAttributes["lineNumber"].ToString(), out line))
                    {
                        lineNumber = line;
                    }
                }

                if (elementAttributes.ContainsKey("fileName") && elementAttributes["fileName"] != null)
                {
                    fileName = elementAttributes["fileName"].ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the filename in the stack
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Gets or sets the value of the Class name in the stack trace
        /// </summary>
        [JsonProperty("className")]
        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        /// <summary>
        /// Gets or sets the line number
        /// </summary>
        [JsonProperty("lineNumber")]
        public int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }

        /// <summary>
        /// Gets or sets the Method name in the stack trace
        /// </summary>
        [JsonProperty("methodName")]
        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        /// <summary>
        /// Gets a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "at {0}.{1} ({2}, {3})", className, methodName, fileName, lineNumber);
        }
    }
}
