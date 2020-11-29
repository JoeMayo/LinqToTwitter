#### Updating Account Settings

Update the user's account settings. All parameters are optional, but at least one must be passed.

##### Signature:

```c#
public async Task<Account> UpdateAccountSettingsAsync(
    int? trendLocationWoeid, bool? sleepTimeEnabled, int? 
    startSleepTime, int? endSleepTime, string timeZone, string lang)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| endSleepTime | Hour to end sleep time | int | no |
| lang | Language | string | no |
| sleepTimeEnabled | Enable sleep time | bool | no |
| startSleepTime | Hour to start sleep time | int | no |
| timeZone | Timezone to display | string | no |
| trendLocationWOEID | Yahoo! Where On Earth ID | string | no |

*Return Type:* [[Account|Account Entity]]

##### v3.0 Example:

```c#
            Account acct = 
                await twitterCtx.UpdateAccountSettingsAsync(
                    null, true, 20, 6, null, null);

            if (acct != null && 
                acct.Settings != null && 
                acct.Settings.SleepTime != null)
            {
                SleepTime sleep = acct.Settings.SleepTime;
                Console.WriteLine(
                    "Enabled: {0}, Start: {1}, End: {2}",
                    sleep.Enabled, sleep.StartHour, sleep.EndHour); 
            }
```

##### v2.1 Example:

```c#
Account acct = twitterCtx.UpdateAccountSettings(null, true, 20, 6, null, null);

SleepTime sleep = acct.Settings.SleepTime;
Console.WriteLine(
    "Enabled: {0}, Start: {1}, End: {2}",
    sleep.Enabled, sleep.StartHour, sleep.EndHour);
```

*Twitter API:* [account/settings](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/post-account-settings)