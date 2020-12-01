#### Learn what Lists a User is a Member Of

Get lists a user is a member of.

*Entity:* [[List|List Entity]]
*Type:* ListType.Memberships

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Token for paging through list | long | no |
| FilterToOwnedLists | Only return lists the user owns | bool | no |
| ScreenName | Screen name of user | string | only if UserID is empty |
| UserID | ID of user | ulong | only if ScreenName is empty |

Note: Either UserID or ScreenName is required.

##### v3.0 Example:

```c#
            var lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Memberships &&
                       list.ScreenName == "JoeMayo" // user to get memberships for
                 select list)
                .ToListAsync();

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "List Name: {0}, Description: {1}",
                        list.Name, list.Description));
```

##### v2.0 Example:

```c#
            var lists =
                from list in twitterCtx.List
                where list.Type == ListType.Memberships &&
                      list.ScreenName == "JoeMayo" // user to get memberships for
                select list;

            foreach (var list in lists)
            {
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
            }
```

*Twitter API:* [lists/memberships](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-memberships)