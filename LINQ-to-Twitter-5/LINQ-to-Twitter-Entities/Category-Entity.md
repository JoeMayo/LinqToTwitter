#### Category Entity

The category entity contains properties about categories. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Categories | List of categories | List of [[Category|Category Entity]] | no |
| Name | Category name | string | no |
| Size | Number of users in category | int | no |
| Slug | Category description | string | no |
| Users | Users in category | List of [[User|User Entity]] | no |

*Twitter API:* not documented