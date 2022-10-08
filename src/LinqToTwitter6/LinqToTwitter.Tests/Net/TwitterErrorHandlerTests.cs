using System.Collections.Generic;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToTwitter.Net;
using System.Linq;

namespace LinqToTwitter.Tests.Net
{
    [TestClass]
    public class TwitterErrorHandlerTests
    {
        public TwitterErrorHandlerTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void ParseTwitterErrorMessage_WithApiV1Error_Parses()
        {
            TwitterErrorDetails errorDetails = TwitterErrorHandler.ParseTwitterErrorMessage(ApiV1ErrorJson);

            Assert.IsNotNull(errorDetails);
            List<Error> errors = errorDetails.Errors;
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Error error = errors.First();
            Assert.AreEqual(144, error.Code);
            Assert.AreEqual("No status found with that ID.", error.Message);
        }

        [TestMethod]
        public void ParseTwitterErrorMessage_WithApiV1RequestError_Parses()
        {
            TwitterErrorDetails errorDetails = TwitterErrorHandler.ParseTwitterErrorMessage(MediaErrorJson);

            Assert.IsNotNull(errorDetails);
            List<Error> errors = errorDetails.Errors;
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Error error = errors.First();
            Assert.AreEqual("/1.1/media/metadata/create.json", error.Request);
            Assert.AreEqual("media_id field must be provided.", error.Message);
        }

        [TestMethod]
        public void ParseTwitterErrorMessage_WithApiV2Error_Parses()
        {
            TwitterErrorDetails errorDetails = TwitterErrorHandler.ParseTwitterErrorMessage(ApiV2ErrorJson);

            Assert.IsNotNull(errorDetails);
            Assert.AreEqual("Invalid Request", errorDetails.Title);
            Assert.AreEqual("One or more parameters to your request was invalid.", errorDetails.Detail);
            Assert.AreEqual("https://api.twitter.com/labs/2/problems/invalid-request", errorDetails.Type);
            List<Error> errors = errorDetails.Errors;
            Assert.IsNotNull(errors);
            Assert.AreEqual(2, errors.Count);
            Error error = errors[1];
            Assert.AreEqual("[q] is not one of [query,start_time,end_time,since_id,until_id,max_results,next_token,expansions,tweet.fields,media.fields,poll.fields,place.fields,user.fields]", error.Message);
            Dictionary<string, string[]> parameters = error.Parameters;
            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Count);
            Assert.IsTrue(parameters.ContainsKey("q"));
            string[] values = parameters["q"];
            Assert.IsNotNull(values);
            Assert.AreEqual(1, values.Count());
            string value = values.First();
            Assert.AreEqual("LINQ%20to%20Twitter", value);
        }

        [TestMethod]
        public void ParseTwitterErrorMessage_WithApiV2AccountLockedError_Parses()
        {
            TwitterErrorDetails errorDetails = TwitterErrorHandler.ParseTwitterErrorMessage(AccountLockedErrorJson);

            Assert.IsNotNull(errorDetails);
            Assert.AreEqual("Forbidden", errorDetails.Title);
            Assert.AreEqual("Your account is temporarily locked. Please log in to https://twitter.com to unlock your account.", errorDetails.Detail);
            Assert.AreEqual("about:blank", errorDetails.Type);
            Assert.AreEqual(403, errorDetails.Status);
        }

        [TestMethod]
        public void ParseTwitterErrorMessage_WithUnrecognizedContent_SendsErrorMessage()
        {
            const string GarbageIn = "This is garbage";

            TwitterErrorDetails errorDetails = TwitterErrorHandler.ParseTwitterErrorMessage(GarbageIn);

            Assert.IsNotNull(errorDetails);
            Assert.IsTrue(errorDetails.Title?.StartsWith("Unhandled Error") ?? false);
            Assert.AreEqual(GarbageIn, errorDetails.Detail);
        }

        [TestMethod]
        public void ParseTwitterErrorMessage_WithApiV2XmlErrorCode415_Parses()
        {
            TwitterErrorDetails errorDetails = TwitterErrorHandler.ParseTwitterErrorMessage(ApiV2ErrorXml);

            Assert.IsNotNull(errorDetails);
            Assert.AreEqual(ApiV2ErrorXml, errorDetails.Detail);
            List<Error> errors = errorDetails.Errors;
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            Error error = errors.First();
            Assert.AreEqual(415, error.Code);
            Assert.AreEqual("Callback URL not approved for this client application. Approved callback URLs can be adjusted in your application settings", error.Message);
        }

        const string AccountLockedErrorJson = @"{
	""title"": ""Forbidden"",
	""detail"": ""Your account is temporarily locked. Please log in to https://twitter.com to unlock your account."",
	""type"": ""about:blank"",
	""status"": 403
}";

        const string ApiV1ErrorJson = @"{
	""errors"": [
		{
			""code"": 144,
			""message"": ""No status found with that ID.""
		}
	]
}";

        const string ApiV2ErrorJson = @"{
	""errors"": [
		{
			""parameters"": {
				""query"": []
    },
			""message"": ""Request parameter `query` can not be empty""
		},
		{
    ""parameters"": {
        ""q"": [

                    ""LINQ%20to%20Twitter""
				]
			},
			""message"": ""[q] is not one of [query,start_time,end_time,since_id,until_id,max_results,next_token,expansions,tweet.fields,media.fields,poll.fields,place.fields,user.fields]""

        }
	],
	""title"": ""Invalid Request"",
	""detail"": ""One or more parameters to your request was invalid."",
	""type"": ""https://api.twitter.com/labs/2/problems/invalid-request""
}";

        const string MediaErrorJson = @"{
	""request"": ""/1.1/media/metadata/create.json"",
	""error"": ""media_id field must be provided.""
}";

        const string ApiV2ErrorXml = @"<?xml version='1.0' encoding='UTF-8'?>
<errors>
    <error code=""415"">Callback URL not approved for this client application. Approved callback URLs can be adjusted in your application settings</error>
</errors>";
    }
}
