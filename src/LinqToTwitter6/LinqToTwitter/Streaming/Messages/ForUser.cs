using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LinqToTwitter
{
    public class ForUser
    {
        public ForUser() { }
        public ForUser(JsonElement user)
        {
            UserID = user.GetProperty("for_user").GetUInt64();
            var message = user.GetProperty("message");

            if (message.TryGetProperty("friends", out JsonElement friends))
                Friends =
                    (from JsonElement friend in friends.EnumerateArray()
                     select friend.GetUInt64())
                    .ToList();
            else
                Friends = new List<ulong>();
        }

        public ulong UserID { get; set; }

        public List<ulong> Friends { get; set; }
    }
}
