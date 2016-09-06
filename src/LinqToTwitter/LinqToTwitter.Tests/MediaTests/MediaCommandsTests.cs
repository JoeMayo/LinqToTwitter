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
        public MediaCommandsTests()
        {
            TestCulture.SetCulture();
        }

        async Task<TwitterContext> InitializeTwitterContext()
        {
            await Task.Delay(1);
            var authMock = new Mock<IAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();

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
