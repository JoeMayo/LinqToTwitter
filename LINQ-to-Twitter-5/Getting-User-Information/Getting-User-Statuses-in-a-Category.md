#### Getting User Statuses in a Category

Get list of user statuses in a category.

*Entity:* [[User|User Entity]]
*Type:* UserType.CategoryStatus

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Slug | Short list name | string | yes |

##### v3.0 Example:

```c#
            var catUsers =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.CategoryStatus &&
                       user.Slug == "Technology"
                 select user)
                .ToListAsync();

            if (catUsers != null)
            {
                Console.WriteLine("Tweets: \n");

                catUsers.ForEach(user =>
                {
                    if (user != null && user.Status != null)
                        Console.WriteLine(
                            "User: {0}\nTweet: {1}\n",
                            user.ScreenNameResponse,
                            user.Status == null ?
                                "<Tweet not available.>" :
                                user.Status.Text);
                }); 
            }
```

##### v2.1 Example:

```c#
            var catTweets =
                (from user in twitterCtx.User
                 where user.Type == UserType.CategoryStatus &&
                       user.Slug == "Technology"
                 select user)
                .ToList();

            Console.WriteLine("Tweets from Suggested Users in Technology Category: \n");

            catTweets.ForEach(tweet => 
                Console.WriteLine(
                    "User: {0}\nTweet: {1}\n", 
                    tweet.Name, 
                    tweet.Status == null ?
                        "<Tweet not available.>" : 
                        tweet.Status.Text));
```

*Twitter API:* [users/suggestions/:slug/members](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-users-suggestions-slug-members)