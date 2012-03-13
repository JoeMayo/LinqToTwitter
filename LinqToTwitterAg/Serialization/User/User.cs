using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string profile_sidebar_border_color { get; set; }
        [DataMember]
        public bool profile_background_tile { get; set; }
        [DataMember]
        public string profile_sidebar_fill_color { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public string profile_image_url { get; set; }
        [DataMember]
        public string profile_image_url_https { get; set; }
        [DataMember]
        public DateTime created_at { get; set; }
        [DataMember]
        public string profile_link_color { get; set; }
        [DataMember]
        public int favourites_count { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public bool contributors_enabled { get; set; }
        [DataMember]
        public int utc_offset { get; set; }
        [DataMember]
        public ulong id { get; set; }
        [DataMember]
        public bool profile_use_background_image { get; set; }
        [DataMember]
        public string profile_text_color { get; set; }
        [DataMember]
        public bool is_protected { get; set; } // protected is a reserved word!
        [DataMember]
        public int followers_count { get; set; }
        [DataMember]
        public string lang { get; set; }
        [DataMember]
        public bool verified { get; set; }
        [DataMember]
        public string profile_background_color { get; set; }
        [DataMember]
        public bool geo_enabled { get; set; }
        [DataMember]
        public bool notifications { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string time_zone { get; set; }
        [DataMember]
        public int friends_count { get; set; }
        [DataMember]
        public int statuses_count { get; set; }
        [DataMember]
        public string profile_background_image_url { get; set; }
        [DataMember]
        public string profile_background_image_url_https { get; set; }
        [DataMember]
        public bool default_profile_image { get; set; }
        [DataMember]
        public bool default_profile { get; set; }
        [DataMember]
        public Status status { get; set; }
        [DataMember]
        public string screen_name { get; set; }
        [DataMember]
        public bool following { get; set; }
        [DataMember]
        public bool show_all_inline_media { get; set; }
        [DataMember]
        public bool follow_request_sent { get; set; }
        [DataMember]
        public bool is_translator { get; set; }
        [DataMember]
        public int listed_count { get; set; }

        public static User Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            // the order here is what is typically returned from Twitter in Json
            var id_str = dictionary.GetValue("id_str", "0");
            var id = id_str.GetULong(0UL);
            var listed_count = dictionary.GetValue("listed_count", 0);
            var notifications = dictionary.GetValue("notifications", false);
            var time_zone = dictionary.GetValue("time_zone", String.Empty);
            var profile_background_color = dictionary.GetValue("profile_background_color", String.Empty);
            // protected is a reserved word...
            var is_protected = dictionary.GetValue("is_protected", false);
            var location = dictionary.GetValue("location", String.Empty);
            var contributors_enabled = dictionary.GetValue("contributors_enabled", false);
            var verified = dictionary.GetValue("verified", false);
            var profile_background_image_url = dictionary.GetValue("profile_background_image_url", String.Empty);
            var profile_image_url_https = dictionary.GetValue("profile_image_url_https", String.Empty);
            var name = dictionary.GetValue("name", String.Empty);
            var url = dictionary.GetValue("url", String.Empty);
            var show_all_inline_media = dictionary.GetValue("show_all_inline_media", false);
            var following = dictionary.GetValue("following", false);
            var geo_enabled = dictionary.GetValue("geo_enabled", false);
            var utc_offset = dictionary.GetValue("utc_offset", 0);
            var profile_text_color = dictionary.GetValue("profile_text_color", String.Empty);
            var description = dictionary.GetValue("description", String.Empty);
            var default_profile_image = dictionary.GetValue("default_profile_image", false);
            var profile_sidebar_fill_color = dictionary.GetValue("profile_sidebar_fill_color", String.Empty);
            var default_profile = dictionary.GetValue("default_profile", false);
            var profile_background_tile = dictionary.GetValue("profile_background_tile", false);
            var friends_count = dictionary.GetValue("friends_count", 0);
            var status = dictionary.GetNested<Status>("status", serializer);
            var is_translator = dictionary.GetValue("is_translator", false);
            var statuses_count = dictionary.GetValue("statuses_count", 0);
            var created_at = dictionary.GetValue("created_at", DateTime.MinValue);
            var profile_image_url = dictionary.GetValue("profile_image_url", String.Empty);
            var follow_request_sent = dictionary.GetValue("follow_request_sent", false);
            var profile_background_image_url_https = dictionary.GetValue("profile_background_image_url_https", String.Empty);
            var favourites_count = dictionary.GetValue("favourites_count", 0);
            var profile_link_color = dictionary.GetValue("profile_link_color", String.Empty);
            var profile_sidebar_border_color = dictionary.GetValue("profile_sidebar_border_color", String.Empty);
            var lang = dictionary.GetValue("lang", String.Empty);
            var profile_use_background_image = dictionary.GetValue("profile_use_background_image", false);
            var followers_count = dictionary.GetValue("followers_count", 0);
            var screen_name = dictionary.GetValue<String>("screen_name");

            return new User
            {
                id = id,
                screen_name = screen_name,
                name = name,
                listed_count = listed_count,
                notifications = notifications,
                time_zone = time_zone,
                profile_background_color = profile_background_color,
                // protected is a reserved word...
                is_protected = is_protected,
                location = location,
                contributors_enabled = contributors_enabled,
                verified = verified,
                profile_background_image_url = profile_background_image_url,
                profile_image_url_https = profile_image_url_https,
                url = url,
                show_all_inline_media = show_all_inline_media,
                following = following,
                geo_enabled = geo_enabled,
                utc_offset = utc_offset,
                profile_text_color = profile_text_color,
                description = description,
                default_profile_image = default_profile_image,
                profile_sidebar_fill_color = profile_sidebar_fill_color,
                default_profile = default_profile,
                profile_background_tile = profile_background_tile,
                friends_count = friends_count,
                status = status,
                is_translator = is_translator,
                statuses_count = statuses_count,
                created_at = created_at,
                profile_image_url = profile_image_url,
                follow_request_sent = follow_request_sent,
                profile_background_image_url_https = profile_background_image_url_https,
                favourites_count = favourites_count,
                profile_link_color = profile_link_color,
                profile_sidebar_border_color = profile_sidebar_border_color,
                lang = lang,
                profile_use_background_image = profile_use_background_image,
                followers_count = followers_count
            };
        }
    }
}
