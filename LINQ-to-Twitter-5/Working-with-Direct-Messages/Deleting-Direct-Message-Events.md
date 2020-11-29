#### Deleting Direct Message Events

Learning how to delete a direct message event.

*Requires Authentication:* Yes
*Rate Limited:* Yes

##### Signature:

```c#
async Task DeleteDirectMessageEventAsync(
    ulong directMessageID, CancellationToken cancelToken = default(CancellationToken));
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| directMessageID | ID of direct message to delete. | ulong | yes |
| cancelToken | Async cancellation token. | bool | no |

*Return Type:* None

##### v4.x+ Example:

```c#
            Console.Write("Which DM would you like to delete? (please enter DM ID): ");
            string dmInput = Console.ReadLine();

            ulong.TryParse(dmInput, out ulong dmID);

            try
            {
                await twitterCtx.DeleteDirectMessageEventAsync(dmID);
                Console.WriteLine("\nDM Deleted");
            }
            catch (TwitterQueryException tqEx)
            {
                Console.WriteLine($"\nProblem deleting DM: ({tqEx.ErrorCode}) - {tqEx.ReasonPhrase}");
            }
```

*Twitter API:* [direct_messages/events/destroy](https://developer.twitter.com/en/docs/direct-messages/sending-and-receiving/api-reference/delete-message-event)