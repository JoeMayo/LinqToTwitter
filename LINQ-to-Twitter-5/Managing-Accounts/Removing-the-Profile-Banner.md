#### Removing the Profile Banner

Deletes the user's profile banner.

##### Signature:

```c#
public async Task<User> RemoveProfileBannerAsync()
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| None | - | - | - |

*Return Type:* [[User|User Entity]]

##### v3.0 Example:

```c#
            var user = await twitterCtx.RemoveProfileBannerAsync();

            if (user != null)
                Console.WriteLine("Profile Banner: " + user.ProfileBannerUrl ?? "None"); 
```

##### v2.1 Example:

```c#
            var user = twitterCtx.RemoveProfileBanner();
            Console.WriteLine("Profile Banner: " + user.ProfileBannerUrl ?? "None");
```

*Twitter API:* [account/remove_profile_banner](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/post-account-remove_profile_banner)