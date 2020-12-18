#### Getting Statuses Retweeted About a User

Gets the 20 most recent retweets about a user.

*Entity:* [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

*Type:* StatusType.RetweetsOfMe

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of tweets to return; max is 200 | int | no |
| IncludeEntities | Include Twitter entities | bool | no |
| IncludeUserEntities | Include user entities | bool | no |
| MaxID | Return tweets earlier than or equal to this ID | ulong | no |
| SinceID | Return tweets later than this ID | ulong | no |
| TrimUser | Return only user status ID and tweet | bool | no |

##### v3.0 Example:

```c#
            var myRetweets =
                await
                (from retweet in twitterCtx.Status
                 where retweet.Type == StatusType.RetweetsOfMe &&
                       retweet.Count == 100
                 select retweet)
                .ToListAsync();

            myRetweets.ForEach(
                retweet => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    retweet.User.Name, retweet.Text));
```

##### v2.1 Example:

```c#
            var myRetweets =
                from retweet in twitterCtx.Status
                where retweet.Type == StatusType.RetweetsOfMe
                select retweet;

            retweets.ToList().ForEach(
                retweet => Console.WriteLine(
                "Name: {0}, Tweet: {1}\n",
                retweet.Retweet.RetweetedUser.Name, retweet.Text));
```

*Twitter API:* [statuses/retweets_of_me](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/get-statuses-retweets_of_me)