#### Retrieving All Lists

Get a list of all lists.

*Entity:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

*Type:* ListType.List

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ScreenName | Screen name of user to return lists for | string | only if UserID is empty |
| UserID | ID of user to return lists for | ulong | only if ScreenName is empty |
| Reverse | Causes Twitter to return lists belonging to the authenticated user first | bool | no |

Note: Either UserID or ScreenName is required.

##### v3.0 Example:

```c#
            string screenName = "Linq2Tweeter";

            var lists =
                await
                    (from list in twitterCtx.List
                     where list.Type == ListType.List &&
                           list.ScreenName == screenName
                     select list)
                    .ToListAsync();

            if (lists != null)
                lists.ForEach(list => Console.WriteLine("Slug: " + list.SlugResult));
```

##### v2.1 Example:

```c#
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.Lists &&
                       list.ScreenName == "Linq2Tweeter"
                 select list)
                .ToList();

            foreach (var list in lists)
            {
                Console.WriteLine("ID: {0}  Slug: {1} Description: {2}",
                    list.ListIDResult, list.SlugResult, list.Description);
            }
```

*Twitter API:* [lists/list](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-list)