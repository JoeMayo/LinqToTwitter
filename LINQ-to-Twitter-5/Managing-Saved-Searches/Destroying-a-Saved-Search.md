#### Destroying a Saved Search

Delete a saved search.

##### Signature:

```c#
public async Task<SavedSearch> DestroySavedSearchAsync(ulong id)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| id | Saved search ID | ulong | yes |

*Return Type:* [[SavedSearch|SavedSearch Entity]]

##### v3.0 Example:

```c#
            ulong savedSearchID = 0;

            SavedSearch savedSearch = 
                await twitterCtx.DestroySavedSearchAsync(savedSearchID);

            if (savedSearch != null)
                Console.WriteLine(
                    "ID: {0}, Search: {1}", 
                    savedSearch.ID, savedSearch.Name);
```

##### v2.1 Example:

```c#
            var savedSearch = twitterCtx.DestroySavedSearch(101352438);

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.ID, savedSearch.Name);
```

*Twitter API:* [saved_searches/destroy/:id](https://dev.twitter.com/docs/api/1.1/post/saved_searches/destroy/%3Aid)