#### Finding Owned Lists

Get lists that the authenticated user owns.

*Entity:* [[List|List Entity]]
*Type:* ListType.Ownerships

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Results per page | int | no |
| Cursor | Token for paging through list | long | no |
| ScreenName | Screen name of user | string | only if UserID is empty |
| UserID | ID of user | ulong | only if ScreenName is empty |

Note: Either UserID or ScreenName is required.

##### v3.0 Example:

```c#
            var lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Ownerships &&
                       list.ScreenName == "Linq2Tweeter"
                 select list)
                .ToListAsync();

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "ID: {0}  Slug: {1} Description: {2}",
                        list.ListIDResult, 
                        list.SlugResult, 
                        list.Description));
```

##### v2.1 Example:

```c#
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.Ownerships &&
                       list.ScreenName == "Linq2Tweeter"
                 select list)
                .ToList();

            foreach (var list in lists)
            {
                Console.WriteLine("ID: {0}  Slug: {1} Description: {2}",
                    list.ListIDResult, list.SlugResult, list.Description);
            }
```

*Twitter API:* [lists/ownerships](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-ownerships)