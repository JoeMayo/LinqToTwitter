#### Querying the Mentions Timeline

Gets the 20 most recent statuses where the user has been mentioned.  A mention is defined as a tweet that contains the @ScreenName of the user. Only works for authorized user.

*Entity:* [[Status|Status Entity]]
*Type:* StatusType.Mentions

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of tweets to return; max is 200 | int | no |
| IncludeContributorDetails | Include additional user details | bool | no |
| IncludeEntities | Include Twitter entities | bool | no |
| MaxID | Return tweets earlier than or equal to this ID | ulong | no |
| SinceID | Return tweets later than this ID | ulong | no |
| TrimUser | Return only user status ID and tweet | bool | no |

##### v3.0 Example:

```c#
            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Mentions &&
                       tweet.ScreenName == "JoeMayo"
                 select tweet)
                .ToListAsync();

            PrintTweetsResults(tweets);
```

##### v2.1 Example:

```c#
            var myMentions =
                from mention in twitterCtx.Status
                where mention.Type == StatusType.Mentions
                select mention;

            myMentions.ToList().ForEach(
                mention => Console.WriteLine(
                    "Name: {0}, Tweet[{1}]: {2}\n",
                    mention.User.Name, mention.StatusID, mention.Text));
```

*Twitter API:* [statuses/mentions_timeline](https://developer.twitter.com/en/docs/tweets/timelines/api-reference/get-statuses-mentions_timeline)