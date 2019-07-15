using System;

namespace Netlenium.WebAPI.Github
{
    [Serializable]
    public class User
    {
        public int ID { get; set; }
        
        public string Login { get; set; }

        public string NodeID { get; set; }

        public Uri AvatarURL { get; set; }

        public string GravatarID { get; set; }

        public Uri URL { get; set; }

        public Uri HtmlURL { get; set; }

        public Uri FollowersURL { get; set; }

        public Uri FollowingURL { get; set; }

        public Uri GistsURL { get; set; }

        public Uri StarredURL { get; set; }

        public Uri SubscriptionsURL { get; set; }

        public Uri OrganizationsURL { get; set; }

        public Uri ReposURL { get; set; }

        public Uri EventsURL { get; set; }

        public Uri ReceivedEventsURL { get; set; }

        public string Type { get; set; }

        public bool SiteAdmin { get; set; }
    }
}
