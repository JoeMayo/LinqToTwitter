#### Media Entity

The Media entity contains information about a media upload to Twitter and is the return value from UploadMediaAsync. It helps identify the uploaded media for subsequent attachment to a tweet via TweetAsync.

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Image | Physical details on the uploaded image | [[MediaImage|MediaImage Entity]] | no |
| MediaID | Twitter identifier for the image | ulong | no |
| Size | Size in bytes of the image | int | no |
| VideoInfo | More info for "video" media types in Extended Entities | VideoInfo | no |

*Twitter API:* [see example response from media/upload](https://dev.twitter.com/rest/reference/post/media/upload)