#### Verifying Credentials

Check to see that a user's credentials are okay.

*Entity:* [[Account|Account Entity]]
*Type:* AccountType.VerifyCredentials

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| IncludeEmail | Includes email address in User entity for VerifyCredentials query (requires Twitter approval) | bool | no |
| IncludeEntities | Include Twitter entities | bool | no |
| SkipStatus | Exclude status information | bool | no |

##### v3.0 Example:

```c#
            try
            {
                var verifyResponse =
                    await
                        (from acct in twitterCtx.Account
                         where acct.Type == AccountType.VerifyCredentials
                         select acct)
                        .SingleOrDefaultAsync();

                if (verifyResponse != null && verifyResponse.User != null)
                {
                    User user = verifyResponse.User;

                    Console.WriteLine(
                        "Credentials are good for {0}.",
                        user.ScreenNameResponse); 
                }
            }
            catch (TwitterQueryException tqe)
            {
                Console.WriteLine(tqe.Message);
            }
```

##### v2.1 Example:

```c#
            var accounts =
                from acct in twitterCtx.Account
                where acct.Type == AccountType.VerifyCredentials
                select acct;

            try
            {
                Account account = accounts.SingleOrDefault();
                User user = account.User;
                Status tweet = user.Status ?? new Status();
                Console.WriteLine(
                    "User ID: {0}\nScreen Name: {1}\nTweet: {2}\n Tweet ID: {3}",
                     user.Identifier.ID,
                     user.Identifier.ScreenName,
                     tweet.Text, 
                     tweet.StatusID);

                Console.WriteLine("Account credentials are verified.");
            }
            catch (WebException wex)
            {
                Console.WriteLine(
                    "Twitter did not recognize the credentials. Response from Twitter: " 
                    + wex.Message);
            }
```

Note: Whenever credentials are okay, Twitter returns the user's Account object. Otherwise, Twitter returns an HTTP 401, which results in an Exception in .NET.

*Twitter API:* [account/verify_credentials](https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/get-account-verify_credentials)