using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.SearchTests
{
    [TestClass]
    public class Search2RequestProcessorTests
    {
        public Search2RequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new Search2RequestProcessor<Search>();

            Expression<Func<Search2, bool>> expression =
                search =>
                    search.Type == SearchType.RecentSearch &&
                    search.EndTime == "2020-08-30T12:59:59Z" &&
                    search.Expansions == "attachments.poll_ids,author_id" &&
                    search.MaxResults == 10 &&
                    search.MediaFields == "height,width" &&
                    search.NextToken == "abc" &&
                    search.PlaceFields == "country" &&
                    search.PollFields == "duration_minutes,end_datetime" &&
                    search.Query == "LINQ to Twitter" &&
                    search.SinceID == "123" &&
                    search.StartTime == "2020-08-30T12:59:59Z" &&
                    search.TweetFields == "author_id,created_at" &&
                    search.UntilID == "525" &&
                    search.UserFields == "created_at,verified";

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.Type), ((int)SearchType.RecentSearch).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.EndTime), "2020-08-30T12:59:59Z")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.Expansions), "attachments.poll_ids,author_id")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.MaxResults), "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.MediaFields), "height,width")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.NextToken), "abc")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(Search2.PlaceFields), "country")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(Search2.PollFields), "duration_minutes,end_datetime")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>(nameof(Search2.Query), "LINQ to Twitter")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.SinceID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.StartTime), "2020-08-30T12:59:59Z")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.UntilID), "525")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.UserFields), "created_at,verified")));
        }
    }
}
