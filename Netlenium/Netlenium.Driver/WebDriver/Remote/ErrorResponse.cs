using System.Collections.Generic;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a way to store errors from a response
    /// </summary>
    public class ErrorResponse
    {
        private StackTraceElement[] stackTrace;
        private string message = string.Empty;
        private string className = string.Empty;
        private string screenshot = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
        /// </summary>
        public ErrorResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse"/> class using the specified values.
        /// </summary>
        /// <param name="responseValue">A <see cref="Dictionary{K, V}"/> containing names and values of
        /// the properties of this <see cref="ErrorResponse"/>.</param>
        public ErrorResponse(Dictionary<string, object> responseValue)
        {
            if (responseValue != null)
            {
                if (responseValue.ContainsKey("message"))
                {
                    if (responseValue["message"] != null)
                    {
                        message = responseValue["message"].ToString();
                    }
                    else
                    {
                        message = "The error did not contain a message.";
                    }
                }

                if (responseValue.ContainsKey("screen") && responseValue["screen"] != null)
                {
                    screenshot = responseValue["screen"].ToString();
                }

                if (responseValue.ContainsKey("class") && responseValue["class"] != null)
                {
                    className = responseValue["class"].ToString();
                }

                if (responseValue.ContainsKey("stackTrace"))
                {
                    var stackTraceArray = responseValue["stackTrace"] as object[];
                    if (stackTraceArray != null)
                    {
                        var stackTraceList = new List<StackTraceElement>();
                        foreach (var rawStackTraceElement in stackTraceArray)
                        {
                            var elementAsDictionary = rawStackTraceElement as Dictionary<string, object>;
                            if (elementAsDictionary != null)
                            {
                                stackTraceList.Add(new StackTraceElement(elementAsDictionary));
                            }
                        }

                        stackTrace = stackTraceList.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the message from the response
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Gets or sets the class name that threw the error
        /// </summary>
        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        /// <summary>
        /// Gets or sets the screenshot of the error
        /// </summary>
        public string Screenshot
        {
            // TODO: (JimEvans) Change this to return an Image.
            get { return screenshot; }
            set { screenshot = value; }
        }

        /// <summary>
        /// Gets or sets the stack trace of the error
        /// </summary>
        public StackTraceElement[] StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }
    }
}
