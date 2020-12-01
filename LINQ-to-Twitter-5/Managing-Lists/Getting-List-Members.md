#### Getting List Members

Get a list of members of a list.

*Entity:* [[List|List Entity]]
*Type:* ListType.Members

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Token to help page through lists | long | no |
| IncludeEntities | Add entities to tweets (default: true) | bool | no |
| ListID | ID of list | ulong | yes |
| OwnerID | ID of user who owns list | ulong | only if Slug is provided |
| OwnerScreenName | Name of user to who owns list | string | only if Slug is provided |
| SkipStatus | Exclude status information | bool | yes |
| Slug | Short list name | string | yes |

Note: Either ListID or Slug is required. If you use Slug, you must also specify either OwnerID or OwnerScreenName.

##### v3.0 Example:

```c#
            var lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Members &&
                       list.OwnerScreenName == "Linq2Tweeter" &&
                       list.Slug == "linq" &&
                       list.SkipStatus == true
                 select list)
                .SingleOrDefaultAsync();

            if (lists != null && lists.Users != null)
                lists.Users.ForEach(user =>
                    Console.WriteLine("Member: " + user.Name));
```

##### v2.1 Example:

```c#
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.Members &&
                       list.OwnerScreenName == "Linq2Tweeter" &&
                       list.Slug == "linq" &&
                       list.SkipStatus == true
                 select list)
                .First();

            foreach (var user in lists.Users)
            {
                Console.WriteLine("Member: " + user.Name);
            }
```

*Twitter API:* [lists/members](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-members)