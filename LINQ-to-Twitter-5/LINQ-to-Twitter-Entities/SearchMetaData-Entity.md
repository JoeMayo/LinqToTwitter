#### SearchMetaData Entity

Additional information associated with search results. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| CompletedIn | Processing time for search | decimal | no |
| MaxID | Return tweets earlier than or equal to this ID | ulong | no |
| NextResults| Url to next search page | string | no |
| Query | Original query | string | no |
| RefreshUrl | Query string to refresh this search | string | no |
| SinceID | Return tweets later than this ID | ulong | no |

*Twitter API:* Not Documented