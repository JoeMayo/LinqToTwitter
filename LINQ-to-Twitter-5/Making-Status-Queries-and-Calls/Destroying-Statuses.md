#### Destroying Statuses

Delete a tweet.

##### Signature:

```c#
public async Task<Status> DeleteTweetAsync(ulong tweetID)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| tweetID | ID of tweet to delete | ulong | yes |

*Return Type:* [[Status|Status Entity]]

##### v 3.0 Example:

```c#
            ulong tweetID = 280433519057068033;

            Status status = 
                await twitterCtx.DeleteTweetAsync(tweetID);

            if (status != null && status.User != null)
                Console.WriteLine(
                    "(" + status.StatusID + ")" +
                    "[" + status.User.UserID + "]" +
                    status.User.ScreenNameResponse + ", " +
                    status.Text + ", " +
                    status.CreatedAt);
```

##### v2.1 Example:

```c#
var message = twitterCtx.NewDirectMessage("16761255", "Direct Message Test - " 
                                          + DateTime.Now);

if (message != null)
{
    Console.WriteLine(
        "Recipient: {0}, Message: {1}, Date: {2}",
        message.RecipientScreenName,
        message.Text,
        message.CreatedAt);
}
```

*Twitter API:* [statuses/destroy/:id](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/post-statuses-destroy-id)