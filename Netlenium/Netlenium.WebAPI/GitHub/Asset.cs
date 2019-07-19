using System;

namespace Netlenium.WebAPI.GitHub
{
    [Serializable]
    public class Asset
    {
        public int Id { get; set; }

        public Uri Url { get; set; }

        public Uri BrowserDownloadUrl { get; set; }

        public string NodeId { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public string State { get; set; }

        public string ContentType { get; set; }

        public int Size { get; set; }

        public int DownloadCount { get; set; }

        public string CreatedAt { get; set; }

        public string UpdatedAt { get; set; }

        public User Uploader { get; set; }
    }
}
