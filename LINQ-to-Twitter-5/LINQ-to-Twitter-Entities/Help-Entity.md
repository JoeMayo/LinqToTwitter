#### Help Entity

The help entity contains properties about help information. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

#### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Configuration | Populated for Help Configuration query | [[Configuration|Configuration Entity]] | no |
| Languages | List of languages, codes, and statuses | int | no |
| OK | True if help test succeeds | bool | no |
| Type | Help type | HelpType | no | 

*Twitter API:* not documented