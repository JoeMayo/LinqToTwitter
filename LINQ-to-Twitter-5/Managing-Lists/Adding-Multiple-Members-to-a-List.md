#### Adding Multiple Members to a List

Add multiple users to a list at the same time.

##### Signature:

```c#
public async Task<List> AddMemberRangeToListAsync(
    ulong listID, string slug, ulong ownerID, 
    string ownerScreenName, List<string> screenNames)
public async Task<List> AddMemberRangeToListAsync(
    ulong listID, string slug, ulong ownerID, 
    string ownerScreenName, List<ulong> userIDs)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| listID | ID of list | ulong| only is slug is empty |
| ownerID | ID of user who owns list | ulong| only if slug is provided |
| ownerScreenName | Name of user to who owns list | string | only if slug is provided |
| screenNames | Screen names to add | List<string> | only if userID is empty |
| slug | Short list name | string | only if listID is empty |
| userIDs | User IDs to add | List<ulong> | only if screenName is empty |

Note: Either userIDs or screenNames is required. Either listID or slug is required. If you use slug, you must also specify either ownerID or ownerScreenName. Maximum users per request is 100; requiring multiple invocations if you need to add more than 100 users.

*Return Type:* [[List|List Entity]]

##### v3.0 Example:

```c#
            string ownerScreenName = "Linq2Tweeter";
            var screenNames = new List<string>
            {
                "JoeMayo",
                "Linq2Tweeter"
            };

            List list = 
                await twitterCtx.AddMemberRangeToListAsync(
                    0, "testDemo", 0, ownerScreenName, screenNames);

            if (list != null && list.Users != null)
                list.Users.ForEach(user => Console.WriteLine(user.Name));
```

##### v2.1 Example:

```c#
var screenNames = new List<string>
{
    "JoeMayo",
    "Linq2Tweeter"
};

List list = twitterCtx.AddMemberRangeToList(null, "linq", null, "Linq2Tweeter",
                                            screenNames);

foreach (var user in list.Users)
{
      Console.WriteLine(user.Name);
}
```

*Twitter API:* [lists/members/create_all](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-members-create_all)