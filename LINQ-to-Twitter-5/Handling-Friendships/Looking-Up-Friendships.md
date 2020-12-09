#### Looking Up Friendships

Look up friendship information.

*Entity:* [Friendship](../LINQ-to-Twitter-Entities/Friendship-Entity.md)

*Type:* FriendshipType.Lookup

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ScreenName | Comma-separated list of Twitter screen names | string | only if UserID is empty |
| UserID | Comma-separated list of Twitter user IDs | string | only if ScreenName is empty |

##### v3.0 Example:

```c#
            var relationships =
                await
                (from look in twitterCtx.Friendship
                 where look.Type == FriendshipType.Lookup &&
                       look.UserID == "15411837,16761255"
                 select look.Relationships)
                .SingleOrDefaultAsync();

            if (relationships != null)
                relationships.ForEach(rel => 
                    Console.WriteLine(
                        "Relationship to " + rel.ScreenName + 
                        ", is Following: " + rel.Following + 
                        ", Followed By: " + rel.FollowedBy));
```

##### v2.1 Example:

```c#
            var relationships =
                (from look in twitterCtx.Friendship
                 where look.Type == FriendshipType.Lookup &&
                       look.UserID == "15411837,16761255"
                 select look.Relationships)
                .SingleOrDefault();

            relationships.ForEach(rel => Console.WriteLine(
                "Relationship to " + rel.ScreenName + " is Following: " + rel.Following + " Followed By: " + rel.FollowedBy));
```

*Twitter API:* [friendships/lookup](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-friendships-lookup)