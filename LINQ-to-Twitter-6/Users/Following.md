#### Following

Get a list of other users that a user is following.

*Entity:* TwitterUser|TwitterUser Entity

*Type:* UserType.Following

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | ID of user to retrieve following | string | yes |
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| MaxResults | Maximum number of tweets to return | int | no |
| PaginationToken | Provide this, when paging, to get the next or previous page of results | string | no |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

##### Example:
```c#
string userID = "15411837";

TwitterUserQuery? userResponse =
    await
    (from user in twitterCtx.TwitterUser
     where user.Type == UserType.Following &&
           user.ID == userID
     select user)
    .SingleOrDefaultAsync();

if (userResponse != null)
    userResponse.Users?.ForEach(user =>
        Console.WriteLine("ID: " + user.ID));
```

*Twitter API:* [users/:id/following](https://developer.twitter.com/en/docs/twitter-api/users/follows/api-reference/get-users-id-following)