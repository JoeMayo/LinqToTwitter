#### Listing Direct Message Events

Show direct message events information. Includes DMs to and from a user.

*Entity:* [DirectMessageEvents](../LINQ-to-Twitter-Entities/DirectMessageEvents-Entity.md)

*Type:* DirectMessageEventsType.List

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of items to return for a single page. | int | no |
| Cursor| Supports paging through results on List queries. | string | no |

##### v4.x Example:

```c#
            int count = 10; // intentionally set to a low number to demo paging
            string cursor = "";
            List<DMEvent> allDmEvents = new List<DMEvent>();

            // you don't have a valid cursor until after the first query
            DirectMessageEvents dmResponse =
                await
                    (from dm in twitterCtx.DirectMessageEvents
                     where dm.Type == DirectMessageEventsType.List &&
                           dm.Count == count
                     select dm)
                    .SingleOrDefaultAsync();

            allDmEvents.AddRange(dmResponse.Value.DMEvents);
            cursor = dmResponse.Value.NextCursor;

            while (!string.IsNullOrWhiteSpace(cursor))
            {
                dmResponse =
                    await
                        (from dm in twitterCtx.DirectMessageEvents
                         where dm.Type == DirectMessageEventsType.List &&
                               dm.Count == count &&
                               dm.Cursor == cursor
                         select dm)
                        .SingleOrDefaultAsync();

                allDmEvents.AddRange(dmResponse.Value.DMEvents);
                cursor = dmResponse.Value.NextCursor;
            }

            if (!allDmEvents.Any())
            {
                Console.WriteLine("No items returned");
                return;
            }

            Console.WriteLine($"Response Count: {allDmEvents.Count}");
            Console.WriteLine("Responses:");

            allDmEvents.ForEach(evt =>
            {
                MessageCreate msgCreate = evt.MessageCreate;

                if (evt != null && msgCreate != null)
                    Console.WriteLine(
                        "From ID: {0}\nTo ID:  {1}\nMessage Text: {2}",
                        msgCreate.SenderID ?? "None",
                        msgCreate.Target?.RecipientID ?? "None",
                        msgCreate.MessageData?.Text ?? "None");
            });
```

*Twitter API:* [direct_messages/events/list](https://developer.twitter.com/en/docs/direct-messages/sending-and-receiving/api-reference/list-events)