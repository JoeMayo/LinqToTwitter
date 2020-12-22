#### Querying User Contributees

Get list of users that the specified user can contribute to.

*Entity:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

*Type:* UserType.Contributees

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeEntities | Include Twitter entities | bool | no |
| UserID | ID of user | ulong | only if screenName is empty |
| ScreenName | Screen name of user | string | only if userID is empty |
| SkipStatus | Exclude status information | bool | no |

##### v3.0 Example:

```c#
            var users =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.Contributees &&
                       user.ScreenName == "biz"
                 select user)
                .ToListAsync();

            if (users != null)
                users.ForEach(user => 
                    Console.WriteLine("User: " + user.ScreenNameResponse));
```

##### v2.1 Example:

```c#
            var users =
                (from user in twitterCtx.User
                 where user.Type == UserType.Contributees &&
                       user.ScreenName == "biz"
                 select user)
                .ToList();

            users.ForEach(
                user => Console.WriteLine("User: " + user.Name));
```

*Twitter API:* [users/contributees](https://developer.twitter.com/en/docs/api-reference-index)