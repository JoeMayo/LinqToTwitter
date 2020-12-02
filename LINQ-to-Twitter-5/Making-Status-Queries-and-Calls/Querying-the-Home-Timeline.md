#### Querying the Home Timeline

Gets the 20 most recent statuses, including retweets, for a user and user's friends.

This query returns the tweets and retweets shown on a user's home page when they log into Twitter.

The maximum number of tweets/retweets returned is 800.

*Entity:* [[Status|Status Entity]]
*Type:* StatusType.Home

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeContributorDetails | Include additional user details | bool | no |
| Count | Number of tweets to return; max is 200; default is 20 | int | no |
| ExcludeReplies | Exclude tweet replies | bool | no |
| MaxID | Return tweets earlier than or equal to this ID | ulong | no |
| SinceID | Return tweets later than this ID | ulong | no |
| TrimUser | Return only user status ID and tweet | bool | no |

##### v3.0 Example:
```c#
            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home
                 select tweet)
                .ToListAsync();

            PrintTweetsResults(tweets);
```

##### v2.1 Example:
```c#
            var tweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Home
                select tweet;

            Console.WriteLine("\nTweets for " + twitterCtx.UserName + "\n");

            tweets.ToList().ForEach(
                tweet => Console.WriteLine(
                "Name: {0}, Tweet: {1}\n",
                tweet.User.Name, tweet.Text));
```

*Twitter API:* [statuses/home_timeline](https://developer.twitter.com/en/docs/tweets/timelines/api-reference/get-statuses-home_timeline)