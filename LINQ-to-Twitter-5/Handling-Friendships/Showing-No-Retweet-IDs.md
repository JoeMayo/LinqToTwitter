#### Showing No Retweet IDs

Show IDs of all users for whom the logged in user doesn't want to see retweets.

*Entity:* [[Friendship|Friendship Entity]]
*Type:* FriendshipType.NoRetweetIDs

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| None | - | - | - |

*Return Type:* User

##### v3.0 Example:

```c#
            var friendship =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.NoRetweetIDs
                 select friend)
                .SingleOrDefaultAsync();

            if (friendship != null && 
                friendship.IDInfo != null && 
                friendship.IDInfo.IDs != null)
            {
                var ids =
                    (from id in friendship.IDInfo.IDs
                     select id.ToString())
                    .ToArray();

                Console.WriteLine("\nIDs: " + string.Join(",", ids)); 
            }
```

##### v2.1 Example:

```c#
var friendship =
    (from friend in twitterCtx.Friendship
    where friend.Type == FriendshipType.NoRetweetIDs
    select friend)
    .First();

Console.WriteLine("\nIDs: " + string.Join(",", friendship.IDInfo.IDs.ToArray()));
```

*Twitter API:* [friendships/no_retweets/ids](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/post-friendships-create)