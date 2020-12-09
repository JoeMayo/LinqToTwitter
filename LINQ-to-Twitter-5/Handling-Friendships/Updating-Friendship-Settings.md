#### Updating Friendship Settings

Update notification settings.

##### Signature:

```c#
public async Task<Friendship> UpdateFriendshipSettingsAsync(
    ulong userID, bool retweets, bool device)
public async Task<Friendship> UpdateFriendshipSettingsAsync(
    string screenName, bool retweets, bool device)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| device | Receive device notifications about followed user | bool | no |
| retweets | Receive retweets from followed user | bool | no |
| screenName | Screen name of user to follow | string | yes |
| userID | ID of user to follow | ulong | yes |

Note: userID or screenName must be provided.

*Return Type:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

##### v3.0 Example:

```c#
            Friendship friend = 
                await twitterCtx.UpdateFriendshipSettingsAsync(
                    "Linq2Tweeter", true, true);

            if (friend != null && friend.SourceRelationship != null)
                Console.WriteLine(
                    "Settings for {0} are: Can Retweet is {1} " +
                    "and Can Send Device Notifications is {2}",
                    friend.SourceRelationship.ScreenName,
                    friend.SourceRelationship.RetweetsWanted,
                    friend.SourceRelationship.NotificationsEnabled);
```

##### v2.1 Example:

```c#
Friendship friend = twitterCtx.UpdateFriendshipSettings("JoeMayo", /*retweets:*/ true,
                                                        /*device:*/ true);

Console.WriteLine("Settings for {0} are: Can Retweet is {1}" + 
    "Can Send Device Notifications is {2}",
    friend.SourceRelationship.ScreenName, 
    friend.SourceRelationship.RetweetsWanted, 
    friend.SourceRelationship.NotificationsEnabled);
```

*Twitter API:* [friendships/update](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/post-friendships-update)