using System;
using System.Collections.Generic;

namespace Netlenium.WebAPI.GitHub
{
    /// <summary>
    /// Release Object from GitHub
    /// </summary>
    [Serializable]
    public class Release
    {
        public int ID { get; set; }

        public Uri URL { get; set; }

        public Uri HtmlURL { get; set; }

        public Uri AssetsURL { get; set; }

        public Uri UploadURL { get; set; }

        public Uri TarballURL { get; set; }

        public Uri ZipballURL { get; set; }

        public string NodeID { get; set; }

        public string TagName { get; set; }

        public string TargetCommitish { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public bool Draft { get; set; }

        public bool PreRelease { get; set; }

        public string CreatedAt { get; set; }

        public string PublishedAt { get; set; }

        public User Author { get; set; }

        public List<Asset> Assets { get; set; }
    }
}
