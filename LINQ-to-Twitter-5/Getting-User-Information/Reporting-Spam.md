#### Sending a Spam Notification

Report a user for spam and optionally block them

##### Signature:

```c#
public async Task<User> ReportSpamAsync(
    string screenName, bool performBlock = false, CancellationToken cancelToken = default(CancellationToken));
public async Task<User> ReportSpamAsync(
    ulong userID, bool performBlock = false, CancellationToken cancelToken = default(CancellationToken));
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| performBlock | Blocks the user | bool | no |
| screenName | Screen name of user to report | string | only if userID is empty |
| userID | ID of user to report | string | only if screenName is empty |

Note: `userID` or `screenName` must be provided.

*Return Type:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

##### Example:

```c#
static async Task ReportSpammerAsync(TwitterContext twitterCtx)
{
    const string SpammerScreenName = "realDonaldTrump";

    User spammer = await twitterCtx.ReportSpamAsync(SpammerScreenName);

    Console.WriteLine("You just reported {0} as a spammer.", spammer.ScreenNameResponse);
}
```

*Twitter API:* [users/report_spam](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/post-users-report_spam)