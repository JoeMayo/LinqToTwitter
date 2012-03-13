using System.Collections.Generic;
using System.Runtime.Serialization;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class Entities
    {
        [DataMember]
        public UrlEntity[] urls { get; set; }
        [DataMember]
        public UserEntity[] users { get; set; }
        [DataMember]
        public HashtagEntity[] hashes { get; set; }
        [DataMember]
        public MediaEntity[] media { get; set; }

        public static Entities Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new Entities
            {
                urls = dictionary.GetNested<UrlEntity[]>("urls", serializer),
                users = dictionary.GetNested<UserEntity[]>("user_mentions", serializer),
                hashes = dictionary.GetNested<HashtagEntity[]>("hashtags", serializer),
                media = dictionary.GetNested<MediaEntity[]>("media", serializer)
            };
        }
    }
}
