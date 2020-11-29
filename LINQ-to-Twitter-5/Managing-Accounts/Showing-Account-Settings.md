#### Showing Account Settings

Show account settings.

*Entity:* [[Account|Account Entity]]
*Type:* AccountType.Settings

##### Parameters/Filters:

None

##### v3.0 Example:

```c#
            var settingsResponse =
                await
                (from acct in twitterCtx.Account
                 where acct.Type == AccountType.Settings
                 select acct)
                .SingleOrDefaultAsync();

            if (settingsResponse?.Settings?.SleepTime != null)
            {
                var settings = settingsResponse.Settings;

                Console.WriteLine(
                    "Trend Location: {0}\nGeo Enabled: {1}\nSleep Enabled: {2}",
                    settings.TrendLocation.Name,
                    settings.GeoEnabled,
                    settings.SleepTime.Enabled); 
            }
```

##### v2.1 Example:

```c#
            var accountSettings =
                (from acct in twitterCtx.Account
                 where acct.Type == AccountType.Settings
                 select acct.Settings)
                .SingleOrDefault();

            Console.WriteLine(
                "Trend Location: {0}\nGeo Enabled: {1}\nSleep Enabled: {2}",
                accountSettings.TrendLocation.Name,
                accountSettings.GeoEnabled,
                accountSettings.SleepTime.Enabled);
```

*Twitter API:* [account/settings](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/get-account-settings)