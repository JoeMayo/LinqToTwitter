#### Showing Followers

Gets User entities for followers.

*Entity:* [[Friendship|Friendship Entity]]
*Type:* FriendshipType.FollowersList

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Supports paging through multiple result sets | long | no |
| IncludeUserEntities | False to leave entities out (default: true) | bool | no |
| ScreenName | Screen name of user to get friends for | string | only if userID is empty |
| SkipStatus | Don't include User's latest tweet | bool | no |
| UserID | ID of user to get friends for | string | only if screenName is empty |

##### v3.0 Example:

```c#
            var friendship =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.FollowersList &&
                       friend.ScreenName == "JoeMayo"
                 select friend)
                .SingleOrDefaultAsync();

            if (friendship != null && friendship.Users != null)
                friendship.Users.ForEach(friend =>
                    Console.WriteLine(
                        "ID: {0} Name: {1}",
                        friend.UserIDResponse, friend.ScreenNameResponse));
```

##### v2.1 Example:

```c#
            var friendship =
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.FollowersList &&
                       friend.ScreenName == "JoeMayo"
                 select friend)
                .SingleOrDefault();

            friendship.Users.ForEach(friend =>
                Console.WriteLine(
                    "ID: {0} Name: {1}",
                    friend.Identifier.UserID, friend.Identifier.ScreenName));
```

*Twitter API:* [followers/list](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-followers-list)