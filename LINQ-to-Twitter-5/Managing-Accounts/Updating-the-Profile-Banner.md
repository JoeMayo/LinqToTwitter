#### Updating the Profile Banner

Update the user's profile banner.

##### Signature:

```c#
public async Task<User> UpdateProfileBannerAsync(
    byte[] banner, CancellationToken cancelToken = default(CancellationToken))
public async Task<User> UpdateProfileBannerAsync(
    byte[] banner, int width, int height, int offsetLeft, int offsetTop, 
    CancellationToken cancelToken = default(CancellationToken))
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| banner | Banner image bytes | byte[] | yes |
| width | Width of banner | int | no |
| height | Height of banner | int | no |
| offsetLeft | Pixels from left to begin cropping image | int | no |
| offsetTop | Pixels from top to begin cropping image | int | no |
| cancelToken| Allows async cancellation | CancellationToken | no |

*Return Type:* [[User|User Entity]]

##### v3.0 Example:

```c#
            byte[] fileBytes = File.ReadAllBytes(@"..\..\images\WP_000003.jpg");

            var user = 
                await twitterCtx.UpdateProfileBannerAsync(
                    fileBytes, 1252, 626, 0, 0);

            if (user != null)
                Console.WriteLine("User Image: " + user.ProfileBannerUrl); 
```

##### v2.1 Example:

```c#
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\WP_000003.jpg");
            var user = twitterCtx.UpdateProfileBanner(fileBytes, "WP_000003.jpg", "jpg", 1252, 626, 0, 0);

            Console.WriteLine("User Image: " + user.ProfileBannerUrl);
```

*Twitter API:* [account/update_profile_banner](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/post-account-update_profile_banner)