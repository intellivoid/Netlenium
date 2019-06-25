using System;

namespace Netlenium.WebAPI.Google
{
    /// <summary>
    /// Storage Content Object
    /// </summary>
    public class Content
    {
        /// <summary>
        /// The name of the content
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Generation property
        /// </summary>
        public string Generation { get; set; }

        /// <summary>
        /// Meta Generation property
        /// </summary>
        public string MetaGeneration { get; set; }

        /// <summary>
        /// The date that this content was last modified
        /// </summary>
        public string LastModified { get; set; }

        /// <summary>
        /// Unique ETag Property
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// The size of the content
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// The Access Location of the content (URL)
        /// </summary>
        public Uri AccessLocation { get; set; }
    }
}
