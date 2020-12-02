#### Querying User Info

Show information for a user.

*Entity:* [[User|User Entity]]
*Type:* UserType.Show

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeEntities | Include Twitter entities | bool | no |
| ScreenName | Screen name of user | string | only if UserID is empty |
| UserID | ID of user | ulong | only if ScreenName is empty |
| tweetMode | Set to TweetMode.Extended for 280 characters returned in Status.FullText. | TweetMode | no |

##### v3.0 Example:

```c#
            var user =
                await
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Show &&
                       tweet.ScreenName == "JoeMayo"
                 select tweet)
                .SingleOrDefaultAsync();

            if (user != null)
            {
                var name = user.ScreenNameResponse;
                var lastStatus =
                    user.Status == null ? "No Status" : user.Status.Text;

                Console.WriteLine();
                Console.WriteLine(
                    "Name: {0}, Last Tweet: {1}\n", name, lastStatus); 
            }
```

##### v2.1 Example:

```c#
            var user =
                await
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Show &&
                       tweet.ScreenName == "JoeMayo"
                 select tweet)
                .SingleOrDefaultAsync();

            var name = user.ScreenNameResponse;
            var lastStatus = 
                user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine();
            Console.WriteLine(
                "Name: {0}, Last Tweet: {1}\n", name, lastStatus);
```

*Twitter API:* [users/show](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-users-show)