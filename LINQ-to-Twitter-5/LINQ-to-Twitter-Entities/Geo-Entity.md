#### Geo Entity

The geo entity contains properties about geographical information. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Accuracy | How accurate results should be | string | yes |
| Attribute | Name/value pair separated by "=" | string | yes |
| ContainedWithin | Place ID to restrict search to | string | yes |
| Granularity | Size of place | string | yes |
| ID | Place ID |string | no |
| IP | IP address to find nearby places | string | yes |
| Latitude | Coordinate latitude | double | no |
| Longitude | Coordinate longitude | double | no |
| MaxResults | Number of places to return | int | yes |
| PlaceName | Name of place for similar places query | string | no |
| Places | Results showing places matching query | List of [[Place|Place Entity]] | no |
| Query | Search text to help find place | string | yes |
| Token | Place token from similar places used to create place | string | yes (CreatePlace) / no (SimilarPlaces) |
| Type | Type of Geo query | GeoType | no |

*Twitter API:* not documented
