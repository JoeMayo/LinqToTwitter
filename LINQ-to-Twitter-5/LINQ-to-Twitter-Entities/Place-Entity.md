#### Place Entity

The place entity contains properties about places. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Attributes | Metadata related to place | Dictionary<string, string> | no |
| BoundingBox | Coordinates that enclose place | [[Geometry|Geometry Entity]] | no |
| Country | Name of country | string | no |
| CountryCode | Country code abbreviation | string | no |
| FullName | Full name of place | string | no |
| Name | Place name | string | no |
| ID | Contributor ID | ulong | no |
| PlaceType | Type of place - neighborhood, city, country, etc. | string | no |
| Url | URL to get more details on place | string | no |

*Twitter API:* [place](https://dev.twitter.com/docs/platform-objects/places)