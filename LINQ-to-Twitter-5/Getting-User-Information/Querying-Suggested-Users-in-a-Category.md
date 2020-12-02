#### Querying Suggested Users in a Category

Get list of users in suggested users category.

*Entity:* [[User|User Entity]]
*Type:* UserType.Category

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Lang | Language to display | string | no |
| Slug | Short list name | string | yes |

##### v3.0 Example:

```c#
            var userResponse =
                await
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Category &&
                       tweet.Slug == "Funny"
                 select tweet)
                .SingleOrDefaultAsync();

            if (userResponse != null && 
                userResponse.Categories != null && 
                userResponse.Categories.Any() && 
                userResponse.Categories.First().Users != null)
            {
                List<User> users = userResponse.Categories.First().Users;

                users.ForEach(user =>
                    Console.WriteLine("User: " + user.ScreenNameResponse)); 
            }
```

##### v2.1 Example:

```c#
            var users =
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Category &&
                       tweet.Slug == "funny"
                 select tweet)
                 .ToList();

            users.ForEach(
                user => Console.WriteLine("User: " + user.Name));
```

*Twitter API:* [users/suggestions/:slug](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-users-suggestions-slug)