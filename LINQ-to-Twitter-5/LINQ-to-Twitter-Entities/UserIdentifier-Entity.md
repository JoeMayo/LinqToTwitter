#### UserIdentifier Entity

The user identifier entity contains properties about a specific user. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| ID | User's Twitter ID | string | no |
| ScreenName | User's ScreenName | string | no |
| UserID | User ID for disambiguating when ID is screen name | string | no |

*Twitter API:* not documented