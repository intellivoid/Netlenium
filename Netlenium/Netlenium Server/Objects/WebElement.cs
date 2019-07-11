using Netlenium.Driver;
using System;

namespace NetleniumServer.Objects
{
    /// <summary>
    /// WebElement Object Structure
    /// </summary>
    [Serializable]
    public class WebElement
    {
        
        public bool Enabled { get; private set; }

        public bool IsSelected { get; private set; }

        public LocationProperty ElementLocation { get; private set; }

        public SizeProperty ElementSize { get; private set; }

        public string TagName { get; private set; }

        public string InnerText { get; private set; }

        /// <summary>
        /// Public Consturctor
        /// </summary>
        /// <param name="webElement"></param>
        public WebElement(IWebElement webElement)
        {
            Enabled = webElement.Enabled;
            IsSelected = webElement.IsSelected;
            ElementLocation = new LocationProperty(webElement.Location);
            ElementSize = new SizeProperty(webElement.Size);
            TagName = webElement.TagName;
            InnerText = webElement.Text;
        }
    }
}
