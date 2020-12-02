#### Creating a Saved Search

Create a new saved search.

##### Signature:

```c#
public async Task<SavedSearch> CreateSavedSearchAsync(string query)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| query | Text of search query | string | yes |

*Return Type:* [[SavedSearch|SavedSearch Entity]]

##### v3.0 Example:

```c#
            SavedSearch savedSearch = 
                await twitterCtx.CreateSavedSearchAsync("linq");

            if (savedSearch != null)
                Console.WriteLine(
                    "ID: {0}, Search: {1}", 
                    savedSearch.IDResponse, savedSearch.Query);
```

##### v2.1 Example:

```c#
var savedSearch = twitterCtx.CreateSavedSearch("#csharp");

Console.WriteLine("ID: {0}, Search: {1}", savedSearch.IDString, savedSearch.Query);
```

*Twitter API:* [saved_searches/create](https://dev.twitter.com/docs/api/1.1/post/saved_searches/create)