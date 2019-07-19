using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Xml;

namespace Netlenium.WebAPI.Google
{
    public class Storage
    {
        /// <summary>
        /// Fetches the content from the Storage API
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static IEnumerable<Content> FetchContents(string endPoint = "https://chromedriver.storage.googleapis.com/")
        {
            var httpClient = new WebClient();
            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(httpClient.DownloadString(endPoint));
            var contentResults = new List<Content>();

            foreach (XmlNode contentNode in xmlDocument.GetElementsByTagName("Contents"))
            {
                contentResults.Add(new Content
                {
                    Name = contentNode["Key"].InnerText,
                    Generation = contentNode["Generation"].InnerText,
                    MetaGeneration = contentNode["MetaGeneration"].InnerText,
                    LastModified = contentNode["LastModified"].InnerText,
                    ETag = contentNode["ETag"].InnerText,
                    Size = contentNode["Size"].InnerText,
                    AccessLocation = new Uri($"{endPoint}{contentNode["Key"].InnerText}")
                });
            }

            return contentResults;
        }

        /// <summary>
        /// Fetches a specific resource
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static Content FetchResource(string name, string endPoint = "https://chromedriver.storage.googleapis.com/")
        {
            foreach (var content in FetchContents(endPoint))
            {
                if (string.Equals(content.Name, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return content;
                }
            }

            throw new ResourceNotFoundException();
        }
    }
}
