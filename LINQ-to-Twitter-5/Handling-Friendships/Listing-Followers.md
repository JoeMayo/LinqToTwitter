#### Listing Followers

Gets IDs of people following the user.

*Entity:* [[Friendship|Friendship Entity]]
*Type:* FriendshipType.FollowerIDs

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of IDs to return | int | no |
| Cursor | Separates results into pages | long | no |
| ScreenName | Screen name of user | string | only if userID is empty |
| UserID | ID of user | string | only if screenName is empty |

##### v3.0 Example:
```c#
            var followers =
                await
                (from follower in twitterCtx.Friendship
                 where follower.Type == FriendshipType.FollowerIDs &&
                       follower.UserID == "15411837"
                 select follower)
                .SingleOrDefaultAsync();

            if (followers != null && 
                followers.IDInfo != null && 
                followers.IDInfo.IDs != null)
            {
                followers.IDInfo.IDs.ForEach(id =>
                    Console.WriteLine("Follower ID: " + id)); 
            }
```

##### v2.1 Example:
```c#
            var followers =
                (from follower in twitterCtx.SocialGraph
                 where follower.Type == SocialGraphType.Followers &&
                       follower.ID == "15411837"
                 select follower)
                 .SingleOrDefault();

            followers.IDs.ForEach(id => Console.WriteLine("Follower ID: " + id));
```

*Twitter API:* [followers/ids](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-followers-ids)