#### Getting Retweeter IDs

Gets up to 100 IDs of users that retweeted a specific tweet.

*Entity:* [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

*Type:* StatusType.Retweeters

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | Tweet ID | ulong | yes |
| Cursor | Next page of data to return | long | no |
##### v3.0 Example:
```c#
            ulong tweetID = 210591841312190464;

            var status =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Retweeters &&
                       tweet.ID == tweetID
                 select tweet)
                .SingleOrDefaultAsync();

            if (status != null && status.User != null)
                status.Users.ForEach(
                    userID => Console.WriteLine("User ID: " + userID));
```

##### v2.1 Example:
```c#
            var status =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Retweeters &&
                       tweet.ID == "210591841312190464"
                 select tweet)
                .SingleOrDefault();

            status.Users.ForEach(
                userID => Console.WriteLine("User ID: " + userID));
```

*Twitter API:* [statuses/retweeters/ids](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/get-statuses-retweeters-ids)