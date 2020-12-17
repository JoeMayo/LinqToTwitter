#### Getting Trend Locations

Get trends for a specified place.

*Entity:* [Trends](../LINQ-to-Twitter-Entities/Trend-Entity.md)

*Type:* TrendType.Place

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| WoeID | Yahoo! Where on Earth ID | int | yes |

##### Example:

```c#
            var trends =
                await
                (from trend in twitterCtx.Trends
                 where trend.Type == TrendType.Place &&
                       trend.WoeID == 2486982
                 select trend)
                .ToListAsync();

            if (trends != null && 
                trends.Any() && 
                trends.First().Locations != null)
            {
                Console.WriteLine(
                    "Location: {0}\n",
                    trends.First().Locations.First().Name);

                trends.ForEach(trnd =>
                    Console.WriteLine(
                        "Name: {0}, Date: {1}, Query: {2}\nSearch {3}",
                        trnd.Name, trnd.CreatedAt, trnd.Query, trnd.SearchUrl)); 
            }
```

*Twitter API:* [trends/place](https://developer.twitter.com/en/docs/trends/trends-for-location/api-reference/get-trends-place)