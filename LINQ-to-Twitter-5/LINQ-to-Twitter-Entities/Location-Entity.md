#### Location Entity

The location entity contains properties about locations. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Country | Location country | string | no |
| CountryCode | Country code | string | no |
| Name | Name of location | string | no |
| ParentID | Parent location relative to current location | string | no |
| PlaceTypeName | Type of location | string | no |
| PlaceTypeNameCode | Code for PlaceTypeName | int | no |
| Url | Yahoo location URL | string | no |
| WoeID | Yahoo Where On Earth ID | string | no |

*Twitter API:* not documented