#### Showing Friendship Details

Gets details on the relationship between two users.

*Entity:* [[Friendship|Friendship Entity]]
*Type:* FriendshipType.Show

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| SourceUserID | ID of source user | ulong | only if SourceScreenName is empty |
| SourceScreenName | Screen name of source user | string | only if SourceID is empty |
| TargetUserID | ID of target user | ulong | only if TargetScreenName is empty |
| TargetScreenName | Screen name of target user | string | only if TargetID is empty |

##### v3.0 Example:

```c#
            var friendship =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.Show &&
                       friend.SourceScreenName == "Linq2Tweeter" &&
                       friend.TargetScreenName == "JoeMayo"
                 select friend)
                .SingleOrDefaultAsync();

            if (friendship != null && 
                friendship.SourceRelationship != null && 
                friendship.TargetRelationship != null)
            {
                Console.WriteLine(
                        "\nJoeMayo follows LinqToTweeter: " +
                        friendship.SourceRelationship.FollowedBy +
                        "\nLinqToTweeter follows JoeMayo: " +
                        friendship.TargetRelationship.FollowedBy); 
            }
```

##### v2.1 Example:

```c#
            var friendship =
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.Show &&
                       friend.SourceScreenName == "Linq2Tweeter" &&
                       friend.TargetScreenName == "JoeMayo"
                 select friend)
                .First();

            Console.WriteLine(
                "\nJoeMayo follows LinqToTweeter: " + 
                friendship.SourceRelationship.FollowedBy + 
                "\nLinqToTweeter follows JoeMayo: " +
                friendship.TargetRelationship.FollowedBy);
```

*Twitter API:* [friendships/show](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-friendships-show)