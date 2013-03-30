using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.ListTests
{
    public class ListRequestProcessorTests
    {
        public ListRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void GetParameters_Parses_All_Available_Parameters()
        {
            var listReqProc = new ListRequestProcessor<List>();
            Expression<Func<List, bool>> expression =
                list =>
                    list.Type == ListType.Members &&
                    list.UserID == "456" &&
                    list.ScreenName == "JoeMayo" &&
                    list.ListID == "456" &&
                    list.Slug == "test" &&
                    list.OwnerID == "789" &&
                    list.OwnerScreenName == "JoeMayo" &&
                    list.Cursor == "123" &&
                    list.MaxID == 789 &&
                    list.Page == 1 &&
                    list.Count == 10 &&
                    list.SinceID == 123 &&
                    list.FilterToOwnedLists == true &&
                    list.TrimUser == true &&
                    list.IncludeEntities == true &&
                    list.IncludeRetweets == true &&
                    list.SkipStatus == true;

            var queryParams = listReqProc.GetParameters(expression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)ListType.Members).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "456")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ListID", "456")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Slug", "test")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("OwnerID", "789")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("OwnerScreenName", "JoeMayo")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "123")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxID", "789")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "10")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("FilterToOwnedLists", "True")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TrimUser", "True")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeEntities", "True")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeRetweets", "True")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
        }

        [Fact]
        public void BuildUrl_Works_With_Json_Format_Data()
        {
            var listReqProc = new ListRequestProcessor<List>();

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(listReqProc);
        }

        [Fact]
        public void BuildUrl_Creates_Lists_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/list.json?screen_name=JoeMayo";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ListType.Lists).ToString() },
                    { "ScreenName", "JoeMayo" }
                };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            const string ExpectedParam = "Type";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "ScreenName", "JoeMayo" }
                };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParam, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_ScreenName()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", "0" },
                };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Null_Param_List()
        {
            const string ExpectedParamName = "Type";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(null));

            Assert.Equal<string>(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildAllListsUrl_Returns_Url_With_ScreenName()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/all.json?screen_name=JoeMayo";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.All).ToString()},
                {"ScreenName", "JoeMayo"},
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildAllListsUrl_Returns_Url_With_UserID()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/all.json?user_id=123";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.All).ToString(CultureInfo.InvariantCulture)},
                {"UserID", "123"},
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildListUrl_Requires_UserID_Or_ScreenName()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.Lists).ToString()},
                //{"UserID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildListsUrl_Returns_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/list.json?user_id=123&screen_name=JoeMayo&cursor=456";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Lists).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Cursor", "456" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildMembershipsUrl_Requires_UserID_Or_ScreenName()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.Memberships).ToString()},
                //{"UserID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildMembershipsUrl_Returns_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/memberships.json?user_id=123&screen_name=JoeMayo&cursor=456&filter_to_owned_lists=true";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Memberships).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Cursor", "456" },
                { "FilterToOwnedLists", "true" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildMembershipsUrl_Does_Not_Add_False_Filter_To_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/memberships.json?user_id=123&screen_name=JoeMayo&cursor=456";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Memberships).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Cursor", "456" },
                { "FilterToOwnedLists", "false" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildSubscriptionsUrl_Requires_UserID_Or_ScreenName()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.Subscriptions).ToString()},
                //{"UserID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildSubscriptionsUrl_Returns_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/subscriptions.json?user_id=123&screen_name=JoeMayo&count=10&cursor=456";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscriptions).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Count", "10" },
                { "Cursor", "456" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildShowUrl_Requires_ListID_Or_Slug()
        {
            const string ExpecteParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpecteParamName, ex.ParamName);
        }

        [Fact]
        public void BuildShowUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString()},
                { "ListID", null },
                { "Slug", "" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildShowUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            const string ExpectedParamName = "OwnerIdOrOwnerScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString()},
                { "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildShowUrl_Returns_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/show.json?slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString() },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "ListID", "456" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildStatusesUrl_Requires_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildStatusesUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString()},
                { "ListID", "" },
                { "Slug", null }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildStatusesUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            const string ExpectedParamName = "OwnerIdOrOwnerScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString()},
                { "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildStatusesUrl_Returns_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/statuses.json?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&since_id=789&max_id=234&count=25&per_page=25&page=3&trim_user=true&include_entities=true&include_rts=true";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString() },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "ListID", "456" },
                { "SinceID", "789" },
                { "MaxID", "234" },
                { "Count", "25" },
                { "Page", "3" },
                { "TrimUser", "true" },
                { "IncludeEntities", "true" },
                { "IncludeRetweets", "true" },
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildStatusesUrl_Includes_False_Parameters()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/statuses.json?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&since_id=789&max_id=234&count=25&per_page=25&page=3&trim_user=false&include_entities=false&include_rts=false";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString() },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "ListID", "456" },
                { "SinceID", "789" },
                { "MaxID", "234" },
                { "Count", "25" },
                { "Page", "3" },
                { "TrimUser", "false" },
                { "IncludeEntities", "false" },
                { "IncludeRetweets", "false" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildMembersUrl_Requires_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildMembersUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString()},
                { "ListID", "" },
                { "Slug", null }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildMembersUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            const string ExpectedParamName = "OwnerIdOrOwnerScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString()},
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildMembersUrl_Returns_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/members.json?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789&include_entities=true&skip_status=true";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", true.ToString() },
                { "SkipStatus", true.ToString() }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildMembersUrl_Includes_False_Parameters()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/members.json?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789&include_entities=false";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", "false" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildIsMemberUrl_Requires_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString()},
                { "ScreenName", "JoeMayo" },
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildIsMemberUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            const string ExpectedParamName = "OwnerIdOrOwnerScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString()},
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildIsMemberUrl_Requires_UserID_Or_ScreenName()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString()},
                { "Slug", "test" },
                {"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildIsMemberUrl_Returns_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/members/show.json?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456&include_entities=true&skip_status=true";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", true.ToString() },
                { "SkipStatus", true.ToString() }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildIsMemberUrl_Includes_False_Parameters()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/members/show.json?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456&include_entities=false";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", "false" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildSubscribersUrl_Requires_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildSubscribersUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString()},
                { "ListID", "" },
                { "Slug", null }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildSubscribersUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            const string ExpectedParamName = "OwnerIdOrOwnerScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString()},
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Returns_SubscribersUrl()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/subscribers.json?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789&include_entities=true&skip_status=true";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", true.ToString() },
                { "SkipStatus", true.ToString() }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildSubscribersUrl_Includes_False_Parameters()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/subscribers.json?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789&include_entities=false";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", "false" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildIsSubscribedUrl_Requires_ListID_Or_Slug()
        {
            const string ExpectedParamName = "ListIdOrSlug";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString()},
                { "ScreenName", "JoeMayo" },
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildIsSubscriberUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            const string ExpectedParamName = "OwnerIdOrOwnerScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString()},
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildIsSubscriberUrl_Requires_UserID_Or_ScreenName()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString()},
                { "Slug", "test" },
                {"OwnerID", "123"},
            };

            var ex = Assert.Throws<ArgumentException>(() => listReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Returns_IsSubscribedUrl()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/subscribers/show.json?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456&include_entities=true&skip_status=true";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", true.ToString() },
                { "SkipStatus", true.ToString() }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildIsSubscriberUrl_Includes_False_Parameters()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/subscribers/show.json?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456&include_entities=false";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", "false" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Returns_Ownerships_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/lists/ownerships.json?user_id=789&screen_name=JoeMayo&count=10&cursor=1";
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Ownerships).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Count", "10" },
                { "Cursor", "1" }
            };

            Request req = listReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        void TestMultipleListsResponse(ListRequestProcessor<List> listProc)
        {
            var listsResponse = listProc.ProcessResults(MultipleListsResponse);

            var lists = listsResponse as IList<List>;
            Assert.NotNull(lists);
            Assert.Equal(4, lists.Count());
            var list = lists.First();
            Assert.Equal("test", list.Name);
            Assert.Equal("@Linq2Tweeter/test", list.FullName);
            Assert.Equal(1, list.MemberCount);
            Assert.Equal("This is a test2", list.Description);
            Assert.Equal("public", list.Mode);
            Assert.Equal("/Linq2Tweeter/test", list.Uri);
            var users = list.Users;
            Assert.NotNull(users);
            Assert.Single(users);
            Assert.Equal("LINQ to Tweeter", users.First().Name);
            Assert.Equal("44758373", list.ListIDResult);
            Assert.Equal(0, list.SubscriberCount);
            Assert.Equal(new DateTime(2011, 5, 8, 2, 0, 33), list.CreatedAt);
            Assert.Equal(false, list.Following);
            Assert.Equal("test", list.SlugResult);
        }

        [Fact]
        public void ProcessResults_Handles_Lists_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.Lists };

            TestMultipleListsResponse(listProc);
        }

        [Fact]
        public void ProcessResults_Handles_Subscriptions_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.Subscriptions };

            TestMultipleListsResponse(listProc);
        }

        [Fact]
        public void ProcessResults_Handles_Memberships_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.Memberships };

            TestMultipleListsResponse(listProc);
        }

        [Fact]
        public void ProcessResults_Handles_All_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.All };

            TestMultipleListsResponse(listProc);
        }

        [Fact]
        public void ProcessResults_Handles_Show_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.Show };

            var listsResponse = listProc.ProcessResults(SingleListResponse);

            var lists = listsResponse as IList<List>;
            Assert.NotNull(lists);
            Assert.Single(lists);
            var list = lists.Single();
            Assert.Equal("DotNetTwittterDevs", list.Name);
            var users = list.Users;
            Assert.NotNull(users);
            Assert.Single(users);
            Assert.Equal("Joe Mayo", users.Single().Name);
        }
  
        void TestMultipleUsersResponse(ListRequestProcessor<List> listProc)
        {
            var listsResponse = listProc.ProcessResults(MultipleUsersResponse);

            var lists = listsResponse as IList<List>;
            Assert.NotNull(lists);
            Assert.Single(lists);
            var list = lists.Single();
            var users = list.Users;
            Assert.NotNull(users);
            Assert.Equal(3, users.Count);
            Assert.Equal("LINQ to Tweeter Test", users.First().Name);
            var cursor = list.CursorMovement;
            Assert.NotNull(cursor);
            Assert.Equal("1352721896474871923", cursor.Next);
            Assert.Equal("7", cursor.Previous);
        }

        [Fact]
        public void ProcessResults_Handles_Subscribers_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.Subscribers };

            TestMultipleUsersResponse(listProc);
        }

        [Fact]
        public void ProcessResults_Handles_Members_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.Members };

            TestMultipleUsersResponse(listProc);
        }
  
        void TestSingleUserResponse(ListRequestProcessor<List> listProc)
        {
            var listsResponse = listProc.ProcessResults(SingleUserResponse);

            var lists = listsResponse as IList<List>;
            Assert.NotNull(lists);
            Assert.Single(lists);
            var users = lists.Single().Users;
            Assert.NotNull(users);
            Assert.Single(users);
            Assert.Equal("LINQ to Tweeter Test", users.Single().Name);
        }

        [Fact]
        public void ProcessResults_Handles_IsMember_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.IsMember };

            TestSingleUserResponse(listProc);
        }

        [Fact]
        public void ProcessResults_Handles_IsSubscribed_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.IsSubscribed };

            TestSingleUserResponse(listProc);
        }

        [Fact]
        public void ProcessResults_Handles_Statuses_Response()
        {
            var listProc = new ListRequestProcessor<List> { Type = ListType.Statuses };

            var listsResponse = listProc.ProcessResults(ListStatusesResponse);

            var lists = listsResponse as IList<List>;
            Assert.NotNull(lists);
            Assert.Single(lists);
            var statuses = lists.Single().Statuses;
            Assert.NotNull(statuses);
            Assert.Equal(4, statuses.Count);
            Assert.True(statuses.First().Text.StartsWith("so using this approach"));
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var listReqProc = new ListRequestProcessor<List>();

            var results = listReqProc.ProcessResults(string.Empty);

            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void ProcessResults_Retains_Original_Input_Parameters()
        {
            var listProc = new ListRequestProcessor<List> 
            { 
                Type = ListType.Show,
                UserID = "123",
                ScreenName = "JoeMayo",
                Cursor = "456",
                ListID = "789",
                Slug = "MyList",
                OwnerID = "123",
                OwnerScreenName = "JoeMayo",
                MaxID = 150,
                Count = 50,
                Page = 1,
                SinceID = 25,
                TrimUser = true,
                IncludeEntities = true,
                IncludeRetweets = true,
                FilterToOwnedLists = true,
                SkipStatus = true
            };

            var listsResponse = listProc.ProcessResults(SingleListResponse);

            var lists = listsResponse as IList<List>;
            Assert.NotNull(lists);
            Assert.Single(lists);
            var list = lists.Single();
            Assert.Equal(ListType.Show, list.Type);
            Assert.Equal("123", list.UserID);
            Assert.Equal("JoeMayo", list.ScreenName);
            Assert.Equal("456", list.Cursor);
            Assert.Equal("789", list.ListID);
            Assert.Equal("MyList", list.Slug);
            Assert.Equal("123", list.OwnerID);
            Assert.Equal("JoeMayo", list.OwnerScreenName);
            Assert.Equal(150ul, list.MaxID);
            Assert.Equal(50, list.Count);
            Assert.Equal(1, list.Page);
            Assert.Equal(25ul, list.SinceID);
            Assert.True(list.TrimUser);
            Assert.True(list.IncludeEntities);
            Assert.True(list.IncludeRetweets);
            Assert.True(list.FilterToOwnedLists);
            Assert.True(list.SkipStatus);
        }

        const string SingleListResponse = @"{
   ""name"":""DotNetTwittterDevs"",
   ""full_name"":""@JoeMayo\/dotnettwittterdevs"",
   ""member_count"":269,
   ""description"":"".NET Developers who use the Twitter API"",
   ""mode"":""public"",
   ""uri"":""\/JoeMayo\/dotnettwittterdevs"",
   ""user"":{
      ""id"":15411837,
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
      ""url"":""http:\/\/www.mayosoftware.com"",
      ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
      ""followers_count"":1099,
      ""default_profile"":false,
      ""profile_background_color"":""0099B9"",
      ""lang"":""en"",
      ""utc_offset"":-25200,
      ""name"":""Joe Mayo"",
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
      ""location"":""Denver, CO"",
      ""profile_link_color"":""0099B9"",
      ""listed_count"":111,
      ""verified"":false,
      ""protected"":false,
      ""profile_use_background_image"":true,
      ""is_translator"":false,
      ""following"":false,
      ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
      ""profile_text_color"":""3C3940"",
      ""statuses_count"":1905,
      ""screen_name"":""JoeMayo"",
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
      ""time_zone"":""Mountain Time (US & Canada)"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
      ""friends_count"":210,
      ""default_profile_image"":false,
      ""contributors_enabled"":false,
      ""profile_sidebar_border_color"":""5ED4DC"",
      ""id_str"":""15411837"",
      ""geo_enabled"":true,
      ""favourites_count"":41,
      ""profile_background_tile"":false,
      ""notifications"":false,
      ""show_all_inline_media"":false,
      ""profile_sidebar_fill_color"":""95E8EC"",
      ""follow_request_sent"":false
   },
   ""id_str"":""4557337"",
   ""subscriber_count"":34,
   ""created_at"":""Sat Dec 12 22:55:43 +0000 2009"",
   ""following"":false,
   ""slug"":""dotnettwittterdevs"",
   ""id"":4557337
}";

        const string MultipleListsResponse = @"[
   {
      ""uri"":""\/Linq2Tweeter\/test"",
      ""name"":""test"",
      ""full_name"":""@Linq2Tweeter\/test"",
      ""description"":""This is a test2"",
      ""mode"":""public"",
      ""user"":{
         ""id"":16761255,
         ""statuses_count"":109,
         ""contributors_enabled"":false,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0000FF"",
         ""utc_offset"":-25200,
         ""id_str"":""16761255"",
         ""is_translator"":false,
         ""default_profile"":false,
         ""name"":""LINQ to Tweeter"",
         ""show_all_inline_media"":true,
         ""profile_use_background_image"":true,
         ""lang"":""en"",
         ""profile_text_color"":""000000"",
         ""protected"":false,
         ""listed_count"":3,
         ""follow_request_sent"":false,
         ""profile_sidebar_border_color"":""87BC44"",
         ""geo_enabled"":true,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""description"":""Testing the Account Profile Update with LINQ to Twitter."",
         ""notifications"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""profile_background_tile"":false,
         ""following"":false,
         ""followers_count"":26,
         ""screen_name"":""Linq2Tweeter"",
         ""profile_sidebar_fill_color"":""E0FF92"",
         ""friends_count"":6,
         ""url"":""http:\/\/linqtotwitter.codeplex.com"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""default_profile_image"":false,
         ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
         ""favourites_count"":2,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""verified"":false,
         ""profile_background_color"":""9AE4E8""
      },
      ""following"":false,
      ""id_str"":""44758373"",
      ""member_count"":1,
      ""created_at"":""Sun May 08 02:00:33 +0000 2011"",
      ""id"":44758373,
      ""subscriber_count"":0,
      ""slug"":""test""
   },
   {
      ""uri"":""\/Linq2Tweeter\/privatelist-4"",
      ""name"":""Privatelist"",
      ""full_name"":""@Linq2Tweeter\/privatelist-4"",
      ""description"":""This is a private list for testing."",
      ""mode"":""private"",
      ""user"":{
         ""id"":16761255,
         ""statuses_count"":109,
         ""contributors_enabled"":false,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0000FF"",
         ""utc_offset"":-25200,
         ""id_str"":""16761255"",
         ""is_translator"":false,
         ""default_profile"":false,
         ""name"":""LINQ to Tweeter"",
         ""show_all_inline_media"":true,
         ""profile_use_background_image"":true,
         ""lang"":""en"",
         ""profile_text_color"":""000000"",
         ""protected"":false,
         ""listed_count"":3,
         ""follow_request_sent"":false,
         ""profile_sidebar_border_color"":""87BC44"",
         ""geo_enabled"":true,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""description"":""Testing the Account Profile Update with LINQ to Twitter."",
         ""notifications"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""profile_background_tile"":false,
         ""following"":false,
         ""followers_count"":26,
         ""screen_name"":""Linq2Tweeter"",
         ""profile_sidebar_fill_color"":""E0FF92"",
         ""friends_count"":6,
         ""url"":""http:\/\/linqtotwitter.codeplex.com"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""default_profile_image"":false,
         ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
         ""favourites_count"":2,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""verified"":false,
         ""profile_background_color"":""9AE4E8""
      },
      ""following"":false,
      ""id_str"":""3897042"",
      ""member_count"":0,
      ""created_at"":""Fri Nov 27 01:42:12 +0000 2009"",
      ""id"":3897042,
      ""subscriber_count"":0,
      ""slug"":""privatelist-4""
   },
   {
      ""uri"":""\/Linq2Tweeter\/mvc-4"",
      ""name"":""MVC"",
      ""full_name"":""@Linq2Tweeter\/mvc-4"",
      ""description"":""Developers Interested in ASP.NET MVC"",
      ""mode"":""public"",
      ""user"":{
         ""id"":16761255,
         ""statuses_count"":109,
         ""contributors_enabled"":false,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0000FF"",
         ""utc_offset"":-25200,
         ""id_str"":""16761255"",
         ""is_translator"":false,
         ""default_profile"":false,
         ""name"":""LINQ to Tweeter"",
         ""show_all_inline_media"":true,
         ""profile_use_background_image"":true,
         ""lang"":""en"",
         ""profile_text_color"":""000000"",
         ""protected"":false,
         ""listed_count"":3,
         ""follow_request_sent"":false,
         ""profile_sidebar_border_color"":""87BC44"",
         ""geo_enabled"":true,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""description"":""Testing the Account Profile Update with LINQ to Twitter."",
         ""notifications"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""profile_background_tile"":false,
         ""following"":false,
         ""followers_count"":26,
         ""screen_name"":""Linq2Tweeter"",
         ""profile_sidebar_fill_color"":""E0FF92"",
         ""friends_count"":6,
         ""url"":""http:\/\/linqtotwitter.codeplex.com"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""default_profile_image"":false,
         ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
         ""favourites_count"":2,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""verified"":false,
         ""profile_background_color"":""9AE4E8""
      },
      ""following"":false,
      ""id_str"":""3897016"",
      ""member_count"":0,
      ""created_at"":""Fri Nov 27 01:41:12 +0000 2009"",
      ""id"":3897016,
      ""subscriber_count"":0,
      ""slug"":""mvc-4""
   },
   {
      ""uri"":""\/Linq2Tweeter\/linq"",
      ""name"":""LINQ"",
      ""full_name"":""@Linq2Tweeter\/linq"",
      ""description"":""People who specialize in LINQ"",
      ""mode"":""public"",
      ""user"":{
         ""id"":16761255,
         ""statuses_count"":109,
         ""contributors_enabled"":false,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0000FF"",
         ""utc_offset"":-25200,
         ""id_str"":""16761255"",
         ""is_translator"":false,
         ""default_profile"":false,
         ""name"":""LINQ to Tweeter"",
         ""show_all_inline_media"":true,
         ""profile_use_background_image"":true,
         ""lang"":""en"",
         ""profile_text_color"":""000000"",
         ""protected"":false,
         ""listed_count"":3,
         ""follow_request_sent"":false,
         ""profile_sidebar_border_color"":""87BC44"",
         ""geo_enabled"":true,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""description"":""Testing the Account Profile Update with LINQ to Twitter."",
         ""notifications"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/653833321\/200xColor_2.png"",
         ""profile_background_tile"":false,
         ""following"":false,
         ""followers_count"":26,
         ""screen_name"":""Linq2Tweeter"",
         ""profile_sidebar_fill_color"":""E0FF92"",
         ""friends_count"":6,
         ""url"":""http:\/\/linqtotwitter.codeplex.com"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""default_profile_image"":false,
         ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
         ""favourites_count"":2,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2587708056\/200xColor_2_normal.png"",
         ""verified"":false,
         ""profile_background_color"":""9AE4E8""
      },
      ""following"":false,
      ""id_str"":""3897006"",
      ""member_count"":3,
      ""created_at"":""Fri Nov 27 01:40:48 +0000 2009"",
      ""id"":3897006,
      ""subscriber_count"":0,
      ""slug"":""linq""
   }
]";

        const string SingleUserResponse = @"{
   ""id"":16761255,
   ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
   ""url"":""http:\/\/linqtotwitter.codeplex.com"",
   ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
   ""followers_count"":22,
   ""default_profile"":false,
   ""profile_background_color"":""9ae4e8"",
   ""lang"":""en"",
   ""utc_offset"":-25200,
   ""name"":""LINQ to Tweeter Test"",
   ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
   ""location"":""Anywhere In The World"",
   ""profile_link_color"":""0000ff"",
   ""listed_count"":3,
   ""verified"":false,
   ""protected"":false,
   ""profile_use_background_image"":true,
   ""is_translator"":false,
   ""following"":false,
   ""description"":""Testing the LINQ to Twitter Account Profile Update."",
   ""profile_text_color"":""000000"",
   ""statuses_count"":100,
   ""screen_name"":""Linq2Tweeter"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
   ""time_zone"":""Mountain Time (US & Canada)"",
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
   ""friends_count"":1,
   ""default_profile_image"":false,
   ""contributors_enabled"":false,
   ""profile_sidebar_border_color"":""87bc44"",
   ""id_str"":""16761255"",
   ""geo_enabled"":false,
   ""favourites_count"":2,
   ""status"":{
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""contributors"":null,
      ""coordinates"":null,
      ""annotations"":null,
      ""place"":null,
      ""retweet_count"":0,
      ""id_str"":""176445993091481604"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""\u003Ca href=\""http:\/\/www.csharp-station.com\/\"" rel=\""nofollow\""\u003EC# Station\u003C\/a\u003E"",
      ""created_at"":""Sun Mar 04 23:16:17 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":176445993091481604,
      ""geo"":null,
      ""text"":""Windows Phone Test, 03\/04\/2012 16:15:12 #linq2twitter""
   },
   ""profile_background_tile"":false,
   ""notifications"":false,
   ""show_all_inline_media"":true,
   ""profile_sidebar_fill_color"":""e0ff92"",
   ""follow_request_sent"":false
}";

        const string MultipleUsersResponse = @"{
   ""users"":[
      {
         ""id"":16761255,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
         ""url"":""http:\/\/linqtotwitter.codeplex.com"",
         ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
         ""followers_count"":22,
         ""default_profile"":false,
         ""profile_background_color"":""9ae4e8"",
         ""lang"":""en"",
         ""utc_offset"":-25200,
         ""name"":""LINQ to Tweeter Test"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
         ""location"":""Anywhere In The World"",
         ""profile_link_color"":""0000ff"",
         ""listed_count"":3,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":false,
         ""description"":""Testing the LINQ to Twitter Account Profile Update."",
         ""profile_text_color"":""000000"",
         ""statuses_count"":100,
         ""screen_name"":""Linq2Tweeter"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
         ""friends_count"":1,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""87bc44"",
         ""id_str"":""16761255"",
         ""geo_enabled"":false,
         ""favourites_count"":2,
         ""status"":{
            ""annotations"":null,
            ""retweeted"":false,
            ""in_reply_to_screen_name"":null,
            ""contributors"":null,
            ""coordinates"":null,
            ""place"":null,
            ""retweet_count"":0,
            ""id_str"":""176445993091481604"",
            ""in_reply_to_user_id"":null,
            ""favorited"":false,
            ""in_reply_to_status_id_str"":null,
            ""in_reply_to_status_id"":null,
            ""source"":""\u003Ca href=\""http:\/\/www.csharp-station.com\/\"" rel=\""nofollow\""\u003EC# Station\u003C\/a\u003E"",
            ""created_at"":""Sun Mar 04 23:16:17 +0000 2012"",
            ""in_reply_to_user_id_str"":null,
            ""truncated"":false,
            ""id"":176445993091481604,
            ""geo"":null,
            ""text"":""Windows Phone Test, 03\/04\/2012 16:15:12 #linq2twitter""
         },
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":true,
         ""profile_sidebar_fill_color"":""e0ff92"",
         ""follow_request_sent"":false
      },
      {
         ""id"":313139213,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1405826247\/twitter_icon_normal.jpg"",
         ""url"":""http:\/\/wefollow.com\/harithamtech"",
         ""created_at"":""Wed Jun 08 06:12:33 +0000 2011"",
         ""followers_count"":1146,
         ""default_profile"":false,
         ""profile_background_color"":""106100"",
         ""lang"":""en"",
         ""utc_offset"":19800,
         ""name"":""HarithamTechnologies"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/276751844\/twitter_bg.jpg"",
         ""location"":""Coimbatore"",
         ""profile_link_color"":""000000"",
         ""listed_count"":1,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":false,
         ""description"":""Enterprise Application Development | Mobile Apps | SEO | Social Media Marketing | Software Testing | Business Consultancy Services"",
         ""profile_text_color"":""2b2b2b"",
         ""statuses_count"":1751,
         ""screen_name"":""harithamtech"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1405826247\/twitter_icon_normal.jpg"",
         ""time_zone"":""Chennai"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/276751844\/twitter_bg.jpg"",
         ""friends_count"":1903,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""488f3f"",
         ""id_str"":""313139213"",
         ""geo_enabled"":true,
         ""favourites_count"":10,
         ""status"":{
            ""retweeted"":false,
            ""in_reply_to_screen_name"":null,
            ""possibly_sensitive"":false,
            ""contributors"":null,
            ""coordinates"":null,
            ""place"":null,
            ""retweet_count"":0,
            ""id_str"":""196237455870017538"",
            ""in_reply_to_user_id"":null,
            ""favorited"":false,
            ""in_reply_to_status_id_str"":null,
            ""in_reply_to_status_id"":null,
            ""source"":""\u003Ca href=\""http:\/\/www.twimbow.com\"" rel=\""nofollow\""\u003ETwimbow\u003C\/a\u003E"",
            ""created_at"":""Sat Apr 28 14:00:30 +0000 2012"",
            ""in_reply_to_user_id_str"":null,
            ""truncated"":false,
            ""id"":196237455870017538,
            ""geo"":null,
            ""text"":""Global Partners of Haritham Technologies ensures a Win-Win situation #enterprise #marketing #harithamtech #in. http:\/\/t.co\/EdUCAXhY""
         },
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""eeffd9"",
         ""follow_request_sent"":false
      },
      {
         ""id"":266862319,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1620398936\/untitled_normal.JPG"",
         ""url"":null,
         ""created_at"":""Tue Mar 15 23:21:41 +0000 2011"",
         ""followers_count"":15,
         ""default_profile"":false,
         ""profile_background_color"":""181b1c"",
         ""lang"":""en"",
         ""utc_offset"":7200,
         ""name"":""talat taher"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme14\/bg.gif"",
         ""location"":""DOHA,QATAR"",
         ""profile_link_color"":""009999"",
         ""listed_count"":1,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""I'm a Muslim, Software Engineer at ASIS -Qatar"",
         ""profile_text_color"":""fffaff"",
         ""statuses_count"":38,
         ""screen_name"":""TalatTaher"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1620398936\/untitled_normal.JPG"",
         ""time_zone"":""Cairo"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme14\/bg.gif"",
         ""friends_count"":44,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""3d3b3d"",
         ""id_str"":""266862319"",
         ""geo_enabled"":true,
         ""favourites_count"":12,
         ""status"":{
            ""retweeted"":false,
            ""in_reply_to_screen_name"":null,
            ""contributors"":null,
            ""coordinates"":null,
            ""place"":null,
            ""retweet_count"":0,
            ""id_str"":""194760674243645440"",
            ""in_reply_to_user_id"":null,
            ""favorited"":false,
            ""in_reply_to_status_id_str"":null,
            ""in_reply_to_status_id"":null,
            ""source"":""web"",
            ""created_at"":""Tue Apr 24 12:12:17 +0000 2012"",
            ""in_reply_to_user_id_str"":null,
            ""truncated"":false,
            ""id"":194760674243645440,
            ""geo"":null,
            ""text"":""\u0641\u0643\u0631\u0648\u0646\u0649 \u0627\u0632\u0627\u0649 ....\u0647\u0648 \u0627\u0646\u0627 \u0646\u0633\u064a\u062a\u0643\u061f\u061f\u061f""
         },
         ""profile_background_tile"":true,
         ""notifications"":false,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""1a181a"",
         ""follow_request_sent"":false
      }
   ],
   ""next_cursor"":1352721896474871923,
   ""previous_cursor"":7,
   ""next_cursor_str"":""1352721896474871923"",
   ""previous_cursor_str"":""7""
}";

        const string ListStatusesResponse = @"[
   {
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":14855950,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1151413227\/MePimping_normal.png"",
         ""url"":""http:\/\/weblogs.sqlteam.com\/mladenp\/"",
         ""created_at"":""Wed May 21 12:55:05 +0000 2008"",
         ""followers_count"":2083,
         ""default_profile"":false,
         ""profile_background_color"":""0060A5"",
         ""lang"":""en"",
         ""utc_offset"":3600,
         ""name"":""Mladen Prajdic"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/4781051\/twitterPic.png"",
         ""location"":""Ljubljana, Slovenia"",
         ""profile_link_color"":""FF9000"",
         ""listed_count"":195,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":false,
         ""description"":""SQL Server MVP and C# developer. Creator of SSMS Tools Pack add-in for SSMS."",
         ""profile_text_color"":""000000"",
         ""statuses_count"":39733,
         ""screen_name"":""MladenPrajdic"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1151413227\/MePimping_normal.png"",
         ""time_zone"":""Ljubljana"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/4781051\/twitterPic.png"",
         ""friends_count"":1002,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""FFFFFF"",
         ""id_str"":""14855950"",
         ""geo_enabled"":false,
         ""favourites_count"":32,
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":true,
         ""profile_sidebar_fill_color"":""0060A5"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":0,
      ""id_str"":""196286812220690433"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""annotations"":null,
      ""source"":""\u003Ca href=\""http:\/\/www.tweetdeck.com\"" rel=\""nofollow\""\u003ETweetDeck\u003C\/a\u003E"",
      ""created_at"":""Sat Apr 28 17:16:37 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":196286812220690433,
      ""geo"":null,
      ""text"":""so using this approach i don't even need to sign my assemblies. well that lessens the complexity a bit. yay!""
   },
   {
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":95212023,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/571849590\/minha_foto_normal.jpg"",
         ""url"":""http:\/\/www.globalcode.com.br"",
         ""created_at"":""Mon Dec 07 14:40:21 +0000 2009"",
         ""followers_count"":2208,
         ""default_profile"":true,
         ""profile_background_color"":""C0DEED"",
         ""lang"":""pt"",
         ""utc_offset"":-10800,
         ""name"":""Vinicius"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""location"":""Ubatuba - SP"",
         ""profile_link_color"":""0084B4"",
         ""listed_count"":98,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":false,
         ""description"":""Programar, educar, criar os filhos, cozinhar, surfar, velejar e claro, beber. Simples n\u00e3o? Ah, aqui eu sou eu, n\u00e3o globalcode ok?"",
         ""profile_text_color"":""333333"",
         ""statuses_count"":8628,
         ""screen_name"":""vsenger"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/571849590\/minha_foto_normal.jpg"",
         ""time_zone"":""Greenland"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""friends_count"":369,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""id_str"":""95212023"",
         ""geo_enabled"":false,
         ""favourites_count"":41,
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":0,
      ""id_str"":""196286622063525889"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""annotations"":null,
      ""source"":""\u003Ca href=\""http:\/\/www.tweetdeck.com\"" rel=\""nofollow\""\u003ETweetDeck\u003C\/a\u003E"",
      ""created_at"":""Sat Apr 28 17:15:52 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":196286622063525889,
      ""geo"":null,
      ""text"":""Tudo bem n\u00e3o ter anvisa aqui no aeroporto, mas ent\u00e3o coloca um bom rod\u00edzio de carnes vai...""
   },
   {
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":6194482,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1618873297\/iPhone_Pictures_524_normal.jpg"",
         ""url"":""http:\/\/techpreacher.corti.com"",
         ""created_at"":""Mon May 21 08:57:50 +0000 2007"",
         ""followers_count"":815,
         ""default_profile"":false,
         ""profile_background_color"":""a6cce6"",
         ""lang"":""en"",
         ""utc_offset"":3600,
         ""name"":""Sascha Corti"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/242394801\/TwitterBackground2.png"",
         ""location"":""47.580262,-122.135105"",
         ""profile_link_color"":""0084B4"",
         ""listed_count"":47,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":false,
         ""description"":""Developer evangelist for Microsoft in Switzerland. Focus on web 2.0 technologies, Windows Phone 7 development. Passionate gamer with a life."",
         ""profile_text_color"":""333333"",
         ""statuses_count"":4293,
         ""screen_name"":""TechPreacher"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1618873297\/iPhone_Pictures_524_normal.jpg"",
         ""time_zone"":""Bern"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/242394801\/TwitterBackground2.png"",
         ""friends_count"":517,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""id_str"":""6194482"",
         ""geo_enabled"":true,
         ""favourites_count"":37,
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":0,
      ""id_str"":""196286470443642880"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""annotations"":null,
      ""source"":""\u003Ca href=\""http:\/\/raptr.com\/\"" rel=\""nofollow\""\u003ERaptr\u003C\/a\u003E"",
      ""created_at"":""Sat Apr 28 17:15:16 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":196286470443642880,
      ""geo"":null,
      ""text"":""I unlocked the Get a cube achievement on Fez! http:\/\/t.co\/Hqhl5oix""
   },
   {
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""contributors"":null,
      ""coordinates"":{
         ""type"":""Point"",
         ""coordinates"":[
            -41.9644758,
            -18.8669036
         ]
      },
      ""place"":{
         ""name"":""Governador Valadares"",
         ""country"":""Brasil"",
         ""attributes"":{

         },
         ""full_name"":""Governador Valadares, Minas Gerais"",
         ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/2fdc3603bc0c1d1d.json"",
         ""bounding_box"":{
            ""type"":""Polygon"",
            ""coordinates"":[
               [
                  [
                     -42.339393,
                     -19.039766
                  ],
                  [
                     -41.539846,
                     -19.039766
                  ],
                  [
                     -41.539846,
                     -18.5511242
                  ],
                  [
                     -42.339393,
                     -18.5511242
                  ]
               ]
            ]
         },
         ""country_code"":""BR"",
         ""id"":""2fdc3603bc0c1d1d"",
         ""place_type"":""city""
      },
      ""user"":{
         ""id"":86172114,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2172389365\/gg4QUwKC_normal"",
         ""url"":""http:\/\/janynnegomes.com"",
         ""created_at"":""Thu Oct 29 21:56:49 +0000 2009"",
         ""followers_count"":1741,
         ""default_profile"":false,
         ""profile_background_color"":""642D8B"",
         ""lang"":""en"",
         ""utc_offset"":-10800,
         ""name"":""Janny Gomes"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme10\/bg.gif"",
         ""location"":""Minas Gerais, Brasil"",
         ""profile_link_color"":""FF0000"",
         ""listed_count"":105,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""Android & .NET Developer. My programmer daily is http:\/\/facebook.com\/DiarioDeUmaProgramadorA"",
         ""profile_text_color"":""3D1957"",
         ""statuses_count"":17504,
         ""screen_name"":""devnetgomez"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2172389365\/gg4QUwKC_normal"",
         ""time_zone"":""Greenland"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme10\/bg.gif"",
         ""friends_count"":1208,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""65B0DA"",
         ""id_str"":""86172114"",
         ""geo_enabled"":true,
         ""favourites_count"":88,
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""7AC3EE"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":0,
      ""id_str"":""196284867770716160"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""annotations"":null,
      ""source"":""\u003Ca href=\""http:\/\/twitter.com\/download\/android\"" rel=\""nofollow\""\u003ETwitter for Android\u003C\/a\u003E"",
      ""created_at"":""Sat Apr 28 17:08:53 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":196284867770716160,
      ""geo"":{
         ""type"":""Point"",
         ""coordinates"":[
            -18.8669036,
            -41.9644758
         ]
      },
      ""text"":""A id\u00e9ia \u00e9 que eu aguente segurar at\u00e9 l\u00e1, mas t\u00e1 dificil \""@BrayanCordeiro: @devnetgomez MENTIRA que ele vai nascer no mesmo dia que eu?! &lt;3\""""
   }
]";
    }
}
