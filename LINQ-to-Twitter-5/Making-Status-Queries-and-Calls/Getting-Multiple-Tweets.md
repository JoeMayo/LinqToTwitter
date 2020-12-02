#### Getting Multiple Tweets

Looks up multiple tweets by ID

*Entity:* [[Status|Status Entity]]
*Type:* StatusType.Lookup

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | Tweet ID to retrieve | ulong | yes |
| IncludeAltText | Includes Alt Text on media entities | bool | no |
| IncludeEntities | Doesn't include entities when set to false (default: true) | bool | no |
| Map | Include tweets that can't be viewed | bool | no |
| TrimUser | Return only user status ID and tweet | bool | no |

##### v4.x Example:

```c#
            string tweetIds = "460788892723978241,462758132448362496,460060836967768064";

            List<Status> tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Lookup &&
                       tweet.TweetIDs == tweetIds
                 select tweet)
                .ToListAsync();

            PrintTweetsResults(tweets);
```

*Twitter API:* [statuses/lookup](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/get-statuses-lookup)