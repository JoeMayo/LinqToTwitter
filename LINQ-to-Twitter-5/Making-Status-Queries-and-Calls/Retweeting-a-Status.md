#### Retweeting a Status

Retweets a tweet.

##### Signature:

```c#
public async Task<Status> RetweetAsync(ulong tweetID)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| tweetID | ID of user replied to. | ulong | yes |

*Return Type:* [[Status|Status Entity]]

##### Example:

```c#
            ulong tweetID = 401033367283453953;

            var retweet = await twitterCtx.RetweetAsync(tweetID);

            if (retweet != null && 
                retweet.RetweetedStatus != null && 
                retweet.RetweetedStatus.User != null)
            {
                Console.WriteLine("Retweeted Tweet: ");
                Console.WriteLine(
                    "\nUser: " + retweet.RetweetedStatus.User.ScreenNameResponse +
                    "\nTweet: " + retweet.RetweetedStatus.Text +
                    "\nTweet ID: " + retweet.RetweetedStatus.ID + "\n"); 
            }
```

*Twitter API:* [statuses/retweet/:id](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/post-statuses-retweet-id)