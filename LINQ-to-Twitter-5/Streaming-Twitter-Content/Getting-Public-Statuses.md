### Getting Public Statuses
Return all public statuses using the firehose.

*Type:* StreamingType.Firehose

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of tweets to go back to when reconnecting | int | no |
| Delimited | Tweets are delimited in the stream | string | no |
| StallWarnings | Whether stall warnings should be delivered | bool | no |

*Return Type:* JSON string.

*Twitter API:* [ statuses/firehose](https://dev.twitter.com/docs/api/1.1/get/statuses/firehose)