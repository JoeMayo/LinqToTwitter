#### Creating a List

Create a new list.

##### Signature:

```c#
public async Task<List> CreateListAsync(
    string listName, string mode, string description)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| description | Description of list | string | no |
| listName | Name of list | string | yes |
| mode | Public or private | string | no |

*Return Type:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

##### v3.0 Example:

```c#
            List list = 
                await twitterCtx.CreateListAsync(
                    "testDemo", "public", "This is a test");

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
```

##### v2.1 Example:

```c#
List list = twitterCtx.CreateList("test", "public", "This is a test");

Console.WriteLine("List Name: {0}, Description: {1}", list.Name, list.Description);
```

*Twitter API:* [lists/create](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/post-lists-create)