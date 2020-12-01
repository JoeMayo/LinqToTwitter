#### Checking if a User Subscribes to a List

Determine if a user subscribes to a list.

*Entity:* [[List|List Entity]]
*Type:* ListType.IsSubscriber

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeEntities | Add entities to tweets (default: true) | bool | no |
| ListID | ID of list | ulong | only is Slug is empty |
| OwnerID | ID of list owner | ulong | only if Slug is provided |
| OwnerScreenName | Name of list owner | string | only if Slug is provided |
| ScreenName | ScreenName of user | string | only if UserID is empty |
| SkipStatus | Exclude status information | bool | no |
| Slug | Short list name | string | only if ListID is empty |
| UserID | ID of user | ulong | only if ScreenName is empty |

Note: Either UserID or ScreenName is required. Either ListID or Slug is required. If you use Slug, you must also specify either OwnerID or OwnerScreenName. 

##### v3.0 Example:

```c#
            try
            {
                var subscribedList =
                    await
                    (from list in twitterCtx.List
                     where list.Type == ListType.IsSubscriber &&
                           list.ScreenName == "JoeMayo" &&
                           list.Slug == "linq" &&
                           list.OwnerScreenName == "Linq2Tweeter"
                     select list)
                    .SingleOrDefaultAsync();

                if (subscribedList != null && subscribedList.Users != null)
                {
                    // list will have only one user matching ID in query
                    var user = subscribedList.Users.First();

                    Console.WriteLine("User: {0} is subscribed to List: {1}",
                        user.Name, subscribedList.ListID); 
                }
            }
            // whenever user is not subscribed to the specified list, Twitter
            // returns an HTTP 404, Not Found, response.  LINQ to Twitter 
            // intercepts the HTTP response and wraps it in a TwitterQueryException 
            // where you can read the error message from Twitter via the Message property.
            catch (TwitterQueryException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(
                        "HTTP Status Code: {0}. Twitter Error Code: {1}, Twitter Message: {2}",
                        ex.StatusCode.ToString(),
                        ex.ErrorCode,
                        ex.Message);
                }
                else
                {
                    throw ex;
                }
            }
```

##### v2.1 Example:

```c#
            try
            {
                var subscribedList =
                   (from list in twitterCtx.List
                    where list.Type == ListType.IsSubscribed &&
                          list.ScreenName == "Linq2Tweeter" &&
                          list.Slug == "dotnettwittterdevs" &&
                          list.OwnerScreenName == "JoeMayo"
                    select list)
                   .FirstOrDefault();

                // list will have only one user matching ID in query
                var user = subscribedList.Users.First();

                Console.WriteLine("User: {0} is subscribed to List: {1}",
                    user.Name, subscribedList.ListID);
            }
            // whenever user is not subscribed to the specified list, Twitter
            // returns an HTTP 404, Not Found, response, which results in a
            // .NET exception.  LINQ to Twitter intercepts the HTTP exception
            // and wraps it in a TwitterQueryResponse where you can read the
            // error message from Twitter via the Response property, shown below.
            catch (TwitterQueryException ex)
            {
                // TwitterQueryException will always reference the original
                // WebException, so the check is redundant but doesn't hurt
                var webEx = ex.InnerException as WebException;
                if (webEx == null) throw ex;

                // The response holds data from Twitter
                var webResponse = webEx.Response as HttpWebResponse;
                if (webResponse == null) throw ex;

                if (webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(
                        "HTTP Status Code: {0}. Response from Twitter: {1}",
                        webEx.Response.Headers["Status"],
                        ex.Response.Error);
                }
                else
                {
                    throw ex;
                }
            }
```

Note: Twitter returns HTTP 404 Not Found, which becomes a .NET exception, when user is not subscribed.

*Twitter API:* [lists/subscribers/show](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-subscribers-show)