#### Friendship Entity

The friendship entity contains properties about friendships. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Cursor | Helps in paging results | string | no |
| FollowingUser | ID or ScreenName of user to test for following | string | no |
| Friend | Information about friend | [[User|User Entity]] | no |
| IDInfo | List of IDs returned by Incoming and Outgoing queries | string | no |
| IsFriend | FollowingUser is following SubjectUser returned from Twitter | bool | no |
| Relationships | List of relationships from Lookup query | List of [[Relationship|Relationship Entity]] | no |
| ScreenName | Comma-seperated list of screen names for Lookup query | string | no |
| SourceRelationship | Relationship details returned from Twitter for the source | [[Relationship|Relationship Entity]] | no |
| SourceScreenName | ScreenName of source user | string | yes |
| SourceUserID | ID of of source user | string | no |
| SubjectUser | ID or ScreenName of subject user | string | no |
| TargetRelationship | Relationship details returned from Twitter for the target | string | no |
| TargetScreenName | ScreenName of target user | string | yes |
| TargetUserID | ID of target user | string | no |
| Type | Type of friendship | FriendshipType | no |
| Users | Users for either FollowersList  FriendsList orqueries | List of [[User|User Entity]]| no |

*Twitter API:* not documented