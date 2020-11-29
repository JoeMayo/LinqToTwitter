#### TZInfo Entity

The time zone info entity contains properties about time zone settings. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Name | Timezone name | string | no |
| TzInfoName | Rails/unix TZINFO name | string | no |
| UtcOffset | Second to subtract from UTC time | int | no |

*Twitter API:* not documented