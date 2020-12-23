#### Raw Commands

A raw query lets you manually add URL segments and parameters to communicate with any HTTP POST endpoint in the Twitter API.

##### Signature:

```c#
public async Task<string> ExecuteRawAsync(
    string queryString, Dictionary<string, string> parameters)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| queryString | URL segments | string | yes |
| parameters | Name/value pairs for post parameters | Dictionary<string, string> | no |

*Return Type:* string

##### Example:

```c#
            string status = 
                "Testing LINQ to Twitter Raw Interface - " + 
                DateTime.Now.ToString() + " #Linq2Twitter";
            var parameters = new Dictionary<string, string>
            {
                { "status", status }
            };

            string queryString = "/statuses/update.json";

            string result = 
                await twitterCtx.ExecuteRawAsync(
                    queryString, parameters);

            if (result != null)
                Console.WriteLine(
                    "\nResponse from update status: \n\n\t" + result);
```

*Twitter API:* [Any HTTP POST endpoint in the Twitter API](https://dev.twitter.com/docs/api/1.1)