using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.MuteTests
{
    [TestClass]
    public class VineRequestProcessorTests
    {
        public VineRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Handles_Input_Params()
        {
            var target = new VineRequestProcessor<Vine>();
            Expression<Func<Vine, bool>> expression =
                vine =>
                    vine.Type == VineType.Oembed &&
                    vine.ID == "123" &&
                    vine.Url == "https://www.vine.com/v/123" &&
                    vine.MaxWidth == 500 &&
                    vine.MaxHeight == 500 &&
                    vine.OmitScript == true;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)VineType.Oembed).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Url", "https://www.vine.com/v/123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxWidth", "500")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxHeight", "500")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("OmitScript", "True")));
        }

        [TestMethod]
        public void BuildUrl_Constructs_Oembeds_Url()
        {
            const string ExpectedUrl = "https://vine.co/oembed.json?id=Ml16lZVTTxe&url=https%3A%2F%2Fvine.co%2Fv%2FMl16lZVTTxe&max_width=500&max_height=500&omit_script=true";
            var reqProc = new VineRequestProcessor<Vine> { VineUrl = "https://vine.co/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)VineType.Oembed).ToString(CultureInfo.InvariantCulture) },
                        { "ID", "Ml16lZVTTxe" },
                        { "Url", "https://vine.co/v/Ml16lZVTTxe" },
                        { "MaxWidth", 500.ToString() },
                        { "MaxHeight", 500.ToString() },
                        { "OmitScript", true.ToString() }
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_Parses_Oembed_Response()
        {
            var reqProc = new VineRequestProcessor<Vine> { Type = VineType.Oembed };

            List<Vine> vines = reqProc.ProcessResults(OembedResponse);

            Assert.IsNotNull(vines);
            Assert.IsNotNull(vines.SingleOrDefault());
            var vine = vines.Single();
            Assert.IsNotNull(vine);
            Assert.AreEqual(1.0d, vine.Version);
            Assert.AreEqual("video", vine.TypeResponse);
            Assert.AreEqual(3153600000, vine.CacheAge);
            Assert.AreEqual("Vine", vine.ProviderName);
            Assert.AreEqual("https://vine.co/", vine.ProviderUrl);
            Assert.AreEqual("The White House", vine.AuthorName);
            Assert.AreEqual("https://vine.co/u/944054069482373120", vine.AuthorUrl);
            Assert.AreEqual("Marine One, ready for takeoff. \ud83d\ude81", vine.Title);
            Assert.AreEqual("https://v.cdn.vine.co/r/thumbs/3A1E5CC27C1116110482407841792_2.5.1.13603721890676705621.mp4.jpg?versionId=_B5dDtCI.ZNd_JLK_.sFmfpQq44iXdey", vine.ThumbnailUrl);
            Assert.AreEqual(480, vine.ThumbnailWidth);
            Assert.AreEqual(480, vine.ThumbnailHeight);
            Assert.IsTrue(vine.Html.StartsWith("<iframe"));
            Assert.AreEqual(600, vine.Width);
            Assert.AreEqual(600, vine.Height);
        }

        const string OembedResponse = @"{
  ""version"": 1.0,
  ""type"": ""video"",
  ""cache_age"": 3153600000,
  ""provider_name"": ""Vine"",
  ""provider_url"": ""https://vine.co/"",
  ""author_name"": ""The White House"",
  ""author_url"": ""https://vine.co/u/944054069482373120"",

  ""title"": ""Marine One, ready for takeoff. \ud83d\ude81"",

  ""thumbnail_url"": ""https://v.cdn.vine.co/r/thumbs/3A1E5CC27C1116110482407841792_2.5.1.13603721890676705621.mp4.jpg?versionId=_B5dDtCI.ZNd_JLK_.sFmfpQq44iXdey"",
  ""thumbnail_width"": 480,
  ""thumbnail_height"": 480,
  ""html"": ""<iframe class=\""vine-embed\"" src=\""https://vine.co/v/Ml16lZVTTxe/embed/simple\"" width=\""600\"" height=\""600\"" frameborder=\""0\""><\/iframe><script async src=\""//platform.vine.co/static/scripts/embed.js\""><\/script>"",
  ""width"": 600,
  ""height"": 600
}";

    }
}
