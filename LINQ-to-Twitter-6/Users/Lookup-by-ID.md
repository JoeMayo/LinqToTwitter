Search for one or more users by ID.

*Entity:* TwitterUser|TwitterUser Entity

*Type:* UserType.IdLookup

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| Ids | Up to 100 comma-separated IDs to search for | string | yes |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

##### Example:

```c#
static async Task LookupByUserIDAsync(TwitterContext twitterCtx)
{
	TwitterUserQuery? userResponse =
		await
		(from user in twitterCtx.TwitterUser
		 where user.Type == UserType.IdLookup &&
			   user.Ids == "15411837,16761255" &&
			   user.Expansions == ExpansionField.AllUserFields &&
			   user.TweetFields == TweetField.AllFieldsExceptPermissioned &&
			   user.UserFields == UserField.AllFields
		 select user)
		.SingleOrDefaultAsync();

	if (userResponse != null)
		userResponse.Users?.ForEach(user =>
			Console.WriteLine("Name: " + user.Username));
}
```

*Twitter API:* [users](https://developer.twitter.com/en/docs/twitter-api/users/lookup/api-reference/get-users)