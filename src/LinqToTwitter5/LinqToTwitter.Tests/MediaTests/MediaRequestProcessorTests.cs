using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.StatusTests
{
    [TestClass]
    public class MediaRequestProcessorTests
    {
        public const string BaseUrlAndVersion = "https://upload.twitter.com/1.1/";

        public MediaRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Handles_Input_Params()
        {
            var reqProc = new MediaRequestProcessor<Media>();

            Expression<Func<Media, bool>> expression =
            media =>
                media.Type == MediaType.Status &&
                media.Command == Media.StatusCommand &&
                media.MediaID == 123;

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Media.Type), ((int)MediaType.Status).ToString())));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(Media.Command), Media.StatusCommand)));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>(nameof(Media.MediaID), 123.ToString())));
        }
        
        [TestMethod]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            var mediaReqProc = new MediaRequestProcessor<Media> { BaseUrl = BaseUrlAndVersion };
            var parameters = new Dictionary<string, string> { };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => mediaReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(Media.Type), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Throws_On_Null_Parameter()
        {
            var target = new MediaRequestProcessor<Media> { BaseUrl = BaseUrlAndVersion };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => target.BuildUrl(null));

            Assert.AreEqual(nameof(Media.Type), ex.ParamName);
        }

        [TestMethod]
        public void BuildStatusUrl_MissingRequiredMediaID_Throws()
        {
            var target = new MediaRequestProcessor<Media> { BaseUrl = BaseUrlAndVersion };

            var parameters = new Dictionary<string, string>
            {
                { nameof(Media.Type), ((int)MediaType.Status).ToString() },
                { nameof(Media.Command), Media.StatusCommand },
                //{ nameof(Media.MediaID), 123.ToString() }
            };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => target.BuildUrl(parameters));

            Assert.AreEqual(nameof(Media.MediaID), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Status_Url()
        {
            const string ExpectedUrl = BaseUrlAndVersion + "media/upload.json?command=STATUS&media_id=123";
            var reqProc = new MediaRequestProcessor<Media>
            {
                Type = MediaType.Status,
                UploadUrl = BaseUrlAndVersion
            };
            var parameters = new Dictionary<string, string>
            {
                { nameof(Media.Type), ((int)MediaType.Status).ToString() },
                { nameof(Media.Command), Media.StatusCommand },
                { nameof(Media.MediaID), 123.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildStatusUrl_WithoutOptionalCommand_HasDefault()
        {
            const string ExpectedUrl = BaseUrlAndVersion + "media/upload.json?command=STATUS&media_id=123";
            var reqProc = new MediaRequestProcessor<Media>
            {
                Type = MediaType.Status,
                UploadUrl = BaseUrlAndVersion
            };
            var parameters = new Dictionary<string, string>
            {
                { nameof(Media.Type), ((int)MediaType.Status).ToString() },
                //{ nameof(Media.Command), Media.StatusCommand },
                { nameof(Media.MediaID), 123.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void MediaRequestProcessor_Works_With_Json_Format_Data()
        {
            var statProc = new MediaRequestProcessor<Media> { BaseUrl = BaseUrlAndVersion };

            Assert.IsInstanceOfType(statProc, typeof(IRequestProcessorWantsJson));
        }

        [TestMethod]
        public void ProcessResults_WithInProgressResponse_IsProcessed()
        {
            var statProc = new MediaRequestProcessor<Media>
            {
                Type = MediaType.Status,
                UploadUrl = BaseUrlAndVersion,
                MediaID = 772492064386912256ul
            };

            List<Media> mediaList = statProc.ProcessResults(MediaStatusInProgressResponse);

            Assert.IsNotNull(mediaList);
            Assert.AreEqual(1, mediaList.Count);
            Media media = mediaList.Single();
            Assert.IsNotNull(media);
            Assert.AreEqual(772492064386912256ul, media.MediaID);
            Assert.IsNotNull(media.ProcessingInfo);
            MediaProcessingInfo info = media.ProcessingInfo;
            Assert.AreEqual(MediaProcessingInfo.InProgress, info.State);
            Assert.AreEqual(5, info.CheckAfterSeconds);
            Assert.AreEqual(5, info.ProgressPercent);
        }

        [TestMethod]
        public void ProcessResults_WithFailedResponse_IsProcessed()
        {
            var statProc = new MediaRequestProcessor<Media>
            {
                Type = MediaType.Status,
                UploadUrl = BaseUrlAndVersion,
                MediaID = 773000650376548356ul
            };

            List<Media> mediaList = statProc.ProcessResults(MediaStatusFailedResponse);

            Assert.IsNotNull(mediaList);
            Assert.AreEqual(1, mediaList.Count);
            Media media = mediaList.Single();
            Assert.IsNotNull(media);
            Assert.AreEqual(773000650376548356ul, media.MediaID);
            Assert.AreEqual(28276017, media.Size);
            Assert.IsNotNull(media.ProcessingInfo);
            MediaProcessingInfo info = media.ProcessingInfo;
            Assert.AreEqual(MediaProcessingInfo.Failed, info.State);
            Assert.AreEqual(100, info.ProgressPercent);
            Assert.IsNotNull(info.Error);
            MediaError error = info.Error;
            Assert.AreEqual(3, error.Code);
            Assert.AreEqual("UnsupportedMedia", error.Name);
            Assert.AreEqual("Duration 10.minutes+22.seconds+628.milliseconds exceeds max duration of 2.minutes+20.seconds", error.Message);
        }

        [TestMethod]
        public void ProcessResults_WithSucceededResponse_IsProcessed()
        {
            var statProc = new MediaRequestProcessor<Media>
            {
                Type = MediaType.Status,
                UploadUrl = BaseUrlAndVersion,
                MediaID = 773017325037580289ul
            };

            List<Media> mediaList = statProc.ProcessResults(MediaStatusSucceededResponse);

            Assert.IsNotNull(mediaList);
            Assert.AreEqual(1, mediaList.Count);
            Media media = mediaList.Single();
            Assert.IsNotNull(media);
            Assert.AreEqual(773017325037580289ul, media.MediaID);
            Assert.AreEqual(7319206, media.Size);
            Assert.AreEqual(86395, media.ExpiresAfterSeconds);
            Assert.AreEqual("video/mp4", media.VideoType);
            Assert.IsNotNull(media.ProcessingInfo);
            MediaProcessingInfo info = media.ProcessingInfo;
            Assert.AreEqual(MediaProcessingInfo.Succeeded, info.State);
            Assert.AreEqual(100, info.ProgressPercent);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var statProc = new MediaRequestProcessor<Media>() { BaseUrl = BaseUrlAndVersion };

            List<Media> stats = statProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, stats.Count);
        }

        [TestMethod]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var statProc = new MediaRequestProcessor<Media>() 
            { 
                BaseUrl = BaseUrlAndVersion,
                Type = MediaType.Status,
                Command = "STATUS",
                MediaID = 123,
            };

            List<Media> mediaList = statProc.ProcessResults(MediaStatusInProgressResponse);

            Assert.IsNotNull(mediaList);
            Assert.AreEqual(1, mediaList.Count);
            Media media = mediaList.Single();
            Assert.IsNotNull(media);
            Assert.AreEqual(MediaType.Status, media.Type);
            Assert.AreEqual("STATUS", media.Command);
            Assert.AreEqual(123ul, media.MediaID);
        }

        const string MediaStatusInProgressResponse = @"{
	""media_id"": 772492064386912256,
	""media_id_string"": ""772492064386912256"",
	""processing_info"": {
		""state"": ""in_progress"",
		""check_after_secs"": 5,
		""progress_percent"": 5
	}
}";

        const string MediaStatusFailedResponse = @"{
	""media_id"": 773000650376548356,
	""media_id_string"": ""773000650376548356"",
	""size"": 28276017,
	""processing_info"": {
		""state"": ""failed"",
		""progress_percent"": 100,
		""error"": {
			""code"": 3,
			""name"": ""UnsupportedMedia"",
			""message"": ""Duration 10.minutes+22.seconds+628.milliseconds exceeds max duration of 2.minutes+20.seconds""
		}
	}
}";

        const string MediaStatusSucceededResponse = @"{
	""media_id"": 773017325037580289,
	""media_id_string"": ""773017325037580289"",
	""size"": 7319206,
	""expires_after_secs"": 86395,
	""video"": {
		""video_type"": ""video\/mp4""
	},
	""processing_info"": {
		""state"": ""succeeded"",
		""progress_percent"": 100
	}
}";
    }
}
