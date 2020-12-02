#### Querying Statuses

Gets a specific tweet.

*Entity:* [[Status|Status Entity]]
*Type:* StatusType.Show

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | Tweet ID to retrieve | ulong | yes |
| IncludeMyRetweet | Populates CurrentUserRetweet in response if set to true | bool | no |
| IncludeEntities | Doesn't include entities when set to false (default: true) | bool | no |
| TrimUser | Return only user status ID and tweet | bool | no |
| IncludeAltText | Includes Alt Text on media entities | bool | no |

##### v3.0+ Example:

```c#
            ulong tweetID = 806571633754284032;

            List<Status> status =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Show &&
                       tweet.ID == tweetID &&
                       tweet.TweetMode == TweetMode.Extended &&
                       tweet.IncludeAltText == true
                 select tweet)
                .ToListAsync();

            if (status != null)
            {
                Console.WriteLine("\nTweets: \n");
                status.ForEach(tweet =>
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "User: " + tweet.User.Name +
                            "\nTweet: " + tweet.Text +
                            "\nTweet ID: " + tweet.ID + "\n");
                }); 
            }
```

##### v2.1 Example:

```c#
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Show &&
                      tweet.ID == "10520783556"
                select tweet;

            Console.WriteLine("\nRequested Tweet: \n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "User: " + tweet.User.Name +
                    "\nTweet: " + tweet.Text +
                    "\nTweet ID: " + tweet.ID + "\n");
            }
```

*Twitter API:* [statuses/show/:id](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/get-statuses-show-id)