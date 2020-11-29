#### Listing Friends

Gets IDs of people being followed by the user.

*Entity:* [[Friendship|Friendship Entity]]
*Type:* FriendshipType..FriendsIDs

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of IDs to return | int | no |
| Cursor | Separates results into pages | long | no |
| ScreenName | Screen name of user | string | only if UserID is empty |
| UserID | ID of user | string | only if ScreenName is empty |

##### v3.0 Example:
```c#
            var friendList =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.FriendIDs &&
                       friend.ScreenName == "JoeMayo"
                 select friend)
                .SingleOrDefaultAsync();

            if (friendList != null &&
                friendList.IDInfo != null &&
                friendList.IDInfo.IDs != null)
            {
                friendList.IDInfo.IDs.ForEach(id =>
                    Console.WriteLine("Follower ID: " + id));
            }
```

##### v2.1 Example:
```c#
            var friendList =
                (from friend in twitterCtx.SocialGraph
                 where friend.Type == SocialGraphType.Friends &&
                       friend.ScreenName == "JoeMayo"
                 select friend)
                 .SingleOrDefault();

            foreach (var id in friendList.IDs)
            {
                Console.WriteLine("Friend ID: " + id);
            }
```

*Twitter API:* [friends/ids](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-friends-ids)