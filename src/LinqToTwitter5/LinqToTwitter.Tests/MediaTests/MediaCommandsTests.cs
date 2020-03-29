using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterPcl.Tests.StatusTests
{
    [TestClass]
    public class MediaCommandsTests
    {
        Mock<IAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public MediaCommandsTests()
        {
            TestCulture.SetCulture();
        }

        async Task<TwitterContext> InitializeTwitterContext()
        {
            await Task.Delay(1);
            authMock = new Mock<IAuthorizer>();
            execMock = new Mock<ITwitterExecute>();

            var tcsAuth = new TaskCompletionSource<IAuthorizer>();
            tcsAuth.SetResult(authMock.Object);

            var tcsMedia = new TaskCompletionSource<string>();
            tcsMedia.SetResult(MediaResponse);

            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsMedia.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<MediaMetadata>(),
                It.IsAny<CancellationToken>()))
                .Returns(tcsMedia.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task UploadMediaAsync_WithBinaryImage_ReturnsMedia()
        {
            const ulong ExpectedMediaID = 521449660083609601ul;
            string mediaType = "image/jpg";
            var image = new byte[] { 1, 2, 3 };
            var additionalOwners = new List<ulong> { 1, 2 };
            string mediaCategory = "tweet_image";
            TwitterContext ctx = await InitializeTwitterContext();

            Media actual = await ctx.UploadMediaAsync(image, mediaType, additionalOwners, mediaCategory);

            Assert.AreEqual(ExpectedMediaID, actual.MediaID);
        }

        [TestMethod]
        public async Task CreateMediaMetadataAsync_WithValidParameters_Succeeds()
        {
            ulong mediaID = 521449660083609601ul;
            string altText = "Sample media description";

            TwitterContext ctx = await InitializeTwitterContext();

            await ctx.CreateMediaMetadataAsync(mediaID, altText);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://upload.twitter.com/1.1/media/metadata/create.json",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<MediaMetadata>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task CreateMediaMetadataAsync_WithZeroMediaID_Throws()
        {
            ulong mediaID = 0;
            string altText = "Sample media description";

            TwitterContext ctx = await InitializeTwitterContext();

            await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.CreateMediaMetadataAsync(mediaID, altText));
        }

        [TestMethod]
        public async Task CreateMediaMetadataAsync_WithEmptyAltText_Throws()
        {
            ulong mediaID = 521449660083609601ul;
            string altText = "";

            TwitterContext ctx = await InitializeTwitterContext();

            await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.CreateMediaMetadataAsync(mediaID, altText));
        }

        [TestMethod]
        public async Task CreateMediaMetadataAsync_WithNullAltText_Throws()
        {
            ulong mediaID = 521449660083609601ul;
            string altText = null;

            TwitterContext ctx = await InitializeTwitterContext();

            await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.CreateMediaMetadataAsync(mediaID, altText));
        }

        const string MediaResponse = @"{
	""media_id"": 521449660083609601,
	""media_id_string"": ""521449660083609601"",
	""size"": 6955,
	""image"": {
		""w"": 100,
		""h"": 100,
		""image_type"": ""image\/png""
	}
}";
    }
}
