#### Sending Direct Message Events

Send a direct message.

##### Signature:

```C#
public async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, CancellationToken cancelToken = default(CancellationToken))
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| recipientID | ID of user to send to. | ulong | yes |
| text | Direct message contents. | string | yes |

*Return Type:* [DirectMessageEvents](../LINQ-to-Twitter-Entities/DirectMessageEvents-Entity.md)

##### v 4.x Example:

```C#
            const ulong Linq2TwitrID = 15411837;

            DirectMessageEvents message = 
                await twitterCtx.NewDirectMessageEventAsync(
                    Linq2TwitrID, 
                    "DM from @JoeMayo to @Linq2Twitr of $MSFT & $TSLA with #TwitterAPI #chatbot " +
                    "at http://bit.ly/2xSJWJk and http://amzn.to/2gD09X6 on " + DateTime.Now + "!'");

            DMEvent dmEvent = message?.Value?.DMEvent;
            if (dmEvent != null)
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}, Date: {2}",
                    dmEvent.MessageCreate.Target.RecipientID,
                    dmEvent.MessageCreate.MessageData.Text,
                    dmEvent.CreatedTimestamp);
```

*Twitter API:* [direct_messages/events/new](https://developer.twitter.com/en/docs/direct-messages/sending-and-receiving/api-reference/new-event)