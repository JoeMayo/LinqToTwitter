#### Learn what Lists a User Subscribes To

Get lists user has subscribed to.

*Entity:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

*Type:* ListType.Subscriptions

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
                 where list.Type == ListType.Subscriptions &&
                       list.ScreenName == "Linq2Tweeter"
                 select list)
                .ToListAsync();

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "List Name: {0}, Description: {1}",
                        list.Name, list.Description));
```

##### v2.1 Example:

```c#
            var lists =
                from list in twitterCtx.List
                where list.Type == ListType.Subscriptions &&
                      list.ScreenName == "Linq2Tweeter" // user to get subscriptions for
                select list;

            foreach (var list in lists)
            {
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
            }
```

*Twitter API:* [lists/subscriptions](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-subscriptions)