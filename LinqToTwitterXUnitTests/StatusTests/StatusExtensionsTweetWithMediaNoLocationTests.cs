using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.StatusTests
{
    public class StatusExtensionsTweetWithMediaNoLocationTests
    {
        readonly string expectedUploadUrl = "https://api.twitter.com/1.1/statuses/update_with_media.json";

        readonly Mock<TwitterContext> twitterCtx;
        readonly Mock<ITwitterExecute> twitterExecute;
        readonly Mock<IRequestProcessor<Status>> statusReqProc;
         
        string status = "test";
        bool possiblySensitive = true;

        List<Media> mediaItems = new List<Media> 
        { 
            new Media
            {
                Data = new byte[] { 0xFF },
                FileName = "AnImage.jpg",
                ContentType = MediaContentType.Jpeg
            }
        };

        public StatusExtensionsTweetWithMediaNoLocationTests()
        {
            statusReqProc = new Mock<IRequestProcessor<Status>>();
            statusReqProc.Setup(reqProc => reqProc.ProcessResults(It.IsAny<string>()))
                         .Returns(new List<Status> { new Status { Text = "Test" } });

            twitterExecute = new Mock<ITwitterExecute>();
            twitterExecute.Setup(exec => exec.PostMedia(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<List<Media>>(), It.IsAny<IRequestProcessor<Status>>()))
                          .Returns(SingleStatusResponse);

            twitterCtx = new Mock<TwitterContext>(this.twitterExecute.Object);
            twitterCtx.Setup(ctx => ctx.CreateRequestProcessor<Status>())
                      .Returns(this.statusReqProc.Object);
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Throws_On_Null_Status()
        {
            this.status = null;

            ArgumentNullException ex =
                Assert.Throws<ArgumentNullException>(() =>
                {
                    StatusExtensions.TweetWithMedia(
                        twitterCtx.Object, status, possiblySensitive, mediaItems);
                });

            Assert.Equal("status", ex.ParamName);
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Calls_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "PostMedia was not called only one time.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Passes_Properly_Formatted_Url_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.Is<string>(url => url == expectedUploadUrl),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia didn't pass properly formatted URL.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Passes_Status_Via_Parameter_Dictionary_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.Is<IDictionary<string, string>>(parms => parms["status"] == status),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia didn't pass status properly.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Passes_PossiblySensitive_Via_Parameter_Dictionary_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.Is<IDictionary<string, string>>(parms => parms["possibly_sensitive"] == possiblySensitive.ToString()),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia didn't pass possiblySensitive parameter properly.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_DoesNot_Pass_False_PossiblySensitive_Via_Parameter_Dictionary_To_PostMedia()
        {
            possiblySensitive = false;

            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.Is<IDictionary<string, string>>(parms => parms["possibly_sensitive"] == null),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia should not have passed possiblySensitive parameter.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_DoesNot_Pass_PlaceID_Via_Parameter_Dictionary_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.Is<IDictionary<string, string>>(parms => parms["place_id"] == null),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia shouldn't pass placeID parameter.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_DoesNot_Pass_DisplayCoordinates_Via_Parameter_Dictionary_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.Is<IDictionary<string, string>>(parms => parms["display_coordinates"] == null),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia shouldn't pass displayCoordinates parameter.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_DoesNot_Pass_InReplyToStatusID_Via_Parameter_Dictionary_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.Is<IDictionary<string, string>>(parms => !parms.ContainsKey("in_reply_To_status_id")),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia shouldn't pass inReplyToStatusID parameter.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_DoesNot_Pass_Lat_And_Long_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.Is<IDictionary<string, string>>( parms => parms["lat"] == null && parms["long"] == null),
                It.IsAny<List<Media>>(),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "Lat and long should be null.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Passes_Media_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.Is<List<Media>>(media => object.ReferenceEquals(media, this.mediaItems)),
                It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "TweetWithMedia didn't pass mediaItems properly.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Requires_NonNull_MediaItems()
        {
            mediaItems = null;

            ArgumentNullException ex =
                Assert.Throws<ArgumentNullException>(() => StatusExtensions.TweetWithMedia(
                    twitterCtx.Object, status, possiblySensitive, mediaItems));

            Assert.Equal("mediaItems", ex.ParamName);
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Requires_At_Least_One_Media()
        {
            mediaItems = new List<Media>();

            ArgumentException ex =
                Assert.Throws<ArgumentException>(() => StatusExtensions.TweetWithMedia(
                    twitterCtx.Object, status, possiblySensitive, mediaItems));

            Assert.Equal("mediaItems", ex.ParamName);
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Passes_IRequestProcessor_To_PostMedia()
        {
            StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            twitterExecute.Verify(exec => exec.PostMedia(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<List<Media>>(),
                It.Is<IRequestProcessor<Status>>( reqProc => reqProc != null && reqProc is IRequestProcessor<Status>)),
                Times.Once(),
                "TweetWithMedia didn't pass reqProc properly.");
        }

        [Fact]
        public void TweetWithMedia_Without_Location_Params_Returns_Status()
        {
            Status tweet = StatusExtensions.TweetWithMedia(
                twitterCtx.Object, status, possiblySensitive, mediaItems);

            Assert.True(tweet.Text.StartsWith("RT @scottgu: I just blogged about"));
        }

        [Fact]
        public void TweetWithMedia_Accepts_Internationalized_Characters()
        {
            string statusMsg = "My new car £10,555. http://test.com #CodePlex #testing";

            Status tweet = StatusExtensions.TweetWithMedia(
                twitterCtx.Object, statusMsg, possiblySensitive, mediaItems);

            Assert.True(tweet.Text.StartsWith("RT @scottgu: I just blogged about"));
        }

        const string SingleStatusResponse = @"{
      ""retweeted"":false,
      ""in_reply_To_screen_name"":null,
      ""possibly_sensitive"":false,
      ""retweeted_status"":{
         ""retweeted"":false,
         ""in_reply_To_screen_name"":null,
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""coordinates"":null,
         ""place"":null,
         ""user"":{
            ""id"":41754227,
            ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""url"":""http:\/\/weblogs.asp.net\/scottgu"",
            ""created_at"":""Fri May 22 04:39:35 +0000 2009"",
            ""followers_count"":57222,
            ""default_profile"":true,
            ""profile_background_color"":""C0DEED"",
            ""lang"":""en"",
            ""utc_offset"":-28800,
            ""name"":""Scott Guthrie"",
            ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""location"":""Redmond, WA"",
            ""profile_link_color"":""0084B4"",
            ""listed_count"":4390,
            ""verified"":false,
            ""protected"":false,
            ""profile_use_background_image"":true,
            ""is_translator"":false,
            ""following"":false,
            ""description"":""I live in Seattle and build a few products for Microsoft"",
            ""profile_text_color"":""333333"",
            ""statuses_count"":3054,
            ""screen_name"":""scottgu"",
            ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""time_zone"":""Pacific Time (US & Canada)"",
            ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""friends_count"":86,
            ""default_profile_image"":false,
            ""contributors_enabled"":false,
            ""profile_sidebar_border_color"":""C0DEED"",
            ""id_str"":""41754227"",
            ""geo_enabled"":false,
            ""favourites_count"":44,
            ""profile_background_tile"":false,
            ""notifications"":false,
            ""show_all_inline_media"":false,
            ""profile_sidebar_fill_color"":""DDEEF6"",
            ""follow_request_sent"":false
         },
         ""retweet_count"":393,
         ""id_str"":""184793217231880192"",
         ""in_reply_To_user_id"":null,
         ""favorited"":false,
         ""in_reply_To_status_id_str"":null,
         ""in_reply_To_status_id"":null,
         ""source"":""web"",
         ""created_at"":""Wed Mar 28 00:05:10 +0000 2012"",
         ""in_reply_To_user_id_str"":null,
         ""truncated"":false,
         ""id"":184793217231880192,
         ""geo"":null,
         ""text"":""I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
      },
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":15411837,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""followers_count"":1102,
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
         ""following"":true,
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
         ""favourites_count"":44,
         ""profile_background_tile"":false,
         ""notifications"":true,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":393,
      ""id_str"":""184835136037191681"",
      ""in_reply_To_user_id"":null,
      ""favorited"":false,
      ""in_reply_To_status_id_str"":null,
      ""in_reply_To_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Wed Mar 28 02:51:45 +0000 2012"",
      ""in_reply_To_user_id_str"":null,
      ""truncated"":false,
      ""id"":184835136037191681,
      ""geo"":null,
      ""text"":""RT @scottgu: I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
   }";
    }
}
