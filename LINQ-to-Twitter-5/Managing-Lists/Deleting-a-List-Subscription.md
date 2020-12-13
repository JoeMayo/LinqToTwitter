#### Deleting a List Subscription

Remove a user's subscription to a list.

##### Signature:

```c#
public async Task<List> UnsubscribeFromListAsync(
    ulong listID, string slug, ulong ownerID, string ownerScreenName)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| listID | ID of list | ulong | only is slug is empty |
| ownerID | ID of list owner | ulong | only if slug is provided |
| ownerScreenName | Name of list owner | string | only if slug is provided |
| slug | Short list name | string | only if listID is empty |

Note: Either listID or slug is required. If you use slug, you must also specify either ownerID or ownerScreenName. 

*Return Type:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

##### v3.0 Example:

```c#
            string ownerScreenName = "Linq2Tweeter";

            List list = 
                await twitterCtx.DeleteMemberFromListAsync(
                    0, "Linq2Tweeter", 0, "testDemo", 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
```

##### v2.1 Example:

```c#
List list = twitterCtx.UnsubscribeFromList(null, "test", null, "Linq2Tweeter");

Console.WriteLine("List Name: {0}, Description: {1}", list.Name, list.Description);
```

*Twitter API:* [lists/subscribers/destroy](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-subscribers-destroy)