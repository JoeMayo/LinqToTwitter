#### Trend Entity

The trend entity contains properties about trends. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| AsOf | Time of request | DateTime | no |
| Date | Start date | DateTime | no |
| Events | Trend events | string | no |
| ExcludeHashtags | Exclude all trends with hashtags | bool | no |
| Latitude | Latitude | string | no |
| Location | Location where trend is occurring | [[Location|Location Entity]] | no |
| Locations | Lists locations of trending topics | List of [[Location|Location Entity]] | no |
| Longitude | Longitude | string | no |
| Name | Name of trend topic | string | no |
| PromotedContent | Flag indicating this is a promoted trend | string | no |
| Query | Twitter search query on topic | string | no |
| SearchUrl | Search URL returned from Local Trends | string | no |
| TrendDate | Date/hour of trend | DateTime | no |
| WeoID | Yahoo Where On Earth ID | int | no |

*Twitter API:* not documented