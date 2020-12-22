#### Querying User Details

Look up details on specified users. Returns up to 100 users.

*Entity:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

*Type:* UserType.Lookup

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeEntities | Include Twitter entities | bool | no |
| ScreenNameList | Comma-separated list of user ScreenNames | string | only if UserID is empty |
| UserIDList | Comma-separated list of user IDs | string | only if ScreenName is empty |
| tweetMode | Set to TweetMode.Extended for 280 characters returned in Status.FullText. | TweetMode | no |

Note: Max number of users is 100 per query

##### v3.0 Example:

```c#
            var userResponse =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.Lookup &&
                       user.ScreenNameList == "JoeMayo,Linq2Tweeter"
                 select user)
                .ToListAsync();

            if (userResponse != null)
                userResponse.ForEach(user => 
                    Console.WriteLine("Name: " + user.ScreenNameResponse));
```

##### v2.1 Example:

```c#
            var users =
                (from user in twitterCtx.User
                 where user.Type == UserType.Lookup &&
                       user.ScreenName == "JoeMayo,LinqToTweeter"
                 select user)
                 .ToList();

            users.ForEach(user => Console.WriteLine("Name: " + user.Name));
```

*Twitter API:* [users/lookup](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-users-lookup)