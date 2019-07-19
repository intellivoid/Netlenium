using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json.Linq;

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
            var releases = new List<Release>();
            var requestUrl = $"{Endpoint}repos/{owner}/{repo}/releases";
            var responseParsed = JArray.Parse(httpClient.DownloadString(requestUrl));

            foreach(var jToken in responseParsed)
            {
                var jsonObject = (JObject) jToken;
                releases.Add(ParseReleaseJObject(jsonObject));
            }

            return releases;
        }

        /// <summary>
        /// Gets a single release from the repo by release id
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public static Release GetRelease(string owner, string repo, string releaseId)
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
            var requestUrl = $"{Endpoint}repos/{owner}/{repo}/releases/{releaseId}";

            try
            {
                var responseParsed = JObject.Parse(httpClient.DownloadString(requestUrl));
                return ParseReleaseJObject(responseParsed);
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
            var requestUrl = $"{Endpoint}repos/{owner}/{repo}/releases/latest";

            try
            {
                var responseParsed = JObject.Parse(httpClient.DownloadString(requestUrl));
                return ParseReleaseJObject(responseParsed);
            }
            catch (Exception ex)
            {
                throw new ReleaseNotFoundException(ex.Message);
            }
        }

        /// <summary>
        /// Parses a Release as a Json Object
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        private static Release ParseReleaseJObject(JObject jsonObject)
        {
            var releaseObject = new Release
            {
                Id = (int) jsonObject["id"],
                Url = new Uri((string) jsonObject["url"]),
                HtmlUrl = new Uri((string) jsonObject["html_url"]),
                AssetsUrl = new Uri((string) jsonObject["assets_url"]),
                UploadUrl = new Uri((string) jsonObject["upload_url"]),
                TarballUrl = new Uri((string) jsonObject["tarball_url"]),
                ZipballUrl = new Uri((string) jsonObject["zipball_url"]),
                NodeId = (string) jsonObject["node_id"],
                TagName = (string) jsonObject["tag_name"],
                TargetCommitish = (string) jsonObject["target_commitish"],
                Name = (string) jsonObject["name"],
                Body = (string) jsonObject["body"],
                Draft = (bool) jsonObject["draft"],
                PreRelease = (bool) jsonObject["prerelease"],
                CreatedAt = (string) jsonObject["created_at"],
                PublishedAt = (string) jsonObject["published_at"],
                Author = new User
                {
                    Id = (int) jsonObject["author"]["id"],
                    Login = (string) jsonObject["author"]["login"],
                    NodeId = (string) jsonObject["author"]["node_id"],
                    AvatarUrl = new Uri((string) jsonObject["author"]["avatar_url"]),
                    GravatarId = (string) jsonObject["author"]["gravatar_id"],
                    EventsUrl = new Uri((string) jsonObject["author"]["url"]),
                    FollowersUrl = new Uri((string) jsonObject["author"]["followers_url"]),
                    FollowingUrl = new Uri((string) jsonObject["author"]["following_url"]),
                    GistsUrl = new Uri((string) jsonObject["author"]["gists_url"]),
                    StarredUrl = new Uri((string) jsonObject["author"]["starred_url"]),
                    SiteAdmin = (bool) jsonObject["author"]["site_admin"],
                    Type = (string) jsonObject["author"]["type"],
                    HtmlUrl = new Uri((string) jsonObject["author"]["html_url"]),
                    OrganizationsUrl = new Uri((string) jsonObject["author"]["organizations_url"]),
                    ReceivedEventsUrl = new Uri((string) jsonObject["author"]["received_events_url"]),
                    ReposUrl = new Uri((string) jsonObject["author"]["repos_url"]),
                    SubscriptionsUrl = new Uri((string) jsonObject["author"]["subscriptions_url"]),
                    Url = new Uri((string) jsonObject["author"]["url"])
                },
                Assets = new List<Asset>()
            };


            foreach (var jToken in jsonObject["assets"])
            {
                var assetJsonObject = (JObject) jToken;
                releaseObject.Assets.Add(new Asset
                {
                    Id = (int)assetJsonObject["id"],
                    Url = new Uri((string)assetJsonObject["url"]),
                    BrowserDownloadUrl = new Uri((string)assetJsonObject["browser_download_url"]),
                    NodeId = (string)assetJsonObject["node_id"],
                    Name = (string)assetJsonObject["name"],
                    Label = (string)assetJsonObject["label"],
                    State = (string)assetJsonObject["state"],
                    ContentType = (string)assetJsonObject["content_type"],
                    Size = (int)assetJsonObject["size"],
                    DownloadCount = (int)assetJsonObject["download_count"],
                    CreatedAt = (string)assetJsonObject["created_at"],
                    UpdatedAt = (string)assetJsonObject["updated_at"],
                    Uploader = new User
                    {
                        Id = (int)assetJsonObject["uploader"]["id"],
                        Login = (string)assetJsonObject["uploader"]["login"],
                        NodeId = (string)assetJsonObject["uploader"]["node_id"],
                        AvatarUrl = new Uri((string)assetJsonObject["uploader"]["avatar_url"]),
                        GravatarId = (string)assetJsonObject["uploader"]["gravatar_id"],
                        EventsUrl = new Uri((string)assetJsonObject["uploader"]["url"]),
                        FollowersUrl = new Uri((string)assetJsonObject["uploader"]["followers_url"]),
                        FollowingUrl = new Uri((string)assetJsonObject["uploader"]["following_url"]),
                        GistsUrl = new Uri((string)assetJsonObject["uploader"]["gists_url"]),
                        StarredUrl = new Uri((string)assetJsonObject["uploader"]["starred_url"]),
                        SiteAdmin = (bool)assetJsonObject["uploader"]["site_admin"],
                        Type = (string)assetJsonObject["uploader"]["type"],
                        HtmlUrl = new Uri((string)assetJsonObject["uploader"]["html_url"]),
                        OrganizationsUrl = new Uri((string)assetJsonObject["uploader"]["organizations_url"]),
                        ReceivedEventsUrl = new Uri((string)assetJsonObject["uploader"]["received_events_url"]),
                        ReposUrl = new Uri((string)assetJsonObject["uploader"]["repos_url"]),
                        SubscriptionsUrl = new Uri((string)assetJsonObject["uploader"]["subscriptions_url"]),
                        Url = new Uri((string)assetJsonObject["uploader"]["url"])
                    }
                });
            }

            return releaseObject;
        }
    }
}
