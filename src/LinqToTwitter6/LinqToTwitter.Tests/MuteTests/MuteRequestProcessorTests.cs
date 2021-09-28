using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter.Tests.MuteTests
{
    [TestClass]
    public class MuteRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public MuteRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_WithInputParams_Succeeds()
        {
            var target = new MuteRequestProcessor<Mute>();
            Expression<Func<Mute, bool>> expression =
                mute =>
                    mute.Type == MuteType.Muted &&
                    mute.ID == "123";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Mute.Type), ((int)MuteType.Muted).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Mute.ID), "123")));
        }

        [TestMethod]
        public void BuildUrl_WithParams_ConstructsUrl()
        {
            const string ExpectedUrl = BaseUrl2 + "users/123/muting";
            var reqProc = new MuteRequestProcessor<Mute> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                        { nameof(Mute.Type), ((int)MuteType.Muted).ToString(CultureInfo.InvariantCulture) },
                        { nameof(Mute.ID), "123" }
                };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_WithMutesResponse_Succeeds()
        {
            var reqProc = new MuteRequestProcessor<Mute> { Type = MuteType.Muted };

            List<Mute> mutes = reqProc.ProcessResults(MutesResponse);

            Assert.IsNotNull(mutes);
            Assert.IsNotNull(mutes.SingleOrDefault());
            var mute = mutes.Single();
            Assert.IsNotNull(mute);
            var users = mute.Users;
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());
            TwitterUser user = users.First();
            Assert.IsNotNull(user);
            Assert.AreEqual("30892897", user.ID);
            Assert.AreEqual("BigWallGraphics", user.Name);
            Assert.AreEqual("BigWallGraphics", user.Username);
            MuteMeta meta = mute.Meta;
            Assert.IsNotNull(meta);
            Assert.AreEqual(2, meta.ResultCount);
        }

        const string MutesResponse = @"{
	""data"": [
		{
			""id"": ""30892897"",
			""name"": ""BigWallGraphics"",
			""username"": ""BigWallGraphics""
		},
		{
			""id"": ""67294478"",
			""name"": ""Walls360"",
			""username"": ""Walls360""
		}
	],
	""meta"": {
		""result_count"": 2
	}
}";
    }
}
