using System;

namespace Netlenium.WebAPI.GitHub
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        
        public string Login { get; set; }

        public string NodeId { get; set; }

        public Uri AvatarUrl { get; set; }

        public string GravatarId { get; set; }

        public Uri Url { get; set; }

        public Uri HtmlUrl { get; set; }

        public Uri FollowersUrl { get; set; }

        public Uri FollowingUrl { get; set; }

        public Uri GistsUrl { get; set; }

        public Uri StarredUrl { get; set; }

        public Uri SubscriptionsUrl { get; set; }

        public Uri OrganizationsUrl { get; set; }

        public Uri ReposUrl { get; set; }

        public Uri EventsUrl { get; set; }

        public Uri ReceivedEventsUrl { get; set; }

        public string Type { get; set; }

        public bool SiteAdmin { get; set; }
    }
}
