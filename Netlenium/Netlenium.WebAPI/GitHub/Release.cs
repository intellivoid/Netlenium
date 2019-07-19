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
        public int Id { get; set; }

        public Uri Url { get; set; }

        public Uri HtmlUrl { get; set; }

        public Uri AssetsUrl { get; set; }

        public Uri UploadUrl { get; set; }

        public Uri TarballUrl { get; set; }

        public Uri ZipballUrl { get; set; }

        public string NodeId { get; set; }

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
