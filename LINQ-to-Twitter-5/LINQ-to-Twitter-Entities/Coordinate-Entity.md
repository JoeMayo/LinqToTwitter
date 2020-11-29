#### Coordinate Entity

The coordinate entity contains properties about coordinate information. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Latitude | Coordinate latitude | double | no |
| Longitude | Coordinate longitude | double | no |
| Type | Coordinate type | string | no |
| IsLocationAvailable | Indicates whether tweet has a valid coordinate | bool | no |

*Twitter API:* [coordinates](https://dev.twitter.com/docs/platform-objects/tweets#obj-coordinates)
