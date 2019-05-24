using Netlenium.Driver.WebDriver.Internal;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Allows the user to control elements on a page in Firefox.
    /// </summary>
    public class FirefoxWebElement : RemoteWebElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxWebElement"/> class.
        /// </summary>
        /// <param name="parentDriver">The <see cref="FirefoxDriver"/> instance hosting this element.</param>
        /// <param name="id">The ID assigned to the element.</param>
        public FirefoxWebElement(FirefoxDriver parentDriver, string id)
            : base(parentDriver, id)
        {
        }

        /// <summary>
        /// Determines whether two <see cref="FirefoxWebElement"/> instances are equal.
        /// </summary>
        /// <param name="obj">The <see cref="FirefoxWebElement"/> to compare with the current <see cref="FirefoxWebElement"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="FirefoxWebElement"/> is equal to the
        /// current <see cref="FirefoxWebElement"/>; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as IWebElement;

            if (other == null)
            {
                return false;
            }

            if (other is IWrapsElement)
            {
                other = ((IWrapsElement)obj).WrappedElement;
            }

            var otherAsElement = other as FirefoxWebElement;
            if (otherAsElement == null)
            {
                return false;
            }

            return Id == otherAsElement.Id;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="FirefoxWebElement"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="FirefoxWebElement"/>.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
