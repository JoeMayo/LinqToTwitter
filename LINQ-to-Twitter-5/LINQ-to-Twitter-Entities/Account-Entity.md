#### Account Entity

The account entity contains properties about accounts. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| EndSessionStatus | Response from request to end session | string | no |
| RateLimitStatus | Rate limit status | [[RateLimitStatus|RateLimitStatus Entity]] | no |
| Settings | Account settings | [[Settings|Settings Entity]] | no |
| Totals | Current totals | [[Totals|Totals Entity]] | no |
| Type | Type of account query | AccountType | no |
| User | User returned by VerifyCredentials Queries | [User] | no |

*Twitter API:* not documented