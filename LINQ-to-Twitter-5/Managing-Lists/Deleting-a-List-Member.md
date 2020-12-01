#### Deleting a List Member

Remove a user from a list

##### Signature:

```c#
public async Task<List> DeleteMemberFromListAsync(
    ulong userID, string screenName, ulong listID, 
    string slug, ulong ownerID, string ownerScreenName)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| listID | ID of list | ulong | only is slug is empty |
| ownerID | ID of list owner | ulong | only if slug is provided |
| ownerScreenName | Name of list owner | string | only if slug is provided |
| screenName | ScreenName of list owner | string | only if userID is empty |
| slug | Short list name | string | only if listID is empty |
| userID | User ID of list owner | ulong | only if screenName is empty |

Note: Either userID or screenName is required. Either listID or slug is required. If you use slug, you must also specify either ownerID or ownerScreenName. 

*Return Type:* [[List|List Entity]]

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
List list = twitterCtx.DeleteMemberFromList(null, "Linq2Tweeter", null, "test", null,
                                            "Linq2Tweeter");

Console.WriteLine("List Name: {0}, Description: {1}", list.Name, list.Description);
```

*Twitter API:* [lists/members/destroy](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-members-destroy)