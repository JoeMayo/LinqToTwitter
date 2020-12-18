#### Updating Statuses

Tweet or reply to a tweet.

##### Signature:

```c#
public async Task<Status> ReplyAsync(
    ulong tweetID, string status)
public async Task<Status> ReplyAsync(
    ulong tweetID, string status, decimal latitude, 
    decimal longitude, bool displayCoordinates)
public async Task<Status> ReplyAsync(
    ulong tweetID, string status, decimal latitude, 
    decimal longitude, string placeID, bool trimUser)
public async Task<Status> ReplyAsync(
    ulong tweetID, string status, string placeID, 
    bool displayCoordinates, bool trimUser)
public async Task<Status> ReplyAsync(
    ulong tweetID, string status, decimal latitude, 
    decimal longitude, string placeID, 
    bool displayCoordinates, bool trimUser)

public async Task<Status> TweetAsync(
    string status)
public async Task<Status> TweetAsync(
    string status, decimal latitude, decimal longitude)
public async Task<Status> TweetAsync(
    string status, decimal latitude, 
    decimal longitude, bool displayCoordinates)
public async Task<Status> TweetAsync(
    string status, decimal latitude, decimal longitude, 
    string placeID, bool trimUser)
public async Task<Status> TweetAsync(
    string status, decimal latitude, decimal longitude, 
    string placeID, bool displayCoordinates, bool trimUser)
public async Task<Status> TweetAsync(
    string status, string placeID, 
    bool displayCoordinates, bool trimUser)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| displayCoordinates | Choose whether location info should appear in this specific tweet; defaults to false. | bool | no |
| tweetID | ID of user replied to. | ulong | no | 
| latitude | Latitude location coordinate. | decimal | no |
| longitude | Logitude location coordinate. | decimal | no | 
| placeID | ID of place (can be obtained via reverse geo lookup (Geo.Reverse in LINQ to Twitter). | string | no | 
| status | Text for new tweet. Must be 280 characters or less in length. | string | yes | 
| tweetMode | Set to TweetMode.Extended for 280 characters returned in Status.FullText. | TweetMode | no |

**Return Type:** [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

**Note:** When replying, remember to prefix the tweet with the _@UserName_ for the person you're replying to.

##### v 3.0 Example:

```c#
            Console.Write("Enter your status update: ");
            string status = Console.ReadLine();

            Console.WriteLine("\nStatus being sent: \n\n\"{0}\"", status);
            Console.Write("\nDo you want to update your status? (y or n): ");
            string confirm = Console.ReadLine();

            if (confirm.ToUpper() == "N")
            {
                Console.WriteLine("\nThis status is *not* being sent.");
            }
            else if (confirm.ToUpper() == "Y")
            {
                Console.WriteLine("\nPress any key to post tweet...\n");
                Console.ReadKey(true);

                var tweet = await twitterCtx.TweetAsync(status);

                if (tweet != null)
                    Console.WriteLine(
                        "Status returned: " +
                        "(" + tweet.StatusID + ")" +
                        tweet.User.Name + ", " +
                        tweet.Text + "\n");
            }
            else
            {
                Console.WriteLine("Not a valid entry.");
            }
```

##### v2.1 Example:

```c#
            var status =
                " Testing LINQ to Twitter update status on \n " +
                DateTime.Now.ToString() + " #linqtotwitter ";

            Console.WriteLine("\nStatus being sent: \n\n\"{0}\"", status);
            Console.WriteLine("\nPress any key to post tweet...\n");
            Console.ReadKey();

            var tweet = twitterCtx.UpdateStatus(status);

            Console.WriteLine(
                "Status returned: " +
                "(" + tweet.StatusID + ")" +
                "[" + tweet.User.ID + "]" +
                tweet.User.Name + ", " +
                tweet.Text + ", " +
                tweet.CreatedAt + "\n");
```

*Twitter API:* [statuses/update](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/post-statuses-update)