#### RateStatusLimit Entity

The rate status limit entity contains properties about rate status limits. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| HourlyLimit | Hourly limit | int | no |
| RemainingHits | Remaining hits | int | no |
| ResetTime | When limits reset | DateTime | no |
| ResetTimeInSeconds | When limits reset in second | int | no |

*Twitter API:* not documented