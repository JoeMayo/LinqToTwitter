#### Deleting Multiple List Members

Remove up to 100 members from a list.

##### Signature:

```c#
public async Task<List> DeleteMemberRangeFromListAsync(
    ulong listID, string slug, List<ulong> userIDs, 
    ulong ownerID, string ownerScreenName)
public async Task<List> DeleteMemberRangeFromListAsync(
    ulong listID, string slug, List<string> screenNames,
    ulong ownerID, string ownerScreenName)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| listID | ID of list | ulong | yes |
| ownerID | ID of list owner | string | no |
| ownerScreenName | Name of list owner | string | no |
| screenName | List owner | string | no |
| slug | List name | string | yes |
| userID| ID of user | string | no |

Note: Either userID or screenName is required. Either listID or slug is required. If you use slug, you must also specify either ownerID or ownerScreenName.

*Return Type:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

##### v3.0 Example:

```c#
            string ownerScreenName = "Linq2Tweeter";
            var screenNames = new List<string> { "JoeMayo", "mp2kmag" };

            List list = 
                await twitterCtx.DeleteMemberRangeFromListAsync(
                    0, "testDemo", screenNames, 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
```

##### v2.1 Example:

```c#
List list = twitterCtx.DestroyAllFromList(null, "test", null, "JoeMayo,mp2kmag",
                                          null, "Linq2Tweeter");

Console.WriteLine("List Name: {0}, Description: {1}",
                  list.Name, list.Description);
```

*Twitter API:* [lists/members/destroy_all](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-members-destroy_all)