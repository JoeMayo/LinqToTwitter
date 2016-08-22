using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterPcl.Tests.AccountTests
{
    [TestClass]
    public class AccountCommandsTests
    {
        Mock<ITwitterExecute> execMock;

        public AccountCommandsTests()
        {
            TestCulture.SetCulture();
        }
  
        TwitterContext InitTwitterContextWithPostToTwitter<TEntity>(string response)
        {
            var authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(response);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(
                exec => exec.PostToTwitterAsync<Account>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            execMock.Setup(
                exec => exec.PostToTwitterAsync<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            execMock.Setup(
                exec => exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task UpdateAccountProfileAsync_Invokes_Executor_Execute()
        {
            const string ExpectedName = "Twitter API";
            const string Name = "Twitter API";
            const string Url = "http://www.mayosoftware.com";
            const string Location = "San Francisco, CA";
            const string Description = "The Real Twitter API.";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.UpdateAccountProfileAsync(Name, Url, Location, Description, true, SkipStatus);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/account/update_profile.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.AreEqual(ExpectedName, actual.Name);
        }

        [TestMethod]
        public async Task UpdateAccountProfileAsync_WithRawResult_Succeeds()
        {
            const string Name = "Twitter API";
            const string Url = "http://www.csharp-station.com";
            const string Location = "San Francisco, CA";
            const string Description = "The Real Twitter API.";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.UpdateAccountProfileAsync(Name, Url, Location, Description, true, SkipStatus);

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateAccountProfileAsync_Throws_On_Null_Input()
        {
            const string ExpectedParamName = "NoInput";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountProfileAsync(null, null, null, null, true, false));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountProfileAsync_Throws_On_Name_Over_20_Chars()
        {
            const string ExpectedParamName = "name";
            string name = new string(Enumerable.Repeat('x', 21).ToArray());
            const string Url = "http://www.csharp-station.com";
            const string Location = "San Francisco, CA";
            const string Description = "The Real Twitter API.";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountProfileAsync(name, Url, Location, Description, true, SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountProfile_Throws_On_Url_Over_100_Chars()
        {
            const string ExpectedParamName = "url";
            const string Name = "Joe";
            var url = new string(Enumerable.Repeat('x', 101).ToArray());
            const string Location = "Denver, CO";
            const string Description = "Open source developer for LINQ to Twitter";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountProfileAsync(Name, url, Location, Description, true, SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountProfileAsync_Throws_On_Location_Over_30_Chars()
        {
            const string ExpectedParamName = "location";
            const string Name = "Joe";
            const string Url = "http://www.csharp-station.com";
            var location = new string(Enumerable.Repeat('x', 31).ToArray());
            const string Description = "Open source developer for LINQ to Twitter";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountProfileAsync(Name, Url, location, Description, true, SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountProfileAsync_Throws_On_Description_Over_160_Chars()
        {
            const string ExpectedParamName = "description";
            const string Name = "Joe";
            const string Url = "http://www.csharp-station.com";
            const string Location = "Denver, CO";
            var description = new string(Enumerable.Repeat('x', 161).ToArray());
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountProfileAsync(Name, Url, Location, description, true, SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountImageAsync_Invokes_Executor_Execute()
        {
            const string ExpectedName = "Twitter API";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.UpdateAccountImageAsync(new byte[] { 1 }, "myFile.jpg", "jpg", SkipStatus);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    "https://api.twitter.com/1.1/account/update_profile_image.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.AreEqual(ExpectedName, actual.Name);
        }

        [TestMethod]
        public async Task UpdateAccountImageAsync_WithRawResult_Succeeds()
        {
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.UpdateAccountImageAsync(new byte[] { 1 }, "myFile.jpg", "jpg", SkipStatus);

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateAccountImageAsync_Throws_On_Empty_Image()
        {
            const string ExpectedParamName = "image";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountImageAsync(new byte[] { }, "myImage.jpg", "jpg", SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountImageAsync_Throws_On_Null_Image()
        {
            const string ExpectedParamName = "image";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountImageAsync(null, "myImage.jpg", "jpg", SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountColorsAsync_Invokes_Executor_Execute()
        {
            const string Background = "9ae4e8";
            const string Text = "#000000";
            const string Link = "#0000ff";
            const string SidebarFill = "#e0ff92";
            const string SidebarBorder = "#87bc44";
            string expectedName = "Twitter API";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.UpdateAccountColorsAsync(Background, Text, Link, SidebarFill, SidebarBorder, true, SkipStatus);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/account/update_profile_colors.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        public async Task UpdateAccountColorsAsync_WithRawResult_Succeeds()
        {
            const string Background = "9ae4e8";
            const string Text = "#000000";
            const string Link = "#0000ff";
            const string SidebarFill = "#e0ff92";
            const string SidebarBorder = "#87bc44";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.UpdateAccountColorsAsync(Background, Text, Link, SidebarFill, SidebarBorder, true, SkipStatus);

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateAccountColorsAsync_Throws_On_No_Input()
        {
            const string ExpectedParamName = "NoInput";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountColorsAsync(null, null, null, null, null, true, SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountColorsAsync_Allows_Null_Parameters()
        {
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User user = await ctx.UpdateAccountColorsAsync("#9ae4e8", null, null, null, null, true, SkipStatus);

            Assert.IsNotNull(user);

            user = await ctx.UpdateAccountColorsAsync(null, "#9ae4e8", null, null, null, true, SkipStatus);

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task UpdateAccountBackgroundImageAsync_Invokes_Executor_PostTwitterFile()
        {
            const bool Tile = false;
            const bool IncludeEntities = false;
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.UpdateAccountBackgroundImageAsync(
                new byte[] { 1 }, "image.png", "png", Tile, IncludeEntities, SkipStatus);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    "https://api.twitter.com/1.1/account/update_profile_background_image.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task UpdateAccountBackgroundImageAsync_WithRawResult_Succeeds()
        {
            const bool Tile = false;
            const bool IncludeEntities = false;
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.UpdateAccountBackgroundImageAsync(
                new byte[] { 1 }, "image.png", "png", Tile, IncludeEntities, SkipStatus);

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateAccountBackgroundImageAsync_Throws_On_Empty_Image()
        {
            const string ExpectedParamName = "image";
            const bool Tile = false;
            const bool IncludeEntities = false;
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountBackgroundImageAsync(
                    new byte[] { }, "image.png", "png", Tile, IncludeEntities, SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountBackgroundImageAsync_Throws_On_Null_Image()
        {
            const string ExpectedParamName = "imageOrmediaID";
            const bool Tile = false;
            const bool IncludeEntities = false;
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountBackgroundImageAsync(
                    null, "image.png", "png", Tile, IncludeEntities, SkipStatus));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateAccountSettingsAsync_Invokes_Executor_Execute()
        {
            var ctx = InitTwitterContextWithPostToTwitter<Account>(SettingsResponse);
            var parameters = new Dictionary<string, string>
            {
                { "time_zone", "MST" },
                { "lang", "en" },
                { "trend_location_woeid", "1" },
                { "sleep_time_enabled", "true" },
                { "start_sleep_time", "20" },
                { "end_sleep_time", "6" },
            };

            Account acct = await ctx.UpdateAccountSettingsAsync(1, true, 20, 6, "MST", "en");

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<Account>(
                    "https://api.twitter.com/1.1/account/settings.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.IsNotNull(acct);
            Settings settings = acct.Settings;
            Assert.IsNotNull(settings);
            Assert.AreEqual("en", settings.Language);
        }

        [TestMethod]
        public async Task UpdateAccountSettingsAsync_WithRawResult_Succeeds()
        {
            var ctx = InitTwitterContextWithPostToTwitter<Account>(SettingsResponse);

            await ctx.UpdateAccountSettingsAsync(1, true, 20, 6, "MST", "en");

            Assert.AreEqual(SettingsResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateAccountSettingsAsync_Throws_On_No_Input()
        {
            const string ExpectedParamName = "NoInput";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateAccountSettingsAsync(null, null, null, null, null, null));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateDeliveryDeviceAsync_Invokes_Executor_Execute()
        {
            var ctx = InitTwitterContextWithPostToTwitter<Account>(SettingsResponse);
            var parameters = new Dictionary<string, string>
            {
                { "device", DeviceType.Sms.ToString().ToLower() },
                { "include_entities", true.ToString().ToLower() }
            };

            Account acct = await ctx.UpdateDeliveryDeviceAsync(DeviceType.Sms, true);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<Account>(
                    "https://api.twitter.com/1.1/account/update_delivery_device.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.IsNotNull(acct);
            Settings settings = acct.Settings;
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public async Task UpdateDeliveryDeviceAsync_WithRawResult_Succeds()
        {
            var ctx = InitTwitterContextWithPostToTwitter<Account>(SettingsResponse);

            await ctx.UpdateDeliveryDeviceAsync(DeviceType.Sms, true);

            Assert.AreEqual(SettingsResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_Invokes_Executor_Execute()
        {
            const string ExpectedProfileBannerUrl = "https://si0.twimg.com/profile_images/1438634086/avatar_normal.png";
            byte[] banner = new byte[]{ 1, 2, 3 };
            const string FileName = "MyImage.png";
            const string FileType = "png";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.UpdateProfileBannerAsync(banner, FileName, FileType);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    "https://api.twitter.com/1.1/account/update_profile_banner.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.ProfileBannerUrl);
            Assert.AreEqual(ExpectedProfileBannerUrl, actual.ProfileBannerUrl);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_WithRawResult_Succeeds()
        {
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = "MyImage.png";
            const string FileType = "png";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.UpdateProfileBannerAsync(banner, FileName, FileType);

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_Throws_On_Null_Banner()
        {
            const string ExpectedParamName = "banner";
            byte[] banner = null;
            const string FileName = "MyImage.png";
            const string FileType = "png";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateProfileBannerAsync(banner, FileName, FileType));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_Throws_On_Empty_Banner()
        {
            const string ExpectedParamName = "banner";
            byte[] banner = new byte[0];
            const string FileName = "MyImage.png";
            const string FileType = "png";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateProfileBannerAsync(banner, FileName, FileType));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_Throws_On_Null_FileName()
        {
            const string ExpectedParamName = "fileName";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = null;
            const string FileType = "png";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateProfileBannerAsync(banner, FileName, FileType));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_Throws_On_Empty_FileName()
        {
            const string ExpectedParamName = "fileName";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = "";
            const string FileType = "png";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateProfileBannerAsync(banner, FileName, FileType));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_Throws_On_Null_FileType()
        {
            const string ExpectedParamName = "imageType";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = "MyFile.png";
            const string FileType = null;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateProfileBannerAsync(banner, FileName, FileType));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateProfileBannerAsync_Throws_On_Empty_FileType()
        {
            const string ExpectedParamName = "imageType";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = "MyFile.png";
            const string FileType = "";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateProfileBannerAsync(banner, FileName, FileType));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task RemoveProfileBannerAsync_Invokes_Executor_Execute()
        {
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.RemoveProfileBannerAsync();

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/account/remove_profile_banner.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task RemoveProfileBannerAsync_WithRawResult_Succeeds()
        {
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.RemoveProfileBannerAsync();

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        const string SingleUserResponse = @"{
   ""id"":6253282,
   ""id_str"":""6253282"",
   ""name"":""Twitter API"",
   ""screen_name"":""twitterapi"",
   ""location"":""San Francisco, CA"",
   ""description"":""The Real Twitter API. I tweet about API changes, service issues and happily answer questions about Twitter and our API. Don't get an answer? It's on my website."",
   ""url"":""http:\/\/dev.twitter.com"",
   ""protected"":false,
   ""followers_count"":1009508,
   ""friends_count"":31,
   ""listed_count"":10361,
   ""created_at"":""Wed May 23 06:01:13 +0000 2007"",
   ""favourites_count"":24,
   ""utc_offset"":-28800,
   ""time_zone"":""Pacific Time (US & Canada)"",
   ""geo_enabled"":true,
   ""verified"":true,
   ""statuses_count"":3278,
   ""lang"":""en"",
   ""status"":{
      ""created_at"":""Mon Apr 30 17:16:17 +0000 2012"",
      ""id"":197011505181507585,
      ""id_str"":""197011505181507585"",
      ""text"":""Developer Teatime is coming to Paris - please sign up to join us on June 16th! https:\/\/t.co\/pQOUNKGD  @rno @jasoncosta"",
      ""source"":""web"",
      ""truncated"":false,
      ""in_reply_to_status_id"":null,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_user_id"":null,
      ""in_reply_to_user_id_str"":null,
      ""in_reply_to_screen_name"":null,
      ""geo"":null,
      ""coordinates"":null,
      ""place"":null,
      ""contributors"":[
         14927800
      ],
      ""retweet_count"":25,
      ""favorited"":false,
      ""retweeted"":false,
      ""possibly_sensitive"":false
   },
   ""contributors_enabled"":true,
   ""is_translator"":false,
   ""profile_background_color"":""E8F2F7"",
   ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/229557229\/twitterapi-bg.png"",
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/229557229\/twitterapi-bg.png"",
   ""profile_background_tile"":false,
   ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",
   ""profile_banner_url"":""https:\/\/si0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",   
   ""profile_link_color"":""0094C2"",
   ""profile_sidebar_border_color"":""0094C2"",
   ""profile_sidebar_fill_color"":""A9D9F1"",
   ""profile_text_color"":""437792"",
   ""profile_use_background_image"":true,
   ""show_all_inline_media"":false,
   ""default_profile"":false,
   ""default_profile_image"":false,
   ""following"":false,
   ""follow_request_sent"":false,
   ""notifications"":false
}";

        const string SettingsResponse = @"{
   ""screen_name"":""Linq2Tweeter"",
   ""protected"":false,
   ""geo_enabled"":false,
   ""time_zone"":{
      ""name"":""Mountain Time (US & Canada)"",
      ""utc_offset"":-25200,
      ""tzinfo_name"":""America\/Denver""
   },
   ""sleep_time"":{
      ""enabled"":true,
      ""start_time"":20,
      ""end_time"":8
   },
   ""show_all_inline_media"":true,
   ""discoverable_by_email"":true,
   ""trend_location"":[
      {
         ""woeid"":23424977,
         ""name"":""United States"",
         ""country"":""United States"",
         ""countryCode"":""US"",
         ""placeType"":{
            ""name"":""Country"",
            ""code"":12
         },
         ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/23424977"",
         ""parentid"":1
      }
   ],
   ""language"":""en"",
   ""always_use_https"":true
}";
    }
}
