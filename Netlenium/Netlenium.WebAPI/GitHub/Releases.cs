using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Netlenium.WebAPI.GitHub
{
    /// <summary>
    /// Releases Manager for GitHub
    /// </summary>
    public class Releases
    {
        public static Uri Endpoint = new Uri("https://api.github.com/");

        /// <summary>
        /// Retrieves a all releases
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <returns></returns>
        public static List<Release> GetReleases(string owner, string repo)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
            }

            var httpClient = new WebClient();
            httpClient.Headers.Add("User-Agent", "Netlenium");
            var Releases = new List<Release>();
            var RequestUrl = string.Format("{0}repos/{1}/{2}/releases", Endpoint.ToString(), owner, repo);
            var ResponseParsed = JArray.Parse(httpClient.DownloadString(RequestUrl));

            foreach(JObject JsonObject in ResponseParsed)
            {
                Releases.Add(ParseReleaseJObject(JsonObject));
            }

            return Releases;
        }

        /// <summary>
        /// Gets a single release from the repo by release id
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repo"></param>x
        /// <returns></returns>
        public static Release GetRelease(string owner, string repo, string release_id)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            var httpClient = new WebClient();
            httpClient.Headers.Add("User-Agent", "Netlenium");
            var Releases = new List<Release>();
            var RequestUrl = string.Format("{0}repos/{1}/{2}/releases/{3}", Endpoint.ToString(), owner, repo, release_id);

            try
            {
                var ResponseParsed = JObject.Parse(httpClient.DownloadString(RequestUrl));
                return ParseReleaseJObject(ResponseParsed);
            }
            catch(Exception ex)
            {
                throw new ReleaseNotFoundException(ex.Message);
            }
        }

        /// <summary>
        /// Gets a single but latest release from the repo
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <returns></returns>
        public static Release GetLatestRelease(string owner, string repo)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            var httpClient = new WebClient();
            httpClient.Headers.Add("User-Agent", "Netlenium");
            var Releases = new List<Release>();
            var RequestUrl = string.Format("{0}repos/{1}/{2}/releases/latest", Endpoint.ToString(), owner, repo);

            try
            {
                var ResponseParsed = JObject.Parse(httpClient.DownloadString(RequestUrl));
                return ParseReleaseJObject(ResponseParsed);
            }
            catch (Exception ex)
            {
                throw new ReleaseNotFoundException(ex.Message);
            }
        }

        /// <summary>
        /// Parses a Release as a Json Object
        /// </summary>
        /// <param name="JsonObject"></param>
        /// <returns></returns>
        private static Release ParseReleaseJObject(JObject JsonObject)
        {
            var ReleaseObject = new Release()
            {
                ID = (int)JsonObject["id"],
                URL = new Uri((string)JsonObject["url"]),
                HtmlURL = new Uri((string)JsonObject["html_url"]),
                AssetsURL = new Uri((string)JsonObject["assets_url"]),
                UploadURL = new Uri((string)JsonObject["upload_url"]),
                TarballURL = new Uri((string)JsonObject["tarball_url"]),
                ZipballURL = new Uri((string)JsonObject["zipball_url"]),
                NodeID = (string)JsonObject["node_id"],
                TagName = (string)JsonObject["tag_name"],
                TargetCommitish = (string)JsonObject["target_commitish"],
                Name = (string)JsonObject["name"],
                Body = (string)JsonObject["body"],
                Draft = (bool)JsonObject["draft"],
                PreRelease = (bool)JsonObject["prerelease"],
                CreatedAt = (string)JsonObject["created_at"],
                PublishedAt = (string)JsonObject["published_at"],
                Author = new User()
                {
                    ID = (int)JsonObject["author"]["id"],
                    Login = (string)JsonObject["author"]["login"],
                    NodeID = (string)JsonObject["author"]["node_id"],
                    AvatarURL = new Uri((string)JsonObject["author"]["avatar_url"]),
                    GravatarID = (string)JsonObject["author"]["gravatar_id"],
                    EventsURL = new Uri((string)JsonObject["author"]["url"]),
                    FollowersURL = new Uri((string)JsonObject["author"]["followers_url"]),
                    FollowingURL = new Uri((string)JsonObject["author"]["following_url"]),
                    GistsURL = new Uri((string)JsonObject["author"]["gists_url"]),
                    StarredURL = new Uri((string)JsonObject["author"]["starred_url"]),
                    SiteAdmin = (bool)JsonObject["author"]["site_admin"],
                    Type = (string)JsonObject["author"]["type"],
                    HtmlURL = new Uri((string)JsonObject["author"]["html_url"]),
                    OrganizationsURL = new Uri((string)JsonObject["author"]["organizations_url"]),
                    ReceivedEventsURL = new Uri((string)JsonObject["author"]["received_events_url"]),
                    ReposURL = new Uri((string)JsonObject["author"]["repos_url"]),
                    SubscriptionsURL = new Uri((string)JsonObject["author"]["subscriptions_url"]),
                    URL = new Uri((string)JsonObject["author"]["url"])
                }
            };

            ReleaseObject.Assets = new List<Asset>();

            foreach (JObject AssetJsonObject in JsonObject["assets"])
            {
                ReleaseObject.Assets.Add(new Asset()
                {
                    ID = (int)AssetJsonObject["id"],
                    URL = new Uri((string)AssetJsonObject["url"]),
                    BrowserDownloadURL = new Uri((string)AssetJsonObject["browser_download_url"]),
                    NodeID = (string)AssetJsonObject["node_id"],
                    Name = (string)AssetJsonObject["name"],
                    Label = (string)AssetJsonObject["label"],
                    State = (string)AssetJsonObject["state"],
                    ContentType = (string)AssetJsonObject["content_type"],
                    Size = (int)AssetJsonObject["size"],
                    DownloadCount = (int)AssetJsonObject["download_count"],
                    CreatedAt = (string)AssetJsonObject["created_at"],
                    UpdatedAt = (string)AssetJsonObject["updated_at"],
                    Uploader = new User()
                    {
                        ID = (int)AssetJsonObject["uploader"]["id"],
                        Login = (string)AssetJsonObject["uploader"]["login"],
                        NodeID = (string)AssetJsonObject["uploader"]["node_id"],
                        AvatarURL = new Uri((string)AssetJsonObject["uploader"]["avatar_url"]),
                        GravatarID = (string)AssetJsonObject["uploader"]["gravatar_id"],
                        EventsURL = new Uri((string)AssetJsonObject["uploader"]["url"]),
                        FollowersURL = new Uri((string)AssetJsonObject["uploader"]["followers_url"]),
                        FollowingURL = new Uri((string)AssetJsonObject["uploader"]["following_url"]),
                        GistsURL = new Uri((string)AssetJsonObject["uploader"]["gists_url"]),
                        StarredURL = new Uri((string)AssetJsonObject["uploader"]["starred_url"]),
                        SiteAdmin = (bool)AssetJsonObject["uploader"]["site_admin"],
                        Type = (string)AssetJsonObject["uploader"]["type"],
                        HtmlURL = new Uri((string)AssetJsonObject["uploader"]["html_url"]),
                        OrganizationsURL = new Uri((string)AssetJsonObject["uploader"]["organizations_url"]),
                        ReceivedEventsURL = new Uri((string)AssetJsonObject["uploader"]["received_events_url"]),
                        ReposURL = new Uri((string)AssetJsonObject["uploader"]["repos_url"]),
                        SubscriptionsURL = new Uri((string)AssetJsonObject["uploader"]["subscriptions_url"]),
                        URL = new Uri((string)AssetJsonObject["uploader"]["url"])
                    }
                });
            }

            return ReleaseObject;
        }
    }
}
