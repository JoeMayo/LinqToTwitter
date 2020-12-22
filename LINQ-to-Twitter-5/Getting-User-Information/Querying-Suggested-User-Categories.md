#### Querying Suggested User Categories

Get list of categories for suggested users.

*Entity:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

*Type:* UserType.Categories

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Lang | Language to display | string | no |

##### v3.0 Example:

```c#
            var user =
                await
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Categories
                 select tweet)
                .SingleOrDefaultAsync();

            if (user != null && user.Categories != null)
                user.Categories.ForEach(cat => 
                    Console.WriteLine("Category: " + cat.Name));
```

##### v2.1 Example:

```c#
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Categories
                select tweet;

            var user = users.SingleOrDefault();

            user.Categories.ForEach(
                cat => Console.WriteLine("Category: " + cat.Name));
```

*Twitter API:* [users/suggestions](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-users-suggestions)