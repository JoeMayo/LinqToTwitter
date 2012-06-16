using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.RelatedResultsTests
{
    public class RelatedResultsRequestProcessorTests
    {
        public RelatedResultsRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void RelatedResultsRequestProcessor_Works_With_Json_Data()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults>();

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(relResultsProc);
        }

        [Fact]
        public void GetParameters_Parses_Parameters()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults>();
            Expression<Func<RelatedResults, bool>> expression =
                res =>
                    res.Type == RelatedResultsType.Show &&
                    res.StatusID == 123ul;
            LambdaExpression lambdaExpression = expression;

            Dictionary<string, string> queryParams = relResultsProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)RelatedResultsType.Show).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("StatusID", "123")));
        }

        [Fact]
        public void BuildUrl_Creates_Show_Url()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults> { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)RelatedResultsType.Show).ToString() },
                    { "StatusID", "123" }
                };
            string expectedUrl = "https://api.twitter.com/1/related_results/show/123.json";

            Request req = relResultsProc.BuildUrl(parameters);

            Assert.Equal(expectedUrl, req.FullUrl);
        }

        [Fact]
        public void ProcessResults_Parses_RelatedResults()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults> { BaseUrl = "https://api.twitter.com/1/" };

            List<RelatedResults> results = relResultsProc.ProcessResults(ShowResultsResponse);

            Assert.NotNull(results);
            var result = results.First();
            Assert.NotNull(result.ResultAnnotations);
            var attribs = result.ResultAnnotations.Attributes;
            Assert.NotNull(attribs);
            Assert.True(attribs.ContainsKey("ConversationRole"));
            Assert.Equal("Fork", attribs["ConversationRole"]);
            Assert.Equal(1.0, result.Score);
            Assert.Equal("Tweet", result.Kind);
            Assert.NotNull(result.ValueAnnotations);
            Assert.Empty(result.ValueAnnotations.Attributes);
            Assert.False(result.Retweeted);
            Assert.Equal("swhitley", result.InReplyToScreenName);
            var contributors = result.Contributors;
            Assert.NotNull(contributors);
            Assert.Empty(contributors);
            Assert.NotNull(result.Coordinates);
            Assert.Equal(0, result.Coordinates.Latitude);
            Assert.NotNull(result.Place);
            Assert.Null(result.Place.FullName);
            var user = result.User;
            Assert.NotNull(user);
            Assert.Equal("Joe Mayo", user.Name);
            Assert.Equal(0, result.RetweetCount);
            Assert.Equal("195999832157073409", result.IDString);
            Assert.Equal(8739282ul, result.InReplyToUserID);
            Assert.False(result.Favorited);
            Assert.Equal("195992821411495936", result.InReplyToStatusIDString);
            Assert.Equal(195992821411495936ul, result.InReplyToStatusID);
            Assert.Equal("web", result.Source);
            Assert.Equal(new DateTime(2012, 4, 27, 22, 16, 16), result.CreatedAt);
            Assert.Equal("8739282", result.InReplyToUserIDString);
            Assert.False(result.Truncated);
            Assert.Equal(8739282ul, result.InReplyToUserID);
            Assert.NotNull(result.Geo);
            Assert.Null(result.Geo.Accuracy);
            Assert.NotNull(result.Text);
            Assert.True(result.Text.StartsWith("@swhitley If building"));
        }

        [Fact]
        public void ProcessResults_Sets_Input_Params()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)RelatedResultsType.Show).ToString() },
                { "StatusID", "123" }
            };
            relResultsProc.BuildUrl(parameters);

            List<RelatedResults> results = relResultsProc.ProcessResults(ShowResultsResponse);

            var result = results.First();

            Assert.Equal(RelatedResultsType.Show, result.Type);
            Assert.Equal(123ul, result.StatusID);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new RelatedResultsRequestProcessor<RelatedResults> { BaseUrl = "https://api.twitter.com/1/" };

            var results = reqProc.ProcessResults(string.Empty);

            Assert.Equal(0, results.Count);
        }

        const string ShowResultsResponse = @"[
   {
      ""annotations"":{
         ""FromUser"":""swhitley""
      },
      ""results"":[
         {
            ""annotations"":{
               ""ConversationRole"":""Fork""
            },
            ""score"":1.0,
            ""kind"":""Tweet"",
            ""value"":{
               ""annotations"":null,
               ""retweeted"":false,
               ""in_reply_to_screen_name"":""swhitley"",
               ""contributors"":null,
               ""coordinates"":null,
               ""place"":null,
               ""user"":{
                  ""id"":15411837,
                  ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
                  ""url"":""http:\/\/www.mayosoftware.com"",
                  ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
                  ""followers_count"":1101,
                  ""default_profile"":false,
                  ""profile_background_color"":""0099B9"",
                  ""lang"":""en"",
                  ""utc_offset"":-25200,
                  ""name"":""Joe Mayo"",
                  ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
                  ""location"":""Denver, CO"",
                  ""profile_link_color"":""0099B9"",
                  ""listed_count"":112,
                  ""verified"":false,
                  ""protected"":false,
                  ""profile_use_background_image"":true,
                  ""is_translator"":false,
                  ""following"":false,
                  ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
                  ""profile_text_color"":""3C3940"",
                  ""statuses_count"":1906,
                  ""screen_name"":""JoeMayo"",
                  ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
                  ""time_zone"":""Mountain Time (US & Canada)"",
                  ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
                  ""friends_count"":211,
                  ""default_profile_image"":false,
                  ""contributors_enabled"":false,
                  ""profile_sidebar_border_color"":""5ED4DC"",
                  ""id_str"":""15411837"",
                  ""geo_enabled"":true,
                  ""favourites_count"":41,
                  ""profile_background_tile"":false,
                  ""notifications"":false,
                  ""show_all_inline_media"":false,
                  ""profile_sidebar_fill_color"":""95E8EC"",
                  ""follow_request_sent"":false
               },
               ""retweet_count"":0,
               ""id_str"":""195999832157073409"",
               ""in_reply_to_user_id"":8739282,
               ""favorited"":false,
               ""in_reply_to_status_id_str"":""195992821411495936"",
               ""in_reply_to_status_id"":195992821411495936,
               ""source"":""web"",
               ""created_at"":""Fri Apr 27 22:16:16 +0000 2012"",
               ""in_reply_to_user_id_str"":""8739282"",
               ""truncated"":false,
               ""id"":195999832157073409,
               ""geo"":null,
               ""text"":""@swhitley If building Web apps for external customers, they would be shooting themselves in the foot by not being able to test""
            }
         },
         {
            ""annotations"":{
               ""ConversationRole"":""Fork""
            },
            ""score"":1.0,
            ""kind"":""Tweet"",
            ""value"":{
               ""annotations"":null,
               ""retweeted"":false,
               ""in_reply_to_screen_name"":""swhitley"",
               ""contributors"":null,
               ""coordinates"":null,
               ""place"":null,
               ""user"":{
                  ""id"":23502490,
                  ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/125779302\/SeanWork_normal.jpg"",
                  ""url"":null,
                  ""created_at"":""Mon Mar 09 21:14:07 +0000 2009"",
                  ""followers_count"":45,
                  ""default_profile"":true,
                  ""profile_background_color"":""C0DEED"",
                  ""lang"":""en"",
                  ""utc_offset"":-21600,
                  ""name"":""Sean Tovson"",
                  ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
                  ""location"":"""",
                  ""profile_link_color"":""0084B4"",
                  ""listed_count"":2,
                  ""verified"":false,
                  ""protected"":false,
                  ""profile_use_background_image"":true,
                  ""is_translator"":false,
                  ""following"":false,
                  ""description"":"""",
                  ""profile_text_color"":""333333"",
                  ""statuses_count"":73,
                  ""screen_name"":""TravelDeveloper"",
                  ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/125779302\/SeanWork_normal.jpg"",
                  ""time_zone"":""Central Time (US & Canada)"",
                  ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
                  ""friends_count"":34,
                  ""default_profile_image"":false,
                  ""contributors_enabled"":false,
                  ""profile_sidebar_border_color"":""C0DEED"",
                  ""id_str"":""23502490"",
                  ""geo_enabled"":false,
                  ""favourites_count"":2,
                  ""profile_background_tile"":false,
                  ""notifications"":false,
                  ""show_all_inline_media"":false,
                  ""profile_sidebar_fill_color"":""DDEEF6"",
                  ""follow_request_sent"":false
               },
               ""retweet_count"":0,
               ""id_str"":""195993342566342657"",
               ""in_reply_to_user_id"":8739282,
               ""favorited"":false,
               ""in_reply_to_status_id_str"":""195992821411495936"",
               ""in_reply_to_status_id"":195992821411495936,
               ""source"":""web"",
               ""created_at"":""Fri Apr 27 21:50:28 +0000 2012"",
               ""in_reply_to_user_id_str"":""8739282"",
               ""truncated"":false,
               ""id"":195993342566342657,
               ""geo"":null,
               ""text"":""@swhitley Sounds like you work for misinformed Nazis""
            }
         }
      ],
      ""score"":1.0,
      ""groupName"":""TweetsWithConversation"",
      ""resultType"":""Tweet""
   }
]";
    }
}
