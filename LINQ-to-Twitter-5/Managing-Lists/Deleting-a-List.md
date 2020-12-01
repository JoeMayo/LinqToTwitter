#### Deleting a List

Delete a list.

##### Signature:

```c#
public async Task<List> DeleteListAsync(
    ulong listID, string slug, ulong ownerID, string ownerScreenName)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| listID | ID of list | ulong | only if slug is empty |
| ownerID | ID of user who owns list | ulong | only if slug is provided |
| ownerScreenName | Name of user to who owns list | string | only is slug is provided |
| slug | Short list name | string | only if listID is empty |

Note: Either listID or slug is required. If you use slug, you must also specify either ownerID or ownerScreenName.

*Return Type:* [[List|List Entity]]

##### v3.0 Example:

```c#
            ulong listID = 0;

            List list = 
                await twitterCtx.DeleteListAsync(
                    listID, "testDemo", 0, "Linq2Tweeter");

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
```

##### v2.1 Example:

```c#
List list = twitterCtx.DeleteList(null, "test-5", null, "Linq2Tweeter");

Console.WriteLine("List Name: {0}, Description: {1}", list.Name, list.Description);
```

*Twitter API:* [lists/destroy](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-destroy)