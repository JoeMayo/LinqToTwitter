#### Checking if a User is a List Member

Determine if a user is a member of a list.

*Entity:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

*Type:* ListType.IsMember

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeEntities | Add entities to tweets (default: true) | bool | no |
| ListID | ID of list | ulong | only is Slug is empty |
| OwnerID | ID of list owner | ulong | only if Slug is provided |
| OwnerScreenName | Name of list owner | string | only if Slug is provided |
| ScreenName | ScreenName of list owner | string | only if UserID is empty |
| SkipStatus | Exclude status information | bool | no |
| Slug | Short list name | string | only if ListID is empty |
| UserID | ID of user to check | ulong | only if ScreenName is empty |

Note: Either UserID or ScreenName is required. Either ListID or Slug is required. If you use Slug, you must also specify either OwnerID or OwnerScreenName. 

##### v3.0 Example:

Note: Twitter returns 404 Not Found, which is an exception in .NET, if user is not a member.

```c#
            try
            {
                var subscribedList =
                    await
                    (from list in twitterCtx.List
                     where list.Type == ListType.IsMember &&
                           list.ScreenName == "JoeMayo" &&
                           list.OwnerScreenName == "Linq2Tweeter" &&
                           list.Slug == "linq"
                     select list)
                    .SingleOrDefaultAsync();

                if (subscribedList != null && subscribedList.Users != null)
                {
                    // list will have only one user matching ID in query
                    var user = subscribedList.Users.First();

                    Console.WriteLine("User: {0} is a member of List: {1}",
                        user.Name, subscribedList.ListID); 
                }
            }
            // whenever user is not a member of the specified list, Twitter
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

Note: Twitter returns 404 Not Found, which is an exception in .NET, if user is not a member.

```c#
            try
            {
                var subscribedList =
                   (from list in twitterCtx.List
                    where list.Type == ListType.IsMember &&
                         list.ScreenName == "Linq2Tweeter" &&
                         list.OwnerScreenName == "JoeMayo" &&
                         list.Slug == "dotnettwittterdevs"
                    select list)
                    .FirstOrDefault();

                // list will have only one user matching ID in query
                var user = subscribedList.Users.First();

                Console.WriteLine("User: {0} is a member of List: {1}",
                    user.Name, subscribedList.ListID);
            }
            // whenever user is not a member of the specified list, Twitter
            // returns an HTTP 404 Not Found, response, which results in a
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

*Twitter API:* [lists/members/show](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-members-show)