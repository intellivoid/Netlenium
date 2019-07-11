using Netlenium.Driver;

namespace NetleniumServer
{
    /// <summary>
    /// General Utilities Class
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Parses the SearchBy value, throws an exception if it's invalid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static SearchBy ParseSearchBy(string input)
        {
            switch (input.ToLower())
            {
                case "class_name":
                    return SearchBy.ClassName;

                case "css_selector":
                    return SearchBy.CssSelector;

                case "id":
                    return SearchBy.Id;

                case "name":
                    return SearchBy.Name;

                case "tag_name":
                    return SearchBy.TagName;

                case "xpath":
                    return SearchBy.XPath;

                default:
                    throw new InvalidSearchByValueException();
            }
        }
    }
}
