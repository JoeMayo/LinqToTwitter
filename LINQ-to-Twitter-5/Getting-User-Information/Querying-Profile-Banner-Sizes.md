#### Querying Profile Banner Sizes

Get list allowable sizes for profile banners.

*Entity:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

*Type:* UserType.BannerSizes

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| UserID | ID of user | ulong | only if ScreenName is empty |
| ScreenName | Screen name of user | string | only if UserID is empty |

##### v3.0 Example:

```c#
            var user =
                await
                (from usr in twitterCtx.User
                 where usr.Type == UserType.BannerSizes &&
                       usr.ScreenName == "JoeMayo"
                 select usr)
                .SingleOrDefaultAsync();

            if (user != null && user.BannerSizes != null)
                user.BannerSizes.ForEach(size =>
                    Console.WriteLine(
                        "Label: {0}, W: {1} H: {2}  {3}",
                        size.Label, size.Width, size.Height, size.Url));
```

##### v2.1 Example:

```c#
            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.BannerSizes &&
                       usr.ScreenName == "Linq2Tweeter"
                 select usr)
                .SingleOrDefault();

            user.BannerSizes.ForEach(size => 
                Console.WriteLine(
                    "Label: {0}, W: {1} H: {2}  {3}",
                    size.Label, size.Width, size.Height, size.Url));
```

*Twitter API:* [users/profile_banner](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/get-users-profile_banner)