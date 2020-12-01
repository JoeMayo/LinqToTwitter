#### Subscribing a User to a List

Create a subscription for a user on a list

##### Signature:

```c#
public async Task<List> SubscribeToListAsync(
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

*Return Type:* [[List|List Entity]]

##### v3.0 Example:

```c#
            string ownerScreenName = "Linq2Tweeter";

            List list = 
                await twitterCtx.SubscribeToListAsync(
                    0, "testDemo", 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
```

##### v2.1 Example:

```c#
List list = twitterCtx.SubscribeToList(null, "test", null,  "Linq2Tweeter");

Console.WriteLine("List Name: {0}, Description: {1}", list.Name, list.Description);
```

*Twitter API:* [lists/subscribers/create](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-subscribers-create)