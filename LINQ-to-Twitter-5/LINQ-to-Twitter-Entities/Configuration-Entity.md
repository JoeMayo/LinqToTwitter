#### Configuration Entity

The configuration entity contains properties about help configuration information. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| CharactersReservedPerMedia | Characters reserved per media | int | no |
| MaxMediaPerUpload | Max number of items that can be uploaded at one time | int | no |
| NonUserNamePaths | Twitter slugs that are not usernames | List of string | no |
| PhotoSizeLimit | Max photo size | int | no |
| PhotoSizes | Sizing allowances/behaviors for each type of photo | List of [[PhotoSize|PhotoSize Entity]] | no |
| ShortUrlLength | Length of a t.co short URL | int | no |
| ShortUrlLengthHttps | Length of an HTTPS t.co short URL | int | no |

*Twitter API:* not documented