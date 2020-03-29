using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToTwitter.Common;

namespace LinqToTwitterPcl.Tests.UserTests
{
    [TestClass]
    public class UserRequestProcessorTests
    {
        public UserRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void BuildUrl_Constructs_BannerSize_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/profile_banner.json?user_id=15411837&screen_name=JoeMayo";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.BannerSizes).ToString() },
                { "UserID", "15411837" },
                { "ScreenName", "JoeMayo" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_BannerSize_Requires_ScreenName_Or_UserID()
        {
            const string ExpectedParamName = "ScreenNameOrUserID";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.BannerSizes).ToString() },
                //{ "UserID", "15411837" },
                //{ "ScreenName", "JoeMayo" }
            };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_BannerSize_Requires_NonNull_UserID()
        {
            const string ExpectedParamName = "UserID";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.BannerSizes).ToString() },
                { "UserID", null },
                //{ "ScreenName", "JoeMayo" }
            };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_BannerSize_Requires_NonEmpty_UserID()
        {
            const string ExpectedParamName = "UserID";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.BannerSizes).ToString() },
                { "UserID", "" },
                //{ "ScreenName", "JoeMayo" }
            };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_BannerSize_Requires_NonNull_ScreenName()
        {
            const string ExpectedParamName = "ScreenName";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.BannerSizes).ToString() },
                //{ "UserID", null },
                { "ScreenName", null }
            };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_BannerSize_Requires_NonEmpty_ScreenName()
        {
            const string ExpectedParamName = "ScreenName";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.BannerSizes).ToString() },
                //{ "UserID", "" },
                { "ScreenName", "" }
            };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Show_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/show.json?user_id=15411837&screen_name=JoeMayo&include_entities=true";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Show).ToString() },
                { "UserID", "15411837" },
                { "ScreenName", "JoeMayo" },
                { "IncludeEntities", true.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Show_Throws_On_Null_UserID()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Show).ToString() },
                { "UserID", null }
            };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual("UserID", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Show_Throws_On_Null_ScreenName()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Show).ToString() },
                { "ScreenName", null }
            };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual("ScreenName", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Categories_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/suggestions/technology/members.json";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.CategoryStatus).ToString() },
                { "Slug", "Technology" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Categores_Throws_On_Missing_Slug()
        {
            var reqProc = new UserRequestProcessor<User>();
            var parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.CategoryStatus).ToString() },
                        //{ "Slug", "Technology" }
                    };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual("Slug", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Categories_Constructs_Url_For_Lang_Param()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/suggestions.json?lang=it";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Categories).ToString() },
                { "Lang", "it" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Category_Constructs_Url_For_Slug_And_Lang_Params()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/suggestions/twitter.json?lang=it";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Category).ToString() },
                { "Slug", "twitter" },
                { "Lang", "it" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Category_Thows_On_Missing_Slug()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Category).ToString() },
            };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual("Slug", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Lookup_Constructs_Url_With_ScreenNameList_Param()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/lookup.json?screen_name=JoeMayo%2CLinqToTweeter";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Lookup).ToString() },
                { "ScreenNameList", "JoeMayo,LinqToTweeter" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Lookup_Constructs_Url_With_UserIDList_Param()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/lookup.json?user_id=1%2C2";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Lookup).ToString() },
                { "UserIdList", "1,2" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Lookup_Throws_On_Missing_ScreenName()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Lookup).ToString() },
            };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual("ScreenNameListOrUserIdList", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Lookup_Throws_On_Both_UserID_And_ScreenName_Params()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Lookup).ToString() },
                { "ScreenName", "JoeMayo,LinqToTweeter" },
                { "UserID", "1,2" }
            };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual("ScreenNameListOrUserIdList", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Search_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/search.json?q=Joe%20Mayo&page=2&count=10&include_entities=true";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Search).ToString() },
                { "Query", "Joe Mayo" },
                { "Page", "2" },
                { "Count", "10" },
                { "IncludeEntities", true.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Search_Throws_On_Missing_Query()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Search).ToString() },
            };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual("Query", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Contributees_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/contributees.json?screen_name=JoeMayo&include_entities=true&skip_status=true";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Contributees).ToString() },
                //{ "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "IncludeEntities", true.ToString() },
                { "SkipStatus", true.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Contributors_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/contributors.json?user_id=123&include_entities=true&skip_status=true";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Contributors).ToString() },
                { "UserID", "123" },
                //{ "ScreenName", "JoeMayo" },
                { "IncludeEntities", true.ToString() },
                { "SkipStatus", true.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string> { };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.AreEqual<string>("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Throws_On_Null_Params()
        {
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => reqProc.BuildUrl(null));

            Assert.AreEqual<string>("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Lookup_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/users/lookup.json?user_id=1%2C2&include_entities=true";
            var reqProc = new UserRequestProcessor<User> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)UserType.Lookup).ToString() },
                { "UserIdList", "1,2" },
                { "IncludeEntities", true.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void GetParameters_Handles_Input_Params()
        {
            var reqProc = new UserRequestProcessor<User>();

            Expression<Func<User, bool>> expression =
            user =>
                user.Type == UserType.Show &&
                user.UserID == 10 &&
                user.UserIdList == "1,2" &&
                user.ScreenName == "JoeMayo" &&
                user.ScreenNameList == "JoeMayo,Linq2Tweeter" &&
                user.Cursor == 10819235 &&
                user.Slug == "twitter" &&
                user.Query == "Joe Mayo" &&
                user.Page == 2 &&
                user.Count == 10 &&
                user.Lang == "it" &&
                user.IncludeEntities == true &&
                user.SkipStatus == true &&
                user.ImageSize == ProfileImageSize.Mini;

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)UserType.Show).ToString())));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("UserID", "10")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("UserIdList", "1,2")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ScreenNameList", "JoeMayo,Linq2Tweeter")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Cursor", "10819235")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("Slug", "twitter")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Query", "Joe Mayo")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Page", "2")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Count", "10")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Lang", "it")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("IncludeEntities", "True")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("SkipStatus", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ImageSize", ((int)ProfileImageSize.Mini).ToString())));
        }

        [TestMethod]
        public void ProcessResults_Retains_Original_Input_Parameters()
        {
            var reqProc = new UserRequestProcessor<User> 
            { 
                Type = UserType.Show, 
                BaseUrl = "https://api.twitter.com/1.1/",
                UserID = 123ul,
                UserIdList = "1,2",
                ScreenName = "JoeMayo",
                ScreenNameList = "JoeMayo,Linq2Tweeter",
                Page = 1,
                Count = 10,
                Cursor = 456,
                Slug = "myslug",
                Query = "myquery",
                Lang = "en-US",
                SkipStatus = true,
                ImageSize = ProfileImageSize.Bigger,
                IncludeEntities = true
            };

            List<User> users = reqProc.ProcessResults(SingleUserResponse);

            Assert.IsNotNull(users);
            Assert.AreEqual(1, users.Count);
            User user = users.First();
            Assert.AreEqual(123ul, user.UserID);
            Assert.AreEqual("1,2", user.UserIdList);
            Assert.AreEqual("JoeMayo", user.ScreenName);
            Assert.AreEqual("JoeMayo,Linq2Tweeter", user.ScreenNameList);
            Assert.AreEqual(1, user.Page);
            Assert.AreEqual(10, user.Count);
            Assert.AreEqual(456L, user.Cursor);
            Assert.AreEqual("myslug", user.Slug);
            Assert.AreEqual("myquery", user.Query);
            Assert.AreEqual("en-US", user.Lang);
            Assert.IsTrue(user.SkipStatus);
            Assert.AreEqual(ProfileImageSize.Bigger, user.ImageSize);
            Assert.IsTrue(user.IncludeEntities);
        }

        [TestMethod]
        public void UserRequestProcessor_Works_With_Json_Format_Data()
        {
            var reqProc = new UserRequestProcessor<User>();

            Assert.IsInstanceOfType(reqProc, typeof(IRequestProcessorWantsJson));
        }

        [TestMethod]
        public void UserRequestProcessor_Handles_Actions()
        {
            var reqProc = new UserRequestProcessor<User>();

            Assert.IsInstanceOfType(reqProc, typeof(IRequestProcessorWithAction<User>));
        }

        void VerifySingleUserResponse(User user)
        {
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.BannerSizes);
            Assert.IsFalse(user.BannerSizes.Any());
            Assert.IsNotNull(user.Categories);
            Assert.IsFalse(user.Categories.Any());
            Assert.AreEqual("6253282", user.UserIDResponse);
            Assert.AreEqual("twitterapi", user.ScreenNameResponse);
            Assert.AreEqual("San Francisco, CA", user.Location);
            Assert.IsNotNull(user.Description);
            Assert.IsTrue(user.Description.StartsWith("The Real Twitter API."));
            Assert.AreEqual("http://dev.twitter.com", user.Url);
            Assert.IsFalse(user.Protected);
            Assert.AreEqual(1009508, user.FollowersCount);
            Assert.AreEqual(31, user.FriendsCount);
            Assert.AreEqual(10361, user.ListedCount);
            Assert.AreEqual(new DateTime(2007, 5, 23, 6, 1, 13), user.CreatedAt);
            Assert.AreEqual(24, user.FavoritesCount);
            Assert.AreEqual(-28800, user.UtcOffset);
            Assert.AreEqual("Pacific Time (US & Canada)", user.TimeZone);
            Assert.IsTrue(user.GeoEnabled);
            Assert.IsTrue(user.Verified);
            Assert.AreEqual(3278, user.StatusesCount);
            Assert.AreEqual("en", user.LangResponse);
            Status status = user.Status;
            Assert.IsNotNull(status);
            Assert.AreEqual("web", status.Source);
            List<Contributor> contributors = status.Contributors;
            Assert.IsNotNull(contributors);
            Assert.IsTrue(contributors.Any());
            Contributor contributor = contributors.First();
            Assert.IsNotNull(contributor);
            Assert.IsTrue(user.ContributorsEnabled);
            Assert.IsFalse(user.IsTranslator);
            Assert.AreEqual("E8F2F7", user.ProfileBackgroundColor);
            Assert.AreEqual("http://a0.twimg.com/profile_background_images/229557229/twitterapi-bg.png", user.ProfileBackgroundImageUrl);
            Assert.AreEqual("https://si0.twimg.com/profile_background_images/229557229/twitterapi-bg.png", user.ProfileBackgroundImageUrlHttps);
            Assert.IsFalse(user.ProfileBackgroundTile);
            Assert.AreEqual("http://a0.twimg.com/profile_images/1438634086/avatar_normal.png", user.ProfileImageUrl);
            Assert.AreEqual("https://si0.twimg.com/profile_images/1438634086/avatar_normal.png", user.ProfileImageUrlHttps);
            Assert.AreEqual("0094C2", user.ProfileLinkColor);
            Assert.AreEqual("0094C2", user.ProfileSidebarBorderColor);
            Assert.AreEqual("A9D9F1", user.ProfileSidebarFillColor);
            Assert.AreEqual("437792", user.ProfileTextColor);
            Assert.IsTrue(user.ProfileUseBackgroundImage);
            Assert.IsFalse(user.ShowAllInlineMedia);
            Assert.IsFalse(user.DefaultProfile);
            Assert.IsFalse(user.DefaultProfileImage);
            Assert.IsFalse(user.Following);
            Assert.IsFalse(user.FollowRequestSent);
            Assert.IsFalse(user.Notifications);
        }
        
        [TestMethod]
        public void ProcessActionResult_Parses_SingleUser_Response()
        {
            var reqProc = new UserRequestProcessor<User>();

            User user = reqProc.ProcessActionResult(SingleUserResponse, UserAction.SingleUser);

            VerifySingleUserResponse(user);
        }

        [TestMethod]
        public void ProcessResults_Parses_Show_Response()
        {
            var reqProc = new UserRequestProcessor<User> { Type = UserType.Show, BaseUrl = "https://api.twitter.com/1.1/" };

            List<User> users = reqProc.ProcessResults(SingleUserResponse);

            Assert.IsNotNull(users);
            Assert.AreEqual(1, users.Count);
            User user = users.First();
            VerifySingleUserResponse(user);
        }
  
        [TestMethod]
        public void ProcessResults_Parses_Categories_Response()
        {
            var reqProc = new UserRequestProcessor<User> { Type = UserType.Categories, BaseUrl = "http://api.twitter.com/1.1/" };

            List<User> userList = reqProc.ProcessResults(CategoriesResponse);

            Assert.IsNotNull(userList);
            Assert.AreEqual(1, userList.Count);
            User user = userList.Single();
            Assert.IsNotNull(user);
            List<Category> categories = user.Categories;
            Assert.IsNotNull(categories);
            Assert.IsTrue(categories.Any());
            Category category = categories.First();
            Assert.IsNotNull(category);
            Assert.AreEqual(106, category.Size);
            Assert.AreEqual("Music", category.Name);
            Assert.AreEqual("music", category.Slug);
        }

        [TestMethod]
        public void ProcessResults_Parses_Category_Response()
        {
            var reqProc = new UserRequestProcessor<User> { Type = UserType.Category, BaseUrl = "http://api.twitter.com/1.1/" };

            List<User> userList = reqProc.ProcessResults(CategoryResponse);

            Assert.IsNotNull(userList);
            Assert.AreEqual(1, userList.Count);
            User user = userList.Single();
            Assert.IsNotNull(user);
            List<Category> categories = user.Categories;
            Assert.IsNotNull(categories);
            Assert.IsTrue(categories.Any());
            Category category = categories.First();
            Assert.IsNotNull(category);
            Assert.AreEqual(64, category.Size);
            Assert.AreEqual("Funny", category.Name);
            Assert.AreEqual("funny", category.Slug);
            List<User> users = category.Users;
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());
            User catUser = users.First();
            Assert.IsNotNull(catUser);
            Assert.AreEqual("OMG TestMethods", catUser.Name);
        }

        void TestMultipleUserResponse(UserType type)
        {
            var reqProc = new UserRequestProcessor<User> { Type = type, BaseUrl = "http://api.twitter.com/1.1/" };

            List<User> userList = reqProc.ProcessResults(MultipleUserResponse);

            Assert.IsNotNull(userList);
            Assert.IsTrue(userList.Any());
            User user = userList.First();
            Assert.IsNotNull(user);
            Assert.AreEqual("bbccff", user.ProfileSidebarBorderColor);
        }

        [TestMethod]
        public void ProcessResults_Parses_CategoryStatus_Response()
        {
            TestMultipleUserResponse(UserType.CategoryStatus);
        }

        [TestMethod]
        public void ProcessResults_Parses_Lookup_Response()
        {
            TestMultipleUserResponse(UserType.Lookup);
        }

        [TestMethod]
        public void ProcessResults_Parses_Search_Response()
        {
            TestMultipleUserResponse(UserType.Search);
        }

        [TestMethod]
        public void ProcessResults_Parses_Contributee_Response()
        {
            TestMultipleUserResponse(UserType.Contributees);
        }

        [TestMethod]
        public void ProcessResults_Parses_Contributor_Response()
        {
            TestMultipleUserResponse(UserType.Contributors);
        }

        [TestMethod]
        public void ProcessResults_Parses_BannerSizes_Response()
        {
            var reqProc = new UserRequestProcessor<User> { Type = UserType.BannerSizes, BaseUrl = "http://api.twitter.com/1.1/" };

            List<User> userList = reqProc.ProcessResults(BannerSizesResponse);

            Assert.IsNotNull(userList);
            Assert.IsTrue(userList.Any());
            Assert.AreEqual(1, userList.Count);
            User user = userList.Single();
            Assert.IsNotNull(user);
            List<BannerSize> bannerSizes = user.BannerSizes;
            Assert.IsNotNull(bannerSizes);
            Assert.AreEqual(6, bannerSizes.Count);
            BannerSize firstSize = bannerSizes.First();
            Assert.IsNotNull(firstSize);
            Assert.AreEqual("ipad_retina", firstSize.Label);
            Assert.AreEqual(1252, firstSize.Width);
            Assert.AreEqual(626, firstSize.Height);
            Assert.AreEqual("https://si0.twimg.com/profile_banners/16761255/1355801341/ipad_retina", firstSize.Url);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var userProc = new UserRequestProcessor<User> { BaseUrl = "http://api.twitter.com/1.1/" };

            List<User> users = userProc.ProcessResults(string.Empty);

            Assert.IsFalse(users.Any());
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

        const string CategoriesResponse = @"[
   {
      ""size"":106,
      ""name"":""Music"",
      ""slug"":""music""
   },
   {
      ""size"":78,
      ""name"":""Sports"",
      ""slug"":""sports""
   },
   {
      ""size"":79,
      ""name"":""Entertainment"",
      ""slug"":""entertainment""
   }
]";

        const string CategoryResponse = @"{
   ""size"":64,
   ""name"":""Funny"",
   ""users"":[
      {
         ""id"":77888423,
         ""geo_enabled"":false,
         ""notifications"":false,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/360808368\/aquarium.jpg"",
         ""is_translator"":false,
         ""show_all_inline_media"":false,
         ""url"":""http:\/\/omg-TestMethods.com"",
         ""follow_request_sent"":false,
         ""profile_link_color"":""006da8"",
         ""statuses_count"":9494,
         ""created_at"":""Mon Sep 28 01:28:23 +0000 2009"",
         ""lang"":""en"",
         ""utc_offset"":-21600,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1818054089\/OMGWhite200frames_normal.gif"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/360808368\/aquarium.jpg"",
         ""friends_count"":7,
         ""name"":""OMG TestMethods"",
         ""default_profile_image"":false,
         ""profile_use_background_image"":false,
         ""profile_text_color"":""332d2d"",
         ""favourites_count"":0,
         ""screen_name"":""OMGTestMethods"",
         ""protected"":false,
         ""location"":""Chicago, Illinois"",
         ""verified"":false,
         ""profile_sidebar_border_color"":""ffffff"",
         ""id_str"":""77888423"",
         ""following"":false,
         ""time_zone"":""Central Time (US & Canada)"",
         ""description"":""The #1 TestMethod Site. For more TestMethods, follow:\r\n @OMGTestMethodsSex @OMGTestMethodsCelebs @OMGTestMethodsAnimals @OMGTestMethodsSports"",
         ""profile_background_tile"":false,
         ""default_profile"":false,
         ""profile_sidebar_fill_color"":""f2f2f2"",
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1818054089\/OMGWhite200frames_normal.gif"",
         ""profile_background_color"":""479ec9"",
         ""listed_count"":32263,
         ""contributors_enabled"":false,
         ""followers_count"":4042870
      },
      {
         ""id"":25521487,
         ""geo_enabled"":false,
         ""notifications"":false,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/12054191\/toshbck.jpg"",
         ""is_translator"":false,
         ""show_all_inline_media"":false,
         ""url"":""http:\/\/www.danieltosh.com\/"",
         ""follow_request_sent"":false,
         ""profile_link_color"":""2FC2EF"",
         ""statuses_count"":5219,
         ""created_at"":""Fri Mar 20 15:32:52 +0000 2009"",
         ""lang"":""en"",
         ""utc_offset"":-28800,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/218283715\/Daniel-Tosh---Shot_2-12976_normal.gif"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/12054191\/toshbck.jpg"",
         ""friends_count"":59,
         ""name"":""daniel tosh"",
         ""default_profile_image"":false,
         ""profile_use_background_image"":true,
         ""profile_text_color"":""666666"",
         ""favourites_count"":6,
         ""screen_name"":""danieltosh"",
         ""protected"":false,
         ""location"":""beach"",
         ""verified"":true,
         ""profile_sidebar_border_color"":""181A1E"",
         ""id_str"":""25521487"",
         ""following"":false,
         ""time_zone"":""Pacific Time (US & Canada)"",
         ""description"":""not a doctor"",
         ""profile_background_tile"":true,
         ""default_profile"":false,
         ""profile_sidebar_fill_color"":""252429"",
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/218283715\/Daniel-Tosh---Shot_2-12976_normal.gif"",
         ""profile_background_color"":""1A1B1F"",
         ""listed_count"":33001,
         ""contributors_enabled"":false,
         ""followers_count"":5520666
      },
      {
         ""id"":36686415,
         ""geo_enabled"":false,
         ""notifications"":false,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/12668082\/bg.jpg"",
         ""is_translator"":false,
         ""show_all_inline_media"":true,
         ""url"":""http:\/\/www.zachgalifianakis.com"",
         ""follow_request_sent"":false,
         ""profile_link_color"":""c5bbb5"",
         ""statuses_count"":58,
         ""created_at"":""Thu Apr 30 15:19:13 +0000 2009"",
         ""lang"":""en"",
         ""utc_offset"":-18000,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1741870370\/greg_and_zach_normal.jpg"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/12668082\/bg.jpg"",
         ""friends_count"":0,
         ""name"":""zach galifianakis"",
         ""default_profile_image"":false,
         ""profile_use_background_image"":true,
         ""profile_text_color"":""e29d9d"",
         ""favourites_count"":0,
         ""screen_name"":""galifianakisz"",
         ""protected"":false,
         ""location"":""north carolina"",
         ""verified"":true,
         ""profile_sidebar_border_color"":""3b2511"",
         ""id_str"":""36686415"",
         ""following"":false,
         ""time_zone"":""Quito"",
         ""description"":""OFFICIAL ZACH GALIFIANAKIS"",
         ""profile_background_tile"":true,
         ""default_profile"":false,
         ""profile_sidebar_fill_color"":""864711"",
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1741870370\/greg_and_zach_normal.jpg"",
         ""profile_background_color"":""c9bea1"",
         ""listed_count"":17016,
         ""contributors_enabled"":false,
         ""followers_count"":1818218
      }
   ],
   ""slug"":""funny""
}";

        const string MultipleUserResponse = @"[
   {
      ""id"":20536157,
      ""geo_enabled"":true,
      ""notifications"":false,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/6219441\/bg-google-white-75.gif"",
      ""is_translator"":false,
      ""show_all_inline_media"":true,
      ""url"":""http:\/\/www.google.com\/support\/"",
      ""follow_request_sent"":false,
      ""profile_link_color"":""0000cc"",
      ""statuses_count"":3416,
      ""created_at"":""Tue Feb 10 19:14:39 +0000 2009"",
      ""lang"":""en"",
      ""utc_offset"":-28800,
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/77186109\/favicon_normal.png"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/6219441\/bg-google-white-75.gif"",
      ""friends_count"":366,
      ""name"":""A Googler"",
      ""default_profile_image"":false,
      ""profile_use_background_image"":true,
      ""profile_text_color"":""000000"",
      ""favourites_count"":131,
      ""screen_name"":""google"",
      ""protected"":false,
      ""location"":""Mountain View, CA"",
      ""verified"":true,
      ""profile_sidebar_border_color"":""bbccff"",
      ""id_str"":""20536157"",
      ""following"":false,
      ""time_zone"":""Pacific Time (US & Canada)"",
      ""description"":""News and updates from Google"",
      ""profile_background_tile"":false,
      ""default_profile"":false,
      ""profile_sidebar_fill_color"":""ebeff9"",
      ""status"":{
         ""coordinates"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""favorited"":false,
         ""place"":null,
         ""in_reply_to_status_id"":null,
         ""created_at"":""Mon Apr 30 14:33:09 +0000 2012"",
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""id_str"":""196970449412960257"",
         ""geo"":null,
         ""in_reply_to_user_id"":null,
         ""truncated"":false,
         ""possibly_sensitive_editable"":true,
         ""retweet_count"":126,
         ""source"":""web"",
         ""id"":196970449412960257,
         ""in_reply_to_status_id_str"":null,
         ""retweeted"":false,
         ""text"":""There's lots of data online. We're supporting journalists who research and report on it in innovative ways http:\/\/t.co\/iSNFLbh9""
      },
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/77186109\/favicon_normal.png"",
      ""profile_background_color"":""ffffff"",
      ""listed_count"":76043,
      ""contributors_enabled"":true,
      ""followers_count"":4656496
   },
   {
      ""id"":50393960,
      ""geo_enabled"":false,
      ""notifications"":false,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/47468249\/bgTwitterBack.jpg"",
      ""is_translator"":false,
      ""show_all_inline_media"":false,
      ""url"":""http:\/\/www.thegatesnotes.com"",
      ""follow_request_sent"":false,
      ""profile_link_color"":""0084B4"",
      ""statuses_count"":476,
      ""created_at"":""Wed Jun 24 18:44:10 +0000 2009"",
      ""lang"":""en"",
      ""utc_offset"":-28800,
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1884069342\/BGtwitter_normal.JPG"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/47468249\/bgTwitterBack.jpg"",
      ""friends_count"":107,
      ""name"":""Bill Gates"",
      ""default_profile_image"":false,
      ""profile_use_background_image"":true,
      ""profile_text_color"":""333333"",
      ""favourites_count"":2,
      ""screen_name"":""BillGates"",
      ""protected"":false,
      ""location"":""Seattle, WA"",
      ""verified"":true,
      ""profile_sidebar_border_color"":""C0DEED"",
      ""id_str"":""50393960"",
      ""following"":false,
      ""time_zone"":""Pacific Time (US & Canada)"",
      ""description"":""Sharing things I'm learning through my foundation work and other interests..."",
      ""profile_background_tile"":true,
      ""default_profile"":false,
      ""profile_sidebar_fill_color"":""DDEEF6"",
      ""status"":{
         ""coordinates"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""favorited"":false,
         ""place"":null,
         ""in_reply_to_status_id"":null,
         ""created_at"":""Mon Apr 30 22:39:58 +0000 2012"",
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""id_str"":""197092962461696000"",
         ""geo"":null,
         ""in_reply_to_user_id"":null,
         ""truncated"":false,
         ""possibly_sensitive_editable"":true,
         ""retweet_count"":215,
         ""source"":""web"",
         ""id"":197092962461696000,
         ""in_reply_to_status_id_str"":null,
         ""retweeted"":false,
         ""text"":""You asked \u201chow does your approach compare to 50 yrs of mixed results from NGOs\u201d? #askbillg: http:\/\/t.co\/LVuNXIDy""
      },
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1884069342\/BGtwitter_normal.JPG"",
      ""profile_background_color"":""C0DEED"",
      ""listed_count"":90298,
      ""contributors_enabled"":false,
      ""followers_count"":6266743
   }
]";

        const string BannerSizesResponse = @"{
   ""sizes"":{
      ""ipad_retina"":{
         ""w"":1252,
         ""url"":""https:\/\/si0.twimg.com\/profile_banners\/16761255\/1355801341\/ipad_retina"",
         ""h"":626
      },
      ""mobile"":{
         ""w"":320,
         ""url"":""https:\/\/si0.twimg.com\/profile_banners\/16761255\/1355801341\/mobile"",
         ""h"":160
      },
      ""web"":{
         ""w"":520,
         ""url"":""https:\/\/si0.twimg.com\/profile_banners\/16761255\/1355801341\/web"",
         ""h"":260
      },
      ""web_retina"":{
         ""w"":1040,
         ""url"":""https:\/\/si0.twimg.com\/profile_banners\/16761255\/1355801341\/web_retina"",
         ""h"":520
      },
      ""mobile_retina"":{
         ""w"":640,
         ""url"":""https:\/\/si0.twimg.com\/profile_banners\/16761255\/1355801341\/mobile_retina"",
         ""h"":320
      },
      ""ipad"":{
         ""w"":626,
         ""url"":""https:\/\/si0.twimg.com\/profile_banners\/16761255\/1355801341\/ipad"",
         ""h"":313
      }
   }
}";
    }
}
