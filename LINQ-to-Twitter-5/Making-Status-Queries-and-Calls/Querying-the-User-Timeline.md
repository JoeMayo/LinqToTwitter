#### Querying the User Timeline

Gets the 20 most recent statuses for the authorized user (doesn't include user's friends). The max number of tweets you can retrieve is 3200.

*Entity:* [[Status|Status Entity]]
*Type:* StatusType.User

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeContributorDetails | Include additional user details | bool | no |
| Count | Number of tweets to return; max is 200 | int | no |
| ExcludeReplies | Exclude tweet replies | bool | no |
| ID | ID or ScreenName of user | ulong | only if UserID and ScreenName are empty |
| IncludeMyRetweet | Include retweets | bool | no |
| MaxID | Return tweets earlier than or equal to this ID | ulong | no |
| ScreenName | Screen name of user | string | only if ID and UserID are empty |
| SinceID | Return tweets later than this ID | ulong | no |
| TrimUser | Trim user details | bool | no |
| UserID | ID of user | ulong | only if ID and ScreenName are empty |

##### v3.0 Example:

```c#
        static async Task RunUserTimelineQueryAsync(TwitterContext twitterCtx)
        {
            //List<Status> tweets =
            //    await
            //    (from tweet in twitterCtx.Status
            //     where tweet.Type == StatusType.User &&
            //           tweet.ScreenName == "JoeMayo"
            //     select tweet)
            //    .ToListAsync();

            const int MaxTweetsToReturn = 200;
            const int MaxTotalResults = 100;

            // oldest id you already have for this search term
            ulong sinceID = 1;

            // used after the first query to track current session
            ulong maxID;

            var combinedSearchResults = new List<Status>();

            List<Status> tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User &&
                       tweet.ScreenName == "JoeMayo" &&
                       tweet.Count == MaxTweetsToReturn &&
                       tweet.SinceID == sinceID &&
                       tweet.TweetMode == TweetMode.Extended
                 select tweet)
                .ToListAsync();

            if (tweets != null)
            {
                combinedSearchResults.AddRange(tweets);
                ulong previousMaxID = ulong.MaxValue;
                do
                {
                    // one less than the newest id you've just queried
                    maxID = tweets.Min(status => status.StatusID) - 1;

                    Debug.Assert(maxID < previousMaxID);
                    previousMaxID = maxID;

                    tweets =
                        await
                        (from tweet in twitterCtx.Status
                         where tweet.Type == StatusType.User &&
                               tweet.ScreenName == "JoeMayo" &&
                               tweet.Count == MaxTweetsToReturn &&
                               tweet.MaxID == maxID &&
                               tweet.SinceID == sinceID &&
                               tweet.TweetMode == TweetMode.Extended
                         select tweet)
                        .ToListAsync();

                    combinedSearchResults.AddRange(tweets);

                } while (tweets.Any() && combinedSearchResults.Count < MaxTotalResults);

                PrintTweetsResults(tweets);
            }
            else
            {
                Console.WriteLine("No entries found.");
            }
        }
```

##### v2.1 Example:

```c#
            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ScreenName == "JoeMayo"
                select tweet;

            statusTweets.ToList().ForEach(
                tweet => Console.WriteLine(
                "Name: {0}, Tweet: {1}\n",
                tweet.User.Name, tweet.Text));
```

*Twitter API:* [statuses/user_timeline](https://developer.twitter.com/en/docs/tweets/timelines/api-reference/get-statuses-user_timeline)