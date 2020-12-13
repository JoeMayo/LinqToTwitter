#### Getting Info on a List

Get details on a list.

*Entity:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

*Type:* ListType.Show

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ListID | ID of list | ulong | only if Slug is empty |
| OwnerID | ID of user who owns list | ulong | only if Slug is provided |
| OwnerScreenName | Name of user to who owns list | string | only is Slug is provided |
| Slug | Short list name | string | only if ListID is empty |

Note: Either ListID or Slug is required. If you use Slug, you must also specify either OwnerID or OwnerScreenName.

##### v3.0 Example:

```c#
            var requestedList =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Show &&
                       list.OwnerScreenName == "Linq2Tweeter" &&
                       list.Slug == "linq"
                 select list)
                .SingleOrDefaultAsync();

            if (requestedList != null)
                Console.WriteLine(
                    "List Name: {0}, Description: {1}, # Users: {2}",
                    requestedList.Name, 
                    requestedList.Description, 
                    requestedList.Users.Count());
```

##### v2.1 Example:

```c#
var requestedList =
     (from list in twitterCtx.List
     where list.Type == ListType.Show &&
           list.OwnerScreenName == "JoeMayo" && // user who owns list
           list.Slug == "dotnettwittterdevs" // list name
     select list)
     .FirstOrDefault();

Console.WriteLine("List Name: {0}, Description: {1}, # Users: {2}",
    requestedList.Name, requestedList.Description, requestedList.Users.Count());
```

*Twitter API:* [lists/show](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-show)