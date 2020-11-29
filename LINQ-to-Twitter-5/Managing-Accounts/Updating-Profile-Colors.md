#### Updating Profile Colors

Set color scheme for user's profile.

##### Signature:

```c#
    public async Task<User> UpdateAccountColorsAsync(
        string background, string text, string link, string sidebarFill, 
        string sidebarBorder, bool skipStatus)
    public async Task<User> UpdateAccountColorsAsync(
        string background, string text, string link, string sidebarFill, 
        string sidebarBorder, bool includeEntities, bool skipStatus)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| background | Background color | string | no |
| link | Link color | string | no |
| includeEntities | Add entities to response (default: true) | bool | no |
| sidebarBorder | Sidebar border color | string | no |
| sidebarFill | Sidebar fill color | string | no |
| skipStatus | Exclude status information | bool | no |
| text | Text color | string | no |

Note: Colors are specified as hex RGB - "#rrggbb".  At least one parameter must be specified.

*Return Type:* [[User|User Entity]]

##### v3.0 Example:

```c#
            var user = await twitterCtx.UpdateAccountColorsAsync(
                background: null, text: "#000000", link: "#0000ff", 
                sidebarFill: "#e0ff92", sidebarBorder: "#87bc44", 
                includeEntities: true, skipStatus: true);

            if (user != null)
            {
                Console.WriteLine("\nAccount Colors:\n");

                Console.WriteLine("Background:     " + user.ProfileBackgroundColor);
                Console.WriteLine("Text:           " + user.ProfileTextColor);
                Console.WriteLine("Link:           " + user.ProfileLinkColor);
                Console.WriteLine("Sidebar Fill:   " + user.ProfileSidebarFillColor);
                Console.WriteLine("Sidebar Border: " + user.ProfileSidebarBorderColor); 
            }
```

##### v2.1 Example:

```c#
var user = twitterCtx.UpdateAccountColors("9ae4e8", "#000000", "#0000ff",
                                          "#e0ff92", "#87bc44");

Console.WriteLine("\nAccount Colors:\n");

Console.WriteLine("Background:     " + user.ProfileBackgroundColor);
Console.WriteLine("Text:           " + user.ProfileTextColor);
Console.WriteLine("Link:           " + user.ProfileLinkColor);
Console.WriteLine("Sidebar Fill:   " + user.ProfileSidebarFillColor);
Console.WriteLine("Sidebar Border: " + user.ProfileSidebarBorderColor);
```

*Twitter API:* [account/update_profile_colors](https://dev.twitter.com/docs/api/1.1/post/account/update_profile_colors)