#### Updating Profile Information

Set various profile information items.

##### Signature:

```c#
public async Task<User> UpdateAccountProfileAsync(
    string name, string url, string location, string description, 
    bool skipStatus)
public async Task<User> UpdateAccountProfileAsync(
    string name, string url, string location, string description, 
    bool includeEntities, bool skipStatus)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| description | Bio information | string | no |
| includeEntities | Include Twitter entities (default: true) | bool | no |
| location | Location | string | no |
| name | Full name | string | no |
| skipStatus | Exclude status information | bool | no |
| url | Web address | string | no |

*Return Type:* [[User|User Entity]]

##### v3.0 Example:

```c#
            var user = await twitterCtx.UpdateAccountProfileAsync(
                "Joe Mayo",
                "https://github.com/JoeMayo/LinqToTwitter",
                "Las Vegas, NV",
                "Testing the Account Profile Update with LINQ to Twitter.",
                true,
                true);

            if (user != null)
                Console.WriteLine(
                    "Name: {0}\n {1}\nLocation: {2}\nDescription: {3}",
                    user.Name, user.Url, user.Location, user.Description); 
```

##### v2.1 Example:

```c#
            var user = twitterCtx.UpdateAccountProfile(
                "LINQ to Tweeter",
                "https://github.com/JoeMayo/LinqToTwitter",
                "Anywhere In The World",
                "Testing the LINQ to Twitter Account Profile Update.");

            Console.WriteLine(
                "Name: {0}\n {1}\nLocation: {2}\nDescription: {3}",
                user.Name, 
                user.Url, 
                user.Location, 
                user.Description);
```

*Twitter API:* [account/update_profile](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/post-account-update_profile)