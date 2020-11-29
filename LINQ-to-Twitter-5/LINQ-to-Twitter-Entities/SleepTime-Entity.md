#### SleepTime Entity

The sleep time entity contains properties about sleep time settings. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Enabled | Whether sleep time is turned on | bool | no |
| EndHour | Resume sending notifications at this time | int | no |
| StartHour | Stop sending notifications at this time | int | no |

*Twitter API:* not documented