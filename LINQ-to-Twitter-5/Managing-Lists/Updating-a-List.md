#### Updating a List

Update a list.

##### Signature:

```c#
public async Task<List> UpdateListAsync(
    ulong listID, string slug, string name, ulong ownerID, 
    string ownerScreenName, string mode, string description)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| description | New description of list | string | no |
| listID | ID of list | ulong | only if slug is empty |
| mode | New mode of list: public or private | string | no |
| name | New name of list | string | no |
| ownerID | ID of list owner | ulong | only if ownerScreenName is empty and slug is not empty |
| ownerScreenName | Name of list owner | string | only if ownerID is empty and slug is not empty |
| slug | Short list name | string | only if listID is empty, must have ownerID or ownerScreenName |

Note: Either listID or slug is required.  If you use slug, you must also specify either ownerID or ownerScreenName.

*Return Type:* [[List|List Entity]]

##### v3.0 Example:

```c#
            string ownerScreenName = "Linq2Tweeter";
            ulong listID = 0;

            List list = 
                await twitterCtx.UpdateListAsync(
                    listID, "testDemo", "Test List", 0, 
                    ownerScreenName, "public", "This is a test2");

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
```

##### v2.1 Example:

```c#
List list = twitterCtx.UpdateList(null, "test", null, "Linq2Tweeter", "public", 
                                  "This is a test2");

Console.WriteLine("List Name: {0}, Description: {1}", list.Name, list.Description);
```

*Twitter API:* [lists/update](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-update)