#### Settings Entity

The settings entity contains properties about account settings. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| AlwaysUseHttps | Whether to always use HTTPS | bool | no |
| DiscoverableByEmail | Whether user is discoverable by email | bool | no |
| GeoEnabled | Whether Geo Tracking is on | bool | no |
| Language | User's UI language | string | no |
| SleepTime | Times not to notify | [[SleepTime|SleepTime Entity]] | no |
| TimeZone | User's timezone | [[TZInfo|TZInfo Entity]] | no |
| TrendLocation | Location to display trends for | [[Location|Location Entity]] | no |

*Twitter API:* not documented