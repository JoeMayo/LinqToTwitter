using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.MuteTests
{
    [TestClass]
    public class MuteRequestProcessorTests
    {
        public MuteRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Handles_Input_Params()
        {
            var target = new MuteRequestProcessor<Mute>();
            Expression<Func<Mute, bool>> expression =
                mute =>
                    mute.Type == MuteType.List &&
                    mute.Cursor == 123L &&
                    mute.SkipStatus == true &&
                    mute.IncludeEntities == true;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)MuteType.List).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeEntities", "True")));
        }

        [TestMethod]
        public void BuildUrl_Constructs_IDs_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/mutes/users/ids.json?cursor=123";
            var reqProc = new MuteRequestProcessor<Mute> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)MuteType.IDs).ToString(CultureInfo.InvariantCulture) },
                        { "Cursor", "123" }
                };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_List_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/mutes/users/list.json?cursor=123&include_entities=false&skip_status=true";
            var reqProc = new MuteRequestProcessor<Mute> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)MuteType.List).ToString(CultureInfo.InvariantCulture) },
                        { "Cursor", "123" },
                        { "IncludeEntities", false.ToString() },
                        { "SkipStatus", true.ToString() }
                };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_Parses_IDs_Response()
        {
            var reqProc = new MuteRequestProcessor<Mute> { Type = MuteType.IDs };

            List<Mute> mutes = reqProc.ProcessResults(IdsResponse);

            Assert.IsNotNull(mutes);
            Assert.IsNotNull(mutes.SingleOrDefault());
            var mute = mutes.Single();
            Assert.IsNotNull(mute);
            var idList = mute.IDList;
            Assert.IsNotNull(idList);
            Assert.IsTrue(idList.Any());
            var id = idList.First();
            Assert.AreEqual(5676142ul, id);
            var users = mute.Users;
            Assert.IsNotNull(users);
            Assert.IsFalse(users.Any());
            var cursor = mute.CursorMovement;
            Assert.IsNotNull(cursor);
            Assert.AreEqual(2, cursor.Previous);
            Assert.AreEqual(1, cursor.Next);
        }

        [TestMethod]
        public void ProcessResults_Parses_UsersList_Response()
        {
            var reqProc = new MuteRequestProcessor<Mute> { Type = MuteType.List };

            List<Mute> muteList = reqProc.ProcessResults(UsersResponse);

            Assert.IsNotNull(muteList);
            Assert.IsNotNull(muteList.SingleOrDefault());
            var mute = muteList.Single();
            Assert.IsNotNull(mute);
            var users = mute.Users;
            Assert.IsNotNull(users);
            Assert.AreEqual(3, users.Count);
            var user = users.First();
            Assert.IsNotNull(user);
            Assert.AreEqual("93604443", user.UserIDResponse);
            var ids = mute.IDList;
            Assert.IsNotNull(ids);
            Assert.IsFalse(ids.Any());
            var cursor = mute.CursorMovement;
            Assert.IsNotNull(cursor);
            Assert.AreEqual(2, cursor.Previous);
            Assert.AreEqual(1, cursor.Next);
        }

        const string IdsResponse = @"{
   ""previous_cursor"":2,
   ""next_cursor_str"":""1"",
   ""ids"":[
      5676142
   ],
   ""previous_cursor_str"":""2"",
   ""next_cursor"":1
}";

        const string UsersResponse = @"{
   ""users"":[
      {
         ""id"":93604443,
         ""id_str"":""93604443"",
         ""name"":""Geert van Horrik"",
         ""screen_name"":""GeertvanHorrik"",
         ""location"":""Heeze, The Netherlands"",
         ""description"":""Software Developer and creator of the Catel MVVM framework (see http:\/\/catel.codeplex.com)"",
         ""url"":""http:\/\/blog.catenalogic.com"",
         ""entities"":{
            ""url"":{
               ""urls"":[
                  {
                     ""url"":""http:\/\/blog.catenalogic.com"",
                     ""expanded_url"":null,
                     ""indices"":[
                        0,
                        27
                     ]
                  }
               ]
            },
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""protected"":false,
         ""followers_count"":212,
         ""friends_count"":97,
         ""listed_count"":12,
         ""created_at"":""Mon Nov 30 10:32:19 +0000 2009"",
         ""favourites_count"":5,
         ""utc_offset"":3600,
         ""time_zone"":""Amsterdam"",
         ""geo_enabled"":true,
         ""verified"":false,
         ""statuses_count"":4431,
         ""lang"":""en"",
         ""status"":{
            ""created_at"":""Tue Dec 18 11:10:16 +0000 2012"",
            ""id"":280993376646598658,
            ""id_str"":""280993376646598658"",
            ""text"":""@FransBouma Dank, ik zocht op changes maar moest queue hebben, kan ik weer verder :-)"",
            ""source"":""\u003ca href=\""http:\/\/www.metrotwit.com\/\"" rel=\""nofollow\""\u003eMetroTwit\u003c\/a\u003e"",
            ""truncated"":false,
            ""in_reply_to_status_id"":280993183754768384,
            ""in_reply_to_status_id_str"":""280993183754768384"",
            ""in_reply_to_user_id"":67035608,
            ""in_reply_to_user_id_str"":""67035608"",
            ""in_reply_to_screen_name"":""FransBouma"",
            ""geo"":null,
            ""coordinates"":null,
            ""place"":null,
            ""contributors"":null,
            ""retweet_count"":0,
            ""entities"":{
               ""hashtags"":[

               ],
               ""urls"":[

               ],
               ""user_mentions"":[
                  {
                     ""screen_name"":""FransBouma"",
                     ""name"":""Frans Bouma"",
                     ""id"":67035608,
                     ""id_str"":""67035608"",
                     ""indices"":[
                        0,
                        11
                     ]
                  }
               ]
            },
            ""favorited"":false,
            ""retweeted"":false
         },
         ""contributors_enabled"":false,
         ""is_translator"":false,
         ""profile_background_color"":""C0DEED"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_tile"":false,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1420487736\/Avatar_3_small_normal.png"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1420487736\/Avatar_3_small_normal.png"",
         ""profile_link_color"":""0084B4"",
         ""profile_sidebar_border_color"":""C0DEED"",
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""profile_text_color"":""333333"",
         ""profile_use_background_image"":true,
         ""default_profile"":true,
         ""default_profile_image"":false,
         ""following"":false,
         ""follow_request_sent"":false,
         ""notifications"":false
      },
      {
         ""id"":389487039,
         ""id_str"":""389487039"",
         ""name"":""Elizabeth Hulett"",
         ""screen_name"":""EHulett"",
         ""location"":""Defuniak Springs"",
         ""description"":"""",
         ""url"":null,
         ""entities"":{
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""protected"":false,
         ""followers_count"":4,
         ""friends_count"":10,
         ""listed_count"":0,
         ""created_at"":""Wed Oct 12 14:16:54 +0000 2011"",
         ""favourites_count"":1,
         ""utc_offset"":null,
         ""time_zone"":null,
         ""geo_enabled"":false,
         ""verified"":false,
         ""statuses_count"":3,
         ""lang"":""en"",
         ""status"":{
            ""created_at"":""Mon Dec 17 19:47:23 +0000 2012"",
            ""id"":280761124901765120,
            ""id_str"":""280761124901765120"",
            ""text"":""Can't believe Amanda is 28weeks today.... Lily Rose will be here soon :)"",
            ""source"":""\u003ca href=\""http:\/\/twitter.com\/#!\/download\/ipad\"" rel=\""nofollow\""\u003eTwitter for iPad\u003c\/a\u003e"",
            ""truncated"":false,
            ""in_reply_to_status_id"":null,
            ""in_reply_to_status_id_str"":null,
            ""in_reply_to_user_id"":null,
            ""in_reply_to_user_id_str"":null,
            ""in_reply_to_screen_name"":null,
            ""geo"":null,
            ""coordinates"":null,
            ""place"":null,
            ""contributors"":null,
            ""retweet_count"":0,
            ""entities"":{
               ""hashtags"":[

               ],
               ""urls"":[

               ],
               ""user_mentions"":[

               ]
            },
            ""favorited"":false,
            ""retweeted"":false
         },
         ""contributors_enabled"":false,
         ""is_translator"":false,
         ""profile_background_color"":""C0DEED"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_tile"":false,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1584881066\/Callie_025_normal.JPG"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1584881066\/Callie_025_normal.JPG"",
         ""profile_link_color"":""0084B4"",
         ""profile_sidebar_border_color"":""C0DEED"",
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""profile_text_color"":""333333"",
         ""profile_use_background_image"":true,
         ""default_profile"":true,
         ""default_profile_image"":false,
         ""following"":false,
         ""follow_request_sent"":false,
         ""notifications"":false
      },
      {
         ""id"":608455311,
         ""id_str"":""608455311"",
         ""name"":""\/\/ mellodev"",
         ""screen_name"":""mellodev"",
         ""location"":""Las Vegas, NV"",
         ""description"":"".Net aficionado and Software craftsman. C#, Asp.Net, TSQL, Quartz.Net, WebForms, MVC + WP7. I build Social Office Suite (@SOS) and tweet about #DevLife"",
         ""url"":""http:\/\/stackoverflow.com\/users\/531901\/mello702"",
         ""entities"":{
            ""url"":{
               ""urls"":[
                  {
                     ""url"":""http:\/\/stackoverflow.com\/users\/531901\/mello702"",
                     ""expanded_url"":null,
                     ""indices"":[
                        0,
                        46
                     ]
                  }
               ]
            },
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""protected"":false,
         ""followers_count"":355,
         ""friends_count"":1373,
         ""listed_count"":20,
         ""created_at"":""Thu Jun 14 18:32:56 +0000 2012"",
         ""favourites_count"":109,
         ""utc_offset"":-28800,
         ""time_zone"":""Pacific Time (US & Canada)"",
         ""geo_enabled"":true,
         ""verified"":false,
         ""statuses_count"":1272,
         ""lang"":""en"",
         ""status"":{
            ""created_at"":""Tue Dec 18 21:05:14 +0000 2012"",
            ""id"":281143104046768129,
            ""id_str"":""281143104046768129"",
            ""text"":""@adamralph Nice background btw! Can't wait to go riding this year!"",
            ""source"":""web"",
            ""truncated"":false,
            ""in_reply_to_status_id"":281141527877652480,
            ""in_reply_to_status_id_str"":""281141527877652480"",
            ""in_reply_to_user_id"":5966642,
            ""in_reply_to_user_id_str"":""5966642"",
            ""in_reply_to_screen_name"":""adamralph"",
            ""geo"":null,
            ""coordinates"":null,
            ""place"":null,
            ""contributors"":null,
            ""retweet_count"":0,
            ""entities"":{
               ""hashtags"":[

               ],
               ""urls"":[

               ],
               ""user_mentions"":[
                  {
                     ""screen_name"":""adamralph"",
                     ""name"":""Adam Ralph"",
                     ""id"":5966642,
                     ""id_str"":""5966642"",
                     ""indices"":[
                        0,
                        10
                     ]
                  }
               ]
            },
            ""favorited"":false,
            ""retweeted"":false
         },
         ""contributors_enabled"":false,
         ""is_translator"":false,
         ""profile_background_color"":""191134"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/591395288\/3jgrymfj3hmiokbf3n2i.jpeg"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/591395288\/3jgrymfj3hmiokbf3n2i.jpeg"",
         ""profile_background_tile"":false,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2357024693\/ad1730c5-ad81-4485-af3b-1cb03a7ffacc_normal.png"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2357024693\/ad1730c5-ad81-4485-af3b-1cb03a7ffacc_normal.png"",
         ""profile_banner_url"":""https:\/\/si0.twimg.com\/profile_banners\/608455311\/1348679711"",
         ""profile_link_color"":""0084B4"",
         ""profile_sidebar_border_color"":""FFFFFF"",
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""profile_text_color"":""333333"",
         ""profile_use_background_image"":true,
         ""default_profile"":false,
         ""default_profile_image"":false,
         ""following"":false,
         ""follow_request_sent"":false,
         ""notifications"":false
      }
   ],
   ""next_cursor"":1,
   ""next_cursor_str"":""1"",
   ""previous_cursor"":2,
   ""previous_cursor_str"":""2""
}";
    }
}
