using System;
using System.Linq;

namespace LinqToTwitter
{
    public enum StreamEntityType
    {
        Unknown,

        Control,

        Delete,

        DirectMessage,

        Disconnect,

        Event,

        ForUser,

        FriendsList,

        GeoScrub,

        Limit,
        
        Stall,

        Status,

        StatusWithheld,

        TooManyFollows,

        UserWithheld
    }
}
