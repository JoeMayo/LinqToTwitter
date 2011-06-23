using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for RelatedResultsRequestProcessorTest and is intended
    ///to contain all RelatedResultsRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RelatedResultsRequestProcessorTest
    {
        private TestContext testContextInstance;

#region Test Data

        private string showResultsXml = @"
<root type=""array"">
  <item type=""object"">
    <results type=""array"">
      <item type=""object"">
        <annotations type=""object"">
          <ConversationRole type=""string"">Fork</ConversationRole>
        </annotations>
        <value type=""object"">
          <text type=""string"">@elylucas grats!</text>
          <truncated type=""boolean"">false</truncated>
          <place type=""null""></place>
          <coordinates type=""null""></coordinates>
          <favorited type=""boolean"">false</favorited>
          <id_str type=""string"">64470111079251969</id_str>
          <annotations type=""null""></annotations>
          <retweet_count type=""number"">0</retweet_count>
          <source type=""string"">&lt;a href=""http://www.tweetdeck.com"" rel=""nofollow""&gt;TweetDeck&lt;/a&gt;</source>
          <created_at type=""string"">Sat Apr 30 23:24:06 +0000 2011</created_at>
          <geo type=""null""></geo>
          <in_reply_to_screen_name type=""string"">elylucas</in_reply_to_screen_name>
          <in_reply_to_status_id_str type=""string"">64464589072498689</in_reply_to_status_id_str>
          <contributors type=""null""></contributors>
          <retweeted type=""boolean"">false</retweeted>
          <in_reply_to_status_id type=""number"">64464589072498689</in_reply_to_status_id>
          <in_reply_to_user_id_str type=""string"">17825699</in_reply_to_user_id_str>
          <in_reply_to_user_id type=""number"">17825699</in_reply_to_user_id>
          <user type=""object"">
            <default_profile_image type=""boolean"">false</default_profile_image>
            <profile_use_background_image type=""boolean"">true</profile_use_background_image>
            <location type=""string"">San Mateo, CA</location>
            <contributors_enabled type=""boolean"">false</contributors_enabled>
            <lang type=""string"">en</lang>
            <profile_background_color type=""string"">022330</profile_background_color>
            <description type=""string"">I am the real Chris Hammond, if you want to know more visit my website.</description>
            <profile_background_image_url type=""string"">http://a2.twimg.com/a/1304019356/images/themes/theme15/bg.png</profile_background_image_url>
            <url type=""string"">http://www.chrishammond.com/</url>
            <show_all_inline_media type=""boolean"">false</show_all_inline_media>
            <follow_request_sent type=""boolean"">false</follow_request_sent>
            <verified type=""boolean"">false</verified>
            <geo_enabled type=""boolean"">true</geo_enabled>
            <id_str type=""string"">15099604</id_str>
            <created_at type=""string"">Thu Jun 12 18:40:56 +0000 2008</created_at>
            <profile_text_color type=""string"">333333</profile_text_color>
            <profile_sidebar_fill_color type=""string"">C0DFEC</profile_sidebar_fill_color>
            <is_translator type=""boolean"">false</is_translator>
            <default_profile type=""boolean"">false</default_profile>
            <statuses_count type=""number"">11979</statuses_count>
            <following type=""boolean"">false</following>
            <profile_background_tile type=""boolean"">false</profile_background_tile>
            <favourites_count type=""number"">3</favourites_count>
            <listed_count type=""number"">90</listed_count>
            <protected type=""boolean"">false</protected>
            <notifications type=""boolean"">false</notifications>
            <time_zone type=""string"">Pacific Time (US &amp; Canada)</time_zone>
            <profile_link_color type=""string"">0084B4</profile_link_color>
            <profile_image_url type=""string"">http://a2.twimg.com/profile_images/1316371595/hammond_3-11_normal.jpg</profile_image_url>
            <name type=""string"">christoc</name>
            <profile_sidebar_border_color type=""string"">a8c7f7</profile_sidebar_border_color>
            <followers_count type=""number"">770</followers_count>
            <id type=""number"">15099604</id>
            <utc_offset type=""number"">-28800</utc_offset>
            <friends_count type=""number"">223</friends_count>
            <screen_name type=""string"">christoc</screen_name>
          </user>
          <id type=""number"">64470111079251969</id>
        </value>
        <score type=""number"">1.0</score>
        <kind type=""string"">Tweet</kind>
      </item>
      <item type=""object"">
        <annotations type=""object"">
          <ConversationRole type=""string"">Fork</ConversationRole>
        </annotations>
        <value type=""object"">
          <text type=""string"">@elylucas Way to go!</text>
          <truncated type=""boolean"">false</truncated>
          <place type=""null""></place>
          <coordinates type=""null""></coordinates>
          <favorited type=""boolean"">false</favorited>
          <id_str type=""string"">64469518730268672</id_str>
          <annotations type=""null""></annotations>
          <retweet_count type=""number"">0</retweet_count>
          <source type=""string"">&lt;a href=""http://www.tweetdeck.com"" rel=""nofollow""&gt;TweetDeck&lt;/a&gt;</source>
          <created_at type=""string"">Sat Apr 30 23:21:45 +0000 2011</created_at>
          <geo type=""null""></geo>
          <in_reply_to_screen_name type=""string"">elylucas</in_reply_to_screen_name>
          <in_reply_to_status_id_str type=""string"">64464589072498689</in_reply_to_status_id_str>
          <contributors type=""null""></contributors>
          <retweeted type=""boolean"">false</retweeted>
          <in_reply_to_status_id type=""number"">64464589072498689</in_reply_to_status_id>
          <in_reply_to_user_id_str type=""string"">17825699</in_reply_to_user_id_str>
          <in_reply_to_user_id type=""number"">17825699</in_reply_to_user_id>
          <user type=""object"">
            <default_profile_image type=""boolean"">false</default_profile_image>
            <profile_use_background_image type=""boolean"">true</profile_use_background_image>
            <location type=""string"">Denver, CO</location>
            <contributors_enabled type=""boolean"">false</contributors_enabled>
            <lang type=""string"">en</lang>
            <profile_background_color type=""string"">eeeeee</profile_background_color>
            <description type=""string"">Husband, father, business owner, .NET developer and consultant</description>
            <profile_background_image_url type=""string"">http://a3.twimg.com/profile_background_images/48461412/Father_s_Day_Walk.jpg</profile_background_image_url>
            <url type=""string"">http://volaresystems.com/blog</url>
            <show_all_inline_media type=""boolean"">false</show_all_inline_media>
            <follow_request_sent type=""boolean"">false</follow_request_sent>
            <verified type=""boolean"">false</verified>
            <geo_enabled type=""boolean"">false</geo_enabled>
            <id_str type=""string"">85729427</id_str>
            <created_at type=""string"">Wed Oct 28 02:55:13 +0000 2009</created_at>
            <profile_text_color type=""string"">666666</profile_text_color>
            <profile_sidebar_fill_color type=""string"">eeeeee</profile_sidebar_fill_color>
            <is_translator type=""boolean"">false</is_translator>
            <default_profile type=""boolean"">false</default_profile>
            <statuses_count type=""number"">632</statuses_count>
            <following type=""boolean"">true</following>
            <profile_background_tile type=""boolean"">false</profile_background_tile>
            <favourites_count type=""number"">0</favourites_count>
            <listed_count type=""number"">14</listed_count>
            <protected type=""boolean"">false</protected>
            <notifications type=""boolean"">false</notifications>
            <time_zone type=""string"">Mountain Time (US &amp; Canada)</time_zone>
            <profile_link_color type=""string"">f59611</profile_link_color>
            <profile_image_url type=""string"">http://a2.twimg.com/profile_images/494009142/Joe_normal.jpg</profile_image_url>
            <name type=""string"">Joe Wilson</name>
            <profile_sidebar_border_color type=""string"">cccccc</profile_sidebar_border_color>
            <followers_count type=""number"">150</followers_count>
            <id type=""number"">85729427</id>
            <utc_offset type=""number"">-25200</utc_offset>
            <friends_count type=""number"">252</friends_count>
            <screen_name type=""string"">joe_in_denver</screen_name>
          </user>
          <id type=""number"">64469518730268672</id>
        </value>
        <score type=""number"">1.0</score>
        <kind type=""string"">Tweet</kind>
      </item>
      <item type=""object"">
        <annotations type=""object"">
          <ConversationRole type=""string"">Fork</ConversationRole>
        </annotations>
        <value type=""object"">
          <text type=""string"">@elylucas Congrats man! Hope it feels good.</text>
          <truncated type=""boolean"">false</truncated>
          <place type=""null""></place>
          <coordinates type=""null""></coordinates>
          <favorited type=""boolean"">false</favorited>
          <id_str type=""string"">64466493332664320</id_str>
          <annotations type=""null""></annotations>
          <retweet_count type=""number"">0</retweet_count>
          <source type=""string"">web</source>
          <created_at type=""string"">Sat Apr 30 23:09:44 +0000 2011</created_at>
          <geo type=""null""></geo>
          <in_reply_to_screen_name type=""string"">elylucas</in_reply_to_screen_name>
          <in_reply_to_status_id_str type=""string"">64464589072498689</in_reply_to_status_id_str>
          <contributors type=""null""></contributors>
          <retweeted type=""boolean"">false</retweeted>
          <in_reply_to_status_id type=""number"">64464589072498689</in_reply_to_status_id>
          <in_reply_to_user_id_str type=""string"">17825699</in_reply_to_user_id_str>
          <in_reply_to_user_id type=""number"">17825699</in_reply_to_user_id>
          <user type=""object"">
            <default_profile_image type=""boolean"">false</default_profile_image>
            <profile_use_background_image type=""boolean"">true</profile_use_background_image>
            <location type=""string""></location>
            <contributors_enabled type=""boolean"">false</contributors_enabled>
            <lang type=""string"">en</lang>
            <profile_background_color type=""string"">1A1B1F</profile_background_color>
            <description type=""string"">.NET Developer with an addiction to internet garbage</description>
            <profile_background_image_url type=""string"">http://a3.twimg.com/a/1304019356/images/themes/theme9/bg.gif</profile_background_image_url>
            <url type=""string"">http://about.me/matthew.bonig</url>
            <show_all_inline_media type=""boolean"">false</show_all_inline_media>
            <follow_request_sent type=""boolean"">false</follow_request_sent>
            <verified type=""boolean"">false</verified>
            <geo_enabled type=""boolean"">false</geo_enabled>
            <id_str type=""string"">14217562</id_str>
            <created_at type=""string"">Tue Mar 25 18:15:01 +0000 2008</created_at>
            <profile_text_color type=""string"">666666</profile_text_color>
            <profile_sidebar_fill_color type=""string"">252429</profile_sidebar_fill_color>
            <is_translator type=""boolean"">false</is_translator>
            <default_profile type=""boolean"">false</default_profile>
            <statuses_count type=""number"">2527</statuses_count>
            <following type=""boolean"">false</following>
            <profile_background_tile type=""boolean"">false</profile_background_tile>
            <favourites_count type=""number"">3</favourites_count>
            <listed_count type=""number"">5</listed_count>
            <protected type=""boolean"">false</protected>
            <notifications type=""boolean"">false</notifications>
            <time_zone type=""string"">Mountain Time (US &amp; Canada)</time_zone>
            <profile_link_color type=""string"">2FC2EF</profile_link_color>
            <profile_image_url type=""string"">http://a1.twimg.com/profile_images/1266428859/android_normal.png</profile_image_url>
            <name type=""string"">Matthew Bonig</name>
            <profile_sidebar_border_color type=""string"">181A1E</profile_sidebar_border_color>
            <followers_count type=""number"">113</followers_count>
            <id type=""number"">14217562</id>
            <utc_offset type=""number"">-25200</utc_offset>
            <friends_count type=""number"">81</friends_count>
            <screen_name type=""string"">whiskeymb</screen_name>
          </user>
          <id type=""number"">64466493332664320</id>
        </value>
        <score type=""number"">1.0</score>
        <kind type=""string"">Tweet</kind>
      </item>
      <item type=""object"">
        <annotations type=""object"">
          <ConversationRole type=""string"">Fork</ConversationRole>
        </annotations>
        <value type=""object"">
          <text type=""string"">@elylucas Congrats!</text>
          <truncated type=""boolean"">false</truncated>
          <place type=""null""></place>
          <coordinates type=""null""></coordinates>
          <favorited type=""boolean"">false</favorited>
          <id_str type=""string"">64465504911376385</id_str>
          <annotations type=""null""></annotations>
          <retweet_count type=""number"">0</retweet_count>
          <source type=""string"">web</source>
          <created_at type=""string"">Sat Apr 30 23:05:48 +0000 2011</created_at>
          <geo type=""null""></geo>
          <in_reply_to_screen_name type=""string"">elylucas</in_reply_to_screen_name>
          <in_reply_to_status_id_str type=""string"">64464589072498689</in_reply_to_status_id_str>
          <contributors type=""null""></contributors>
          <retweeted type=""boolean"">false</retweeted>
          <in_reply_to_status_id type=""number"">64464589072498689</in_reply_to_status_id>
          <in_reply_to_user_id_str type=""string"">17825699</in_reply_to_user_id_str>
          <in_reply_to_user_id type=""number"">17825699</in_reply_to_user_id>
          <user type=""object"">
            <default_profile_image type=""boolean"">false</default_profile_image>
            <profile_use_background_image type=""boolean"">true</profile_use_background_image>
            <location type=""string"">Denver, CO</location>
            <contributors_enabled type=""boolean"">false</contributors_enabled>
            <lang type=""string"">en</lang>
            <profile_background_color type=""string"">0099B9</profile_background_color>
            <description type=""string"">Created LINQ to Twitter, author of 6 .NET books, .NET Consultant, and C# MVP</description>
            <profile_background_image_url type=""string"">http://a0.twimg.com/profile_background_images/13330711/200xColor_2.png</profile_background_image_url>
            <url type=""string"">http://linqtotwitter.codeplex.com/</url>
            <show_all_inline_media type=""boolean"">false</show_all_inline_media>
            <follow_request_sent type=""boolean"">false</follow_request_sent>
            <verified type=""boolean"">false</verified>
            <geo_enabled type=""boolean"">true</geo_enabled>
            <id_str type=""string"">15411837</id_str>
            <created_at type=""string"">Sun Jul 13 04:35:50 +0000 2008</created_at>
            <profile_text_color type=""string"">3C3940</profile_text_color>
            <profile_sidebar_fill_color type=""string"">95E8EC</profile_sidebar_fill_color>
            <is_translator type=""boolean"">false</is_translator>
            <default_profile type=""boolean"">false</default_profile>
            <statuses_count type=""number"">1514</statuses_count>
            <following type=""boolean"">false</following>
            <profile_background_tile type=""boolean"">false</profile_background_tile>
            <favourites_count type=""number"">45</favourites_count>
            <listed_count type=""number"">88</listed_count>
            <protected type=""boolean"">false</protected>
            <notifications type=""boolean"">false</notifications>
            <time_zone type=""string"">Mountain Time (US &amp; Canada)</time_zone>
            <profile_link_color type=""string"">0099B9</profile_link_color>
            <profile_image_url type=""string"">http://a1.twimg.com/profile_images/520626655/JoeTwitterBW_-_150_x_150_normal.jpg</profile_image_url>
            <name type=""string"">Joe Mayo</name>
            <profile_sidebar_border_color type=""string"">5ED4DC</profile_sidebar_border_color>
            <followers_count type=""number"">737</followers_count>
            <id type=""number"">15411837</id>
            <utc_offset type=""number"">-25200</utc_offset>
            <friends_count type=""number"">133</friends_count>
            <screen_name type=""string"">JoeMayo</screen_name>
          </user>
          <id type=""number"">64465504911376385</id>
        </value>
        <score type=""number"">1.0</score>
        <kind type=""string"">Tweet</kind>
      </item>
    </results>
    <resultType type=""string"">Tweet</resultType>
    <groupName type=""string"">TweetsWithConversation</groupName>
    <annotations type=""object"">
      <FromUser type=""string"">elylucas</FromUser>
    </annotations>
    <score type=""number"">1.0</score>
  </item>
</root>";

#endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void GetParameters_Parses_Parameters()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults>();
            Expression<Func<RelatedResults, bool>> expression =
                res =>
                    res.Type == RelatedResultsType.Show &&
                    res.StatusID == 123ul;
            LambdaExpression lambdaExpression = expression;

            Dictionary<string,string> queryParams = relResultsProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)RelatedResultsType.Show).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("StatusID", "123")));
        }

        [TestMethod]
        public void BuildURL_Creates_Show_Url()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults> { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)RelatedResultsType.Show).ToString() },
                    { "StatusID", "123" }
                };
            string expectedUrl = "https://api.twitter.com/1/related_results/show/123.json";

            Request req = relResultsProc.BuildURL(parameters);

            Assert.AreEqual(expectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_Parses_RelatedResults()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults> { BaseUrl = "https://api.twitter.com/1/" };

            List<RelatedResults> results = relResultsProc.ProcessResults(this.showResultsXml);

            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void ProcessResults_Sets_Input_Params()
        {
            var relResultsProc = new RelatedResultsRequestProcessor<RelatedResults> { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)RelatedResultsType.Show).ToString() },
                    { "StatusID", "123" }
                };
            relResultsProc.BuildURL(parameters);

            List<RelatedResults> results = relResultsProc.ProcessResults(this.showResultsXml);

            var result = results.First();

            Assert.AreEqual(RelatedResultsType.Show, result.Type);
            Assert.AreEqual(123ul, result.StatusID);
        }
    }
}
