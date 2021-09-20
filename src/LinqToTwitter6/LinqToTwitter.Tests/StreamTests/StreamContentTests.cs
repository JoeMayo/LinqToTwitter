using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitter.Tests.StreamContentTests
{
    [TestClass]
    public class StreamContentTests
    {
        Mock<ITwitterExecute> execMock = new();

        public StreamContentTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void ParseJson_WithTweetEntity_Succeeds()
        {
            var content = new StreamContent(execMock.Object, TweetContent);

            (StreamTweet entity, StreamEntityType entityType) = content.ParseJson(TweetContent);

            Assert.IsNotNull(entity);
            Assert.AreEqual("1439984798332866573", entity.Tweet.ID);
            Assert.IsTrue(entity.Tweet.Text.StartsWith("Very"));
            Assert.AreEqual("1324201416731160579", entity.MatchingRules[0].ID);
            Assert.AreEqual("funny things", entity.MatchingRules[0].Tag);
            Assert.AreEqual(StreamEntityType.Tweet, entityType);
        }

        const string TweetContent = @"{
	""data"": {
		""id"": ""1439984798332866573"",
		""text"": ""Very very true, Dev! ..😜😂💕\n#Dev https://t.co/cHFjan""
	},
	""matching_rules"": [
		{
			""id"": ""1324201416731160579"",
			""tag"": ""funny things""

        }
	]
}";
    }
}
