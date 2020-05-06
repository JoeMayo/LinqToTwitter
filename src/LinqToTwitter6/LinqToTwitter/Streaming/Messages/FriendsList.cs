using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LinqToTwitter
{
    public class FriendsList
    {
        public FriendsList() { }
        public FriendsList(JsonElement friends)
        {
            if (friends.TryGetProperty("friends", out JsonElement friendsArr))
                Friends =
                    (from  friend in friendsArr.EnumerateArray()
                     select friend.GetUInt64())
                    .ToList();
            else
                Friends = new List<ulong>();
        }

        public List<ulong> Friends { get; set; }
    }
}
