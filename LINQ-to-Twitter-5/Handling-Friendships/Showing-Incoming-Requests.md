#### Showing Incoming Requests

Show IDs of all users requesting friendship with logged in user.

*Entity:* [Friendship](../LINQ-to-Twitter-Entities/Friendship-Entity.md)

*Type:* FriendshipType.Incoming

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Separates results into pages | long | no |

*Return Type:* User

##### v3.0 Example:

```c#
            var request =
                await
                (from req in twitterCtx.Friendship
                 where req.Type == FriendshipType.Incoming
                 select req)
                .SingleOrDefaultAsync();

            if (request != null && 
                request.IDInfo != null && 
                request.IDInfo.IDs != null)
            {
                request.IDInfo.IDs.ForEach(req => Console.WriteLine(req));
            }
```

##### v2.1 Example:

```c#
            var request =
                (from req in twitterCtx.Friendship
                 where req.Type == FriendshipType.Incoming
                 select req)
                 .FirstOrDefault();

            request.IDInfo.IDs.ForEach(req => Console.WriteLine(req));
```

*Twitter API:* [friendships/incoming](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-friendships-incoming)