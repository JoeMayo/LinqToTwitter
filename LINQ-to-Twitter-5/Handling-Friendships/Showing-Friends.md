#### Showing Friends

Gets User entities for friends.

*Entity:* [Friendship](../LINQ-to-Twitter-Entities/Friendship-Entity.md)

*Type:* FriendshipType.FriendsList

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Max number of friends to return in one request | int | no |
| Cursor | Supports paging through multiple result sets | long | no |
| IncludeUserEntities | False to leave entities out (default: true) | bool | no |
| ScreenName | Screen name of user to get friends for | string | only if userID is empty |
| SkipStatus | Don't include User's latest tweet | bool | no |
| UserID | ID of user to get friends for | string | only if screenName is empty |

##### v3.0 Example:

```c#
            Friendship friendship;
            long cursor = -1;
            do
            {
                friendship =
                    await
                    (from friend in twitterCtx.Friendship
                     where friend.Type == FriendshipType.FriendsList &&
                           friend.ScreenName == "JoeMayo" &&
                           friend.Cursor == cursor
                     select friend)
                    .SingleOrDefaultAsync();

                if (friendship != null && 
                    friendship.Users != null && 
                    friendship.CursorMovement != null)
                {
                    cursor = friendship.CursorMovement.Next;

                    friendship.Users.ForEach(friend =>
                        Console.WriteLine(
                            "ID: {0} Name: {1}",
                            friend.UserIDResponse, friend.ScreenNameResponse)); 
                }

            } while (cursor != 0);
```

##### v2.1 Example:

```c#
            Friendship friendship;
            string cursor = "-1";
            do
            {
                friendship =
                    (from friend in twitterCtx.Friendship
                     where friend.Type == FriendshipType.FriendsList &&
                           friend.ScreenName == "JoeMayo" &&
                           friend.Cursor == cursor                         
                     select friend)
                    .SingleOrDefault();

                cursor = friendship.CursorMovement.Next;       

                friendship.Users.ForEach(friend =>
                    Console.WriteLine(
                        "ID: {0} Name: {1}",
                        friend.Identifier.UserID, friend.Identifier.ScreenName)); 

            } while (cursor != "0");
```

*Twitter API:* [friends/list](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-friends-list)