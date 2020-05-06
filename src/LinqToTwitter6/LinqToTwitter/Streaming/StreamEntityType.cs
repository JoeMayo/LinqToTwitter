using System;
using System.Linq;

namespace LinqToTwitter.Streaming
{
    public enum StreamEntityType
    {
        Unknown,

        ParseError,

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
