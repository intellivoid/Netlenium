using System;

namespace Netlenium.WebAPI.GitHub
{
    [Serializable]
    public class Asset
    {
        public int ID { get; set; }

        public Uri URL { get; set; }

        public Uri BrowserDownloadURL { get; set; }

        public string NodeID { get; set; }

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
