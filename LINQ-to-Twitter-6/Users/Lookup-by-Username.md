Search for one or more users by username.

*Entity:* TwitterUser|TwitterUser Entity

*Type:* UserType.UsernameLookup

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| Usernames | Up to 100 comma-separated usernames to search for | string | yes |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

##### Example:

```c#
static async Task LookupByUsernameAsync(TwitterContext twitterCtx)
{
	TwitterUserQuery? userResponse =
		await
		(from user in twitterCtx.TwitterUser
		 where user.Type == UserType.UsernameLookup &&
			   user.Usernames == "JoeMayo,Linq2Twitr" &&
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

*Twitter API:* [users/by/username](https://developer.twitter.com/en/docs/twitter-api/users/lookup/api-reference/get-users-by-username-username)