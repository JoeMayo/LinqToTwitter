#### Updating a Profile Image

Sets user's profile image.

##### Signature:

```c#
public async Task<User> UpdateAccountImageAsync(
    byte[] image, string fileName, 
    string imageType, bool skipStatus)
public async Task<User> UpdateAccountImageAsync(
    byte[] image, string fileName, 
    string imageType, bool includeEntities, bool skipStatus)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| fileName | Image file name | string | no |
| image | Base64-encoded image | byte[] | yes |
| imageFilePath | Image file path | string | no |
| imageType | Image file type | string | no |
| includeEntities | Include Twitter entities (default: true) | bool | no |
| skipStatus | Exclude status information (default: false) | bool | no |

*Return Type:* [[User|User Entity]]

##### v3.0 Example:

```c#
            byte[] imageBytes = File.ReadAllBytes(@"..\..\Images\200xColor_2.png");

            var user = await twitterCtx.UpdateAccountImageAsync(
                imageBytes, "200xColor_2.png", "png", false);

            if (user != null)
                Console.WriteLine("User Image: " + user.ProfileImageUrl); 
```

##### v2.1 Example:

```c#
            var user = twitterCtx.UpdateAccountImage(@"..\..\images\200xColor_2.png");

            Console.WriteLine("User Image: " + user.ProfileImageUrl);
```

*Twitter API:* [account/update_profile_image](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/post-account-update_profile_image)
