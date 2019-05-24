using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netlenium.Driver.WebDriver.Chrome
{
    /// <summary>
    /// Provides manipulation of getting and setting network conditions from Chrome.
    /// </summary>
    public class ChromeNetworkConditions
    {
        private bool offline;
        private TimeSpan latency = TimeSpan.Zero;
        private long downloadThroughput = -1;
        private long uploadThroughput = -1;

        /// <summary>
        /// Gets or sets a value indicating whether the network is offline. Defaults to <see langword="false"/>.
        /// </summary>
        public bool IsOffline
        {
            get { return offline; }
            set { offline = value; }
        }

        /// <summary>
        /// Gets or sets the simulated latency of the connection. Typically given in milliseconds.
        /// </summary>
        public TimeSpan Latency
        {
            get { return latency; }
            set { latency = value; }
        }

        /// <summary>
        /// Gets or sets the throughput of the network connection in kb/second for downloading.
        /// </summary>
        public long DownloadThroughput
        {
            get { return downloadThroughput; }
            set { downloadThroughput = value; }
        }

        /// <summary>
        /// Gets or sets the throughput of the network connection in kb/second for uploading.
        /// </summary>
        public long UploadThroughput
        {
            get { return uploadThroughput; }
            set { uploadThroughput = value; }
        }

        static internal ChromeNetworkConditions FromDictionary(Dictionary<string, object> dictionary)
        {
            var conditions = new ChromeNetworkConditions();
            if (dictionary.ContainsKey("offline"))
            {
                conditions.IsOffline = (bool)dictionary["offline"];
            }

            if (dictionary.ContainsKey("latency"))
            {
                conditions.Latency = TimeSpan.FromMilliseconds(Convert.ToDouble(dictionary["latency"]));
            }

            if (dictionary.ContainsKey("upload_throughput"))
            {
                conditions.UploadThroughput = (long)dictionary["upload_throughput"];
            }

            if (dictionary.ContainsKey("download_throughput"))
            {
                conditions.DownloadThroughput = (long)dictionary["download_throughput"];
            }

            return conditions;
        }

        internal Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary["offline"] = offline;
            if (latency != TimeSpan.Zero)
            {
                dictionary["latency"] = Convert.ToInt64(latency.TotalMilliseconds);
            }

            if (downloadThroughput >= 0)
            {
                dictionary["download_throughput"] = downloadThroughput;
            }

            if (uploadThroughput >= 0)
            {
                dictionary["upload_throughput"] = uploadThroughput;
            }

            return dictionary;
        }
    }
}
