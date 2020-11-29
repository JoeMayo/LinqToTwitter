#### Showing Direct Message Events

Show direct message events information.

*Entity:* [[DirectMessageEvents|DirectMessageEvents Entity]]
*Type:* DirectMessageEventsType.Show

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | Direct message event ID | ulong | yes |

##### v4.x Example:

```c#
            DirectMessageEvents dmResponse =
                await
                    (from dm in twitterCtx.DirectMessageEvents
                     where dm.Type == DirectMessageEventsType.Show &&
                           dm.ID == 917929712638246916
                     select dm)
                    .SingleOrDefaultAsync();

            MessageCreate msgCreate = dmResponse?.Value?.DMEvent?.MessageCreate;

            if (dmResponse != null && msgCreate != null)
                Console.WriteLine(
                    "From ID: {0}\nTo ID:  {1}\nMessage Text: {2}",
                    msgCreate.SenderID ?? "None",
                    msgCreate.Target.RecipientID ?? "None",
                    msgCreate.MessageData.Text ?? "None");
```

*Twitter API:* [direct_messages/events/show](https://developer.twitter.com/en/docs/direct-messages/sending-and-receiving/api-reference/get-event)