using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using LinqToTwitter;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.LinqToTwitterTests
{
    public class TwitterExecuteErrorHandlingTests
    {
        readonly TwitterExecute twitExe;

        public TwitterExecuteErrorHandlingTests()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            twitExe = new TwitterExecute(authMock.Object);
        }

        [Fact]
        public void CheckResultsForTwitterError_Throws_On_Twitter_Error()
        {
            var ex = Assert.Throws<TwitterQueryException>(
                () => twitExe.CheckResultsForTwitterError(TwitterErrorResponse, null));

            Assert.Equal(34, ex.ErrorCode);
            Assert.Equal("Sorry, that page does not exist", ex.Message);
        }

        [Fact]
        public void CheckResultsForTwitterError_Does_Not_Throw_On_No_Error()
        {
            Assert.DoesNotThrow(
                () => twitExe.CheckResultsForTwitterError(NonError, null));
        }

        WebException CreateWebException(string responseContent)
        {
            var memStr = new MemoryStream(Encoding.UTF8.GetBytes(responseContent));
            memStr.Position = 0;
            var webRespMock = new Mock<WebResponse>();
            webRespMock.Setup(resp => resp.GetResponseStream()).Returns(memStr);
            webRespMock.Setup(resp => resp.ResponseUri).Returns(new Uri("http://mysite.com"));
            webRespMock.Setup(resp => resp.Headers).Returns(new WebHeaderCollection());
            webRespMock.Setup(resp => resp.ContentType).Returns("text/plain");

            var webEx =
                new WebException(
                    "Web Exception Message", null,
                    WebExceptionStatus.CacheEntryNotFound,
                    webRespMock.Object);

            return webEx;
        }

        [Fact]
        public void CreateTwitterQueryException_Returns_A_TwitterQueryException()
        {
            var webEx = CreateWebException(TwitterErrorResponse);

            var ex = twitExe.CreateTwitterQueryException(webEx);

            Assert.Equal(34, ex.ErrorCode);
            Assert.Equal("Sorry, that page does not exist", ex.Message);
            Assert.Equal(WebExceptionStatus.CacheEntryNotFound.ToString(), ex.HttpError);
        }
  
        [Fact]
        public void CreateTwitterQueryException_Returns_Default_TwitterQueryException_With_Unknown_Response()
        {
            var webEx = CreateWebException("<html><body>some html response</body></html>");

            var ex = twitExe.CreateTwitterQueryException(webEx);

            Assert.Equal(0, ex.ErrorCode);
            Assert.Equal("No message from Twitter", ex.Message);
            Assert.Equal(WebExceptionStatus.CacheEntryNotFound.ToString(), ex.HttpError);
        }

        const string NonError = "{}";

        const string TwitterErrorResponse = 
            @"{""errors"":[{""message"":""Sorry, that page does not exist"",""code"":34}]}";
    }
}
