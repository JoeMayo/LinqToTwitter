#### TweetPoll Object

Represents a Poll.

##### Properties:

| Name | Purpose | Type |
|------|---------|------|
| ID | ID for the poll | string |
| Options | Poll options | List&lt;[TweetPollOption]()&gt; |
| DurationMinutes | Number of minutes to run poll | int |
| EndDatetime | Ending date and time | DateTime |
| VotingStatus | Whether poll is open or closed | string |

*Twitter API:* [Poll Object](https://developer.twitter.com/en/docs/twitter-api/data-dictionary/object-model/poll)