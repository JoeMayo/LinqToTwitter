using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using LinqToTwitter;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.StatusTests
{
    public class StatusExtensionsReplyWithMediaAndLocationTests
    {
        const string ExpectedUploadUrl = "https://upload.twitter.com/1/statuses/update_with_media.xml";
        const string FrenchLocale = "FR-fr";

        readonly Mock<TwitterContext> twitterCtx;
        readonly Mock<ITwitterExecute> twitterExecute;
        readonly Mock<IRequestProcessor<Status>> statusReqProc;

        string status = "test";
        bool possiblySensitive = true;
        decimal latitude = 37.78215m;
        decimal longitude = -122.40060m;
        string placeID = "place";
        bool displayCoordinates = true;
        ulong inReplyToStatusID = 23030327348932ul;

        List<Media> mediaItems = new List<Media> 
        { 
            new Media
            {
                Data = new byte[] { 0xFF },
                FileName = "AnImage.jpg",
                ContentType = MediaContentType.JPEG
            }
        };

        const string TestStatusQueryResponse = @"<statuses type=""array"">
    <status>
      <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
      <id>6118906745</id>
      <text>ah,vou lá comer</text>
      <source>web</source>
      <truncated>false</truncated>
      <in_reply_to_status_id></in_reply_to_status_id>
      <in_reply_to_user_id></in_reply_to_user_id>
      <favorited>false</favorited>
      <in_reply_to_screen_name></in_reply_to_screen_name>
      <user>
        <id>77880019</id>
        <name>caah </name>
        <screen_name>caahbuss</screen_name>
        <location></location>
        <description></description>
        <profile_image_url>http://a1.twimg.com/profile_images/440024240/d_normal.JPG</profile_image_url>
        <url></url>
        <protected>false</protected>
        <followers_count>48</followers_count>
        <profile_background_color>131516</profile_background_color>
        <profile_text_color>333333</profile_text_color>
        <profile_link_color>009999</profile_link_color>
        <profile_sidebar_fill_color>efefef</profile_sidebar_fill_color>
        <profile_sidebar_border_color>eeeeee</profile_sidebar_border_color>
        <friends_count>47</friends_count>
        <created_at>Mon Sep 28 00:47:48 +0000 2009</created_at>
        <favourites_count>0</favourites_count>
        <utc_offset></utc_offset>
        <time_zone></time_zone>
        <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme14/bg.gif</profile_background_image_url>
        <profile_background_tile>true</profile_background_tile>
        <statuses_count>211</statuses_count>
        <notifications>false</notifications>
        <geo_enabled>false</geo_enabled>
        <verified>false</verified>
        <following>false</following>
      </user>
      <geo />
    </status>
  </statuses>";

        public StatusExtensionsReplyWithMediaAndLocationTests()
        {
            statusReqProc = new Mock<IRequestProcessor<Status>>();
            statusReqProc.Setup(reqProc => reqProc.ProcessResults(It.IsAny<string>()))
                .Returns(new List<Status> { new Status { Text = "Test" } });

            twitterExecute = new Mock<ITwitterExecute>();
            twitterExecute.Setup(exec => 
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(TestStatusQueryResponse);

            twitterCtx = new Mock<TwitterContext>(twitterExecute.Object);
            twitterCtx.Setup(ctx => ctx.CreateRequestProcessor<Status>())
                .Returns(statusReqProc.Object);
        }

        [Fact]
        public void ReplyWithMedia_Throws_On_Null_Status()
        {
            status = null;

            var ex = 
                Assert.Throws<ArgumentNullException>(() =>
                {
                    StatusExtensions.ReplyWithMedia(
                        twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                        latitude, longitude, placeID, displayCoordinates, mediaItems, null);
                });

            Assert.Equal("status", ex.ParamName);
        }

        [Fact]
        public void ReplyWithMedia_Calls_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()), 
                Times.Once(),
                "PostMedia was not called only one time.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_Properly_Formatted_Url_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.Is<string>(url => url == ExpectedUploadUrl),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass properly formatted URL.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_Status_via_Parameter_Dictionary_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["status"] == status),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass status properly.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_possiblySensitive_via_Parameter_Dictionary_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["possibly_sensitive"] == possiblySensitive.ToString(CultureInfo.InvariantCulture)),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass possiblySensitive parameter properly.");
        }

        [Fact]
        public void ReplyWithMedia_DoesNot_Pass_false_possiblySensitive_via_Parameter_Dictionary_to_PostMedia()
        {
            possiblySensitive = false;

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["possibly_sensitive"] == null),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia should not have passed possiblySensitive parameter.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_placeID_via_Parameter_Dictionary_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["place_id"] == placeID),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass placeID parameter properly.");
        }

        [Fact]
        public void ReplyWithMedia_DoesNot_Pass_null_placeID_via_Parameter_Dictionary_to_PostMedia()
        {
            placeID = null;

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["place_id"] == null),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia shouldn't pass placeID parameter.");
        }

        [Fact]
        public void ReplyWithMedia_DoesNot_Pass_Empty_placeID_via_Parameter_Dictionary_to_PostMedia()
        {
            placeID = "";

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["place_id"] == null),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia shouldn't pass placeID parameter.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_displayCoordinates_via_Parameter_Dictionary_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["display_coordinates"] == displayCoordinates.ToString(CultureInfo.InvariantCulture)),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass displayCoordinates parameter properly.");
        }

        [Fact]
        public void ReplyWithMedia_DoesNot_Pass_false_displayCoordinates_via_Parameter_Dictionary_to_PostMedia()
        {
            displayCoordinates = false;

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["display_coordinates"] == null),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia shouldn't pass displayCoordinates parameter.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_inReplyToStatusID_via_Parameter_Dictionary_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["in_reply_to_status_id"] == inReplyToStatusID.ToString(CultureInfo.InvariantCulture)),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass inReplyToStatusID parameter properly.");
        }

        [Fact]
        public void ReplyWithMedia_DoesNot_Pass_NoReply_inReplyToStatusID_via_Parameter_Dictionary_to_PostMedia()
        {
            inReplyToStatusID = StatusExtensions.NoReply;

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["in_reply_to_status_id"] == null),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia shouldn't pass inReplyToStatusID parameter.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_latitude_via_Parameter_Dictionary_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["lat"] == latitude.ToString(CultureInfo.InvariantCulture)),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass latitude parameter properly.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_longitude_via_Parameter_Dictionary_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["long"] == longitude.ToString(CultureInfo.InvariantCulture)),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass longitude parameter properly.");
        }

        [Fact]
        public void ReplyWithMedia_DoesNot_Pass_NoCoordinate_Lat_And_Long_To_PostMedia()
        {
            latitude = StatusExtensions.NoCoordinate;
            longitude = StatusExtensions.NoCoordinate;

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(
                        parms =>
                        parms["lat"] == null &&
                        parms["long"] == null),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "Lat and long should be null.");
        }

        [Fact]
        public void ReplyWithMedia_Translates_Localized_Lat_And_Long()
        {
            string latString = latitude.ToString(CultureInfo.InvariantCulture);
            string longString = longitude.ToString(CultureInfo.InvariantCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(FrenchLocale);

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(
                        parms =>
                        parms["lat"] == latString &&
                        parms["long"] == longString),
                    It.IsAny<List<Media>>(),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "Invariant culture not applied to Lat and/or long.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_Media_to_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.Is<List<Media>>(media => ReferenceEquals(media, mediaItems)),
                    It.IsAny<IRequestProcessor<Status>>()),
                Times.Once(),
                "ReplyWithMedia didn't pass mediaItems properly.");
        }

        [Fact]
        public void ReplyWithMedia_Requires_NonNull_mediaItems()
        {
            mediaItems = null;

            var ex = 
                Assert.Throws<ArgumentNullException>(() =>
                    StatusExtensions.ReplyWithMedia(
                        twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                        latitude, longitude, placeID, displayCoordinates, mediaItems, null));

            Assert.Equal("mediaItems", ex.ParamName);
        }

        [Fact]
        public void ReplyWithMedia_Requires_At_Least_One_Media()
        {
            mediaItems = new List<Media>();

            var ex =
                Assert.Throws<ArgumentException>(() =>
                    StatusExtensions.ReplyWithMedia(
                        twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                        latitude, longitude, placeID, displayCoordinates, mediaItems, null));

            Assert.Equal("mediaItems", ex.ParamName);
        }

        [Fact]
        public void ReplyWithMedia_Calls_TwitterContext_CreateRequestProcessor()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterCtx.Verify(
                ctx => ctx.CreateRequestProcessor<Status>(),
                Times.Once(),
                "Didn't call CreateRequestProcessor.");
        }

        [Fact]
        public void ReplyWithMedia_Passes_IRequestProcessor_To_PostMedia()
        {
            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            twitterExecute.Verify(exec =>
                exec.PostMedia(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<List<Media>>(),
                    It.Is<IRequestProcessor<Status>>(reqProc => reqProc != null)),
                Times.Once(),
                "ReplyWithMedia didn't pass reqProc properly.");
        }

        [Fact]
        public void ReplyWithMedia_Returns_Status()
        {
            Status tweet = StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates, mediaItems, null);

            Assert.Equal("Test", tweet.Text);
        }

        [Fact]
        public void ReplyWithMedia_Sets_Callback()
        {
            twitterExecute.SetupSet(exec => exec.AsyncCallback);

            StatusExtensions.ReplyWithMedia(
                twitterCtx.Object, inReplyToStatusID, status, possiblySensitive,
                latitude, longitude, placeID, displayCoordinates,
                mediaItems, null);

            twitterExecute.VerifySet(exec => exec.AsyncCallback, Times.Once());
        }
    }
}
