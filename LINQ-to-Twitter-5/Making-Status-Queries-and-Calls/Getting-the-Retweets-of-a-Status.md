#### Getting the Retweets of a Status

Gets up to 100 retweets of a tweet.

*Entity:* [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

*Type:* StatusType.Retweets

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | Identifies tweet to get retweets for | ulong | yes |
| Count | Number of tweets to return; max is 100 | int | no |
| TrimUser | Return only user status ID and tweet | bool | no |

##### v3.0 Example:

```c#
            ulong tweetID = 196991337554378752;

            var publicTweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Retweets &&
                       tweet.ID == tweetID
                 select tweet)
                .ToListAsync();

            if (publicTweets != null)
                publicTweets.ForEach(tweet =>
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "@{0} {1} ({2})",
                            tweet.User.ScreenNameResponse,
                            tweet.Text,
                            tweet.RetweetCount);
                });
```

##### v2.1 Example:

```c#
            var publicTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Retweets &&
                      tweet.ID == "196991337554378752"
                select tweet;

            publicTweets.ToList().ForEach(
                tweet =>
                {
                    Console.WriteLine(
                        "@{0} {1} ({2})",
                        tweet.User.Identifier.ScreenName,
                        tweet.Text,
                        tweet.RetweetCount);
                });
```

*Twitter API:* [statuses/retweets/:id](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/get-statuses-retweets-id)