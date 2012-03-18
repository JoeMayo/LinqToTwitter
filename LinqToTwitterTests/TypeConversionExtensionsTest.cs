using LinqToTwitter.Common;

using LitJson;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace LinqToTwitterTests
{
    [TestClass]
    public class TypeConversionExtensionsTest
    {
        private readonly XNamespace atom = "http://www.w3.org/2005/Atom";
        private readonly XNamespace twitter = "http://api.twitter.com/";

        [TestMethod]
        public void TagValue_Returns_Null_For_Null_Target()
        {
            XName tagName = "a";

            string value = TypeConversionExtensions.TagValue(null, tagName);

            Assert.IsNull(value);
        }

        [TestMethod]
        public void TagValue_Returns_Value()
        {
            XElement resultElem = XElement.Parse(TestQueryResponse);
            XElement elem = resultElem.Element(atom + "entry");
            XName tagName = twitter + "lang";

            string value = elem.TagValue(tagName);

            Assert.AreEqual("en", value);
        }

        [TestMethod]
        public void TagValue_Returns_Null_On_Null_Element()
        {
            XElement resultElem = XElement.Parse(TestQueryResponse);
            XElement elem = resultElem.Element(atom + "entry");
            XName tagName = twitter + "notthere";

            string value = elem.TagValue(tagName);

            Assert.IsNull(value);
        }

        [TestMethod]
        public void TagValue_Accepts_Default_Namespace_Strings_Too()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "text";

            string value = elem.TagValue(tagName);

            Assert.IsTrue(value.StartsWith("RT"));
        }

        [TestMethod]
        public void GetString_Returns_String()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "text";

            string value = elem.GetString(tagName);

            Assert.IsTrue(value.StartsWith("RT"));
        }

        [TestMethod]
        public void GetString_Returns_Default()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_status_id";
            const string defaultVal = "0";

            string value = elem.GetString(tagName, defaultVal);

            Assert.AreEqual(defaultVal, value);
        }

        [TestMethod]
        public void GetBool_Returns_Bool()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "favorited";

            bool value = elem.GetBool(tagName);

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void GetBool_Returns_Default()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "truncated";
            const bool defaultVal = true;

            bool value = elem.GetBool(tagName, defaultVal);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void GetInt_Returns_Int()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_user_id";

            int value = elem.GetInt(tagName);

            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void GetInt_Returns_Default()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_status_id";
            const int defaultVal = 1;

            int value = elem.GetInt(tagName, defaultVal);

            Assert.AreEqual(defaultVal, value);
        }

        [TestMethod]
        public void GetULong_Returns_ULong()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_user_id";

            ulong value = elem.GetULong(tagName);

            Assert.AreEqual(1ul, value);
        }

        [TestMethod]
        public void GetULong_Returns_Default()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_status_id";
            const ulong defaultVal = 1;

            ulong value = elem.GetULong(tagName, defaultVal);

            Assert.AreEqual(defaultVal, value);
        }

        [TestMethod]
        public void GetDouble_Returns_Double()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_user_id";

            double value = elem.GetDouble(tagName);

            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void GetDouble_Returns_Default()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_status_id";
            const int defaultVal = 1;

            double value = elem.GetDouble(tagName, defaultVal);

            Assert.AreEqual(defaultVal, value);
        }

        [TestMethod]
        public void GetDate_Returns_Date()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "created_at";

            DateTime value = elem.GetDate(tagName);

            var expectedDate = new DateTime(2010, 10, 2, 2, 12, 34, DateTimeKind.Utc);
            Assert.AreEqual(expectedDate, value);
        }

        [TestMethod]
        public void GetDate_Returns_Default()
        {
            XElement resultElem = XElement.Parse(TestStatusTweet);
            XElement elem = resultElem.Element("status");
            const string tagName = "in_reply_to_status_id";
            DateTime defaultVal = DateTime.Now;

            DateTime value = elem.GetDate(tagName, defaultVal);

            Assert.AreEqual(defaultVal, value);
        }

        [TestMethod]
        public void GetValue_Returns_Decimal()
        {
            JsonData search = JsonMapper.ToObject(SearchJson);

            var val = search.GetValue<decimal>("completed_in");

            Assert.AreEqual(.057m, val);
        }

        const string TestQueryResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<feed xmlns:google=""http://base.google.com/ns/1.0"" xml:lang=""en-US"" xmlns:openSearch=""http://a9.com/-/spec/opensearch/1.1/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:twitter=""http://api.twitter.com/"">
  <id>tag:search.twitter.com,2005:search/</id>
  <link type=""text/html"" href=""https://search.twitter.com/search?q="" rel=""alternate""/>
  <link type=""application/atom+xml"" href=""https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500km&amp;oauth_consumer_key=7fNSFc8WaIjqghRM5fkzw&amp;oauth_nonce=614273&amp;oauth_signature_method=HMAC-SHA1&amp;oauth_timestamp=1309632860&amp;oauth_token=15411837-wzrBGuT7n2fjDW8aed4qfokPL0y7b4r5cjE0yX7Oo&amp;oauth_verifier=7605900&amp;oauth_version=1.0&amp;oauth_signature=7NFFpuO8bexRC%2bwwlV7abmLOICo%3d"" rel=""self""/>
  <title> - Twitter Search</title>
  <link type=""application/opensearchdescription+xml"" href=""http://search.twitter.com/opensearch.xml"" rel=""search""/>
  <link type=""application/atom+xml"" href=""https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500km&amp;oauth_consumer_key=7fNSFc8WaIjqghRM5fkzw&amp;oauth_nonce=614273&amp;oauth_signature=7NFFpuO8bexRC%2BwwlV7abmLOICo%3D&amp;oauth_signature_method=HMAC-SHA1&amp;oauth_timestamp=1309632860&amp;oauth_token=15411837-wzrBGuT7n2fjDW8aed4qfokPL0y7b4r5cjE0yX7Oo&amp;oauth_verifier=7605900&amp;oauth_version=1.0&amp;since_id=87232168752988160"" rel=""refresh""/>
  <twitter:warning>adjusted since_id to 84682264511922176 (), requested since_id was older than allowedsince_id removed for pagination.</twitter:warning>
  <updated>2011-07-02T18:52:24Z</updated>
  <openSearch:itemsPerPage>15</openSearch:itemsPerPage>
  <link type=""application/atom+xml"" href=""https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500.0km&amp;max_id=87232168752988160&amp;page=2&amp;q="" rel=""next""/>
  <entry>
    <id>tag:search.twitter.com,2005:87232168752988160</id>
    <published>2011-07-02T18:52:24Z</published>
    <link type=""text/html"" href=""https://api.twitter.com/1/amberrmcfly/statuses/87232168752988160"" rel=""alternate""/>
    <title>@SarahNeateX thankyou :) i was gonna do that but i decided to try it just in case :L x</title>
    <content type=""html"">&lt;a href=&quot;https://api.twitter.com/1/SarahNeateX&quot;&gt;@SarahNeateX&lt;/a&gt; thankyou :) i was gonna do that but i decided to try it just in case :L x</content>
    <updated>2011-07-02T18:52:24Z</updated>
    <link type=""image/png"" href=""http://a3.twimg.com/profile_images/1396078312/cam_062_normal.jpg"" rel=""image""/>
    <google:location>Devizes</google:location>
    <twitter:geo>
    </twitter:geo>
    <twitter:metadata>
      <twitter:result_type>recent</twitter:result_type>
    </twitter:metadata>
    <twitter:source>&lt;a href=&quot;https://api.twitter.com/1/&quot;&gt;web&lt;/a&gt;</twitter:source>
    <twitter:lang>en</twitter:lang>
    <author>
      <name>amberrmcfly (Amber Elliott)</name>
      <uri>https://api.twitter.com/1/amberrmcfly</uri>
    </author>
  </entry>
</feed>";

        const string TestStatusTweet = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<statuses type=""array"">
<status>
  <created_at>Sat Oct 02 02:12:34 +0000 2010</created_at>
  <id>26136741809</id>
  <text>RT @peterbromberg: At the end of the day, Microsoft .Net has been very good to me for the last 10 years. Three cheers, Microsoft! Got th ...</text>
  <source>web</source>
  <truncated></truncated>
  <in_reply_to_status_id></in_reply_to_status_id>
  <in_reply_to_user_id>1</in_reply_to_user_id>
  <favorited>false</favorited>
  <in_reply_to_screen_name></in_reply_to_screen_name>
  <retweet_count>100+</retweet_count>
  <retweeted>false</retweeted>
  <retweeted_status>
    <created_at>Sat Oct 02 01:42:05 +0000 2010</created_at>
    <id>26134450146</id>
    <text>At the end of the day, Microsoft .Net has been very good to me for the last 10 years. Three cheers, Microsoft! Got that one right.</text>
    <source>&lt;a href=&quot;http://www.sobees.com&quot; rel=&quot;nofollow&quot;&gt;sobees&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <retweet_count>100+</retweet_count>
    <retweeted>false</retweeted>
    <user>
      <id>10290862</id>
      <name>Peter Bromberg</name>
      <screen_name>peterbromberg</screen_name>
      <location>29.05213,-81.289265</location>
      <description>.NET C# MVP, eggheadcafe.com co-founder, philanthropist, UnEducator, Badass .NET programmer.</description>
      <profile_image_url>http://a2.twimg.com/profile_images/1127116786/warholpetesm4_normal.jpg</profile_image_url>
      <url>http://www.eggheadcafe.com</url>
      <protected>false</protected>
      <followers_count>755</followers_count>
      <profile_background_color>9ae4e8</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>0000ff</profile_link_color>
      <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
      <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
      <friends_count>592</friends_count>
      <created_at>Fri Nov 16 01:55:29 +0000 2007</created_at>
      <favourites_count>9</favourites_count>
      <utc_offset>-18000</utc_offset>
      <time_zone>Eastern Time (US &amp; Canada)</time_zone>
      <profile_background_image_url>http://a3.twimg.com/profile_background_images/159697515/twilk_background3.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <profile_use_background_image>true</profile_use_background_image>
      <notifications>false</notifications>
      <geo_enabled>true</geo_enabled>
      <verified>false</verified>
      <following>false</following>
      <statuses_count>9342</statuses_count>
      <lang>en</lang>
      <contributors_enabled>false</contributors_enabled>
      <follow_request_sent>false</follow_request_sent>
      <listed_count>76</listed_count>
      <show_all_inline_media>true</show_all_inline_media>
    </user>
    <geo/>
    <coordinates/>
    <place/>
    <contributors/>
  </retweeted_status>
  <user>
    <id>15411837</id>
    <name>Joe Mayo</name>
    <screen_name>JoeMayo</screen_name>
    <location>Denver, CO</location>
    <description>Created LINQ to Twitter, author of 6 .NET books, .NET Consultant, and C# MVP</description>
    <profile_image_url>http://a3.twimg.com/profile_images/520626655/JoeTwitterBW_-_150_x_150_normal.jpg</profile_image_url>
    <url>http://linqtotwitter.codeplex.com/</url>
    <protected>false</protected>
    <followers_count>570</followers_count>
    <profile_background_color>0099B9</profile_background_color>
    <profile_text_color>3C3940</profile_text_color>
    <profile_link_color>0099B9</profile_link_color>
    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
    <friends_count>44</friends_count>
    <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
    <favourites_count>92</favourites_count>
    <utc_offset>-25200</utc_offset>
    <time_zone>Mountain Time (US &amp; Canada)</time_zone>
    <profile_background_image_url>http://a3.twimg.com/profile_background_images/13330711/200xColor_2.png</profile_background_image_url>
    <profile_background_tile>false</profile_background_tile>
    <profile_use_background_image>true</profile_use_background_image>
    <notifications>true</notifications>
    <geo_enabled>true</geo_enabled>
    <verified>false</verified>
    <following>true</following>
    <statuses_count>1202</statuses_count>
    <lang>en</lang>
    <contributors_enabled>false</contributors_enabled>
    <follow_request_sent>false</follow_request_sent>
    <listed_count>81</listed_count>
    <show_all_inline_media>false</show_all_inline_media>
  </user>
  <geo/>
  <coordinates/>
  <place/>
  <contributors/>
</status>
</statuses>";

const string SearchJson = @"{
   ""completed_in"":0.057,
   ""max_id"":155786587962224641,
   ""max_id_str"":""155786587962224641"",
   ""next_page"":""?page=2&max_id=155786587962224641&q=blue%20angels&include_entities=1"",
   ""page"":1,
   ""query"":""blue+angels"",
   ""refresh_url"":""?since_id=155786587962224641&q=blue%20angels&include_entities=1"",
   ""results"":[
      {
         ""created_at"":""Sat, 07 Jan 2012 23:03:11 +0000"",
         ""entities"":{
            ""urls"":[
               {
                  ""url"":""http://t.co/xSmFKo5h"",
                  ""expanded_url"":""http://bit.ly/yXkWPy"",
                  ""display_url"":""bit.ly/yXkWPy"",
                  ""indices"":[
                     116,
                     136
                  ]
               }
            ]
         },
         ""from_user"":""LakeMtkaLiberty"",
         ""from_user_id"":15117715,
         ""from_user_id_str"":""15117715"",
         ""from_user_name"":""The Admiral"",
         ""geo"":{
            ""coordinates"":[
               -22.7747,
               -41.9052
            ],
            ""type"":""Point""
         },
         ""id"":155786587962224641,
         ""id_str"":""155786587962224641"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""recent_retweets"":3,         
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a1.twimg.com/profile_images/69013587/small_The_Admiral_normal.jpg"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/69013587/small_The_Admiral_normal.jpg"",
         ""source"":""&lt;a href=&quot;http://twitterfeed.com&quot; rel=&quot;nofollow&quot;&gt;twitterfeed&lt;/a&gt;"",
         ""text"":""Photo of the week: US Navy rescues 18 Iranians from Somali Pirates: Related Posts:Daily Navy Photo: Blue Angels ... http://t.co/xSmFKo5h"",
         ""to_user"":""JoeMayo"",
         ""to_user_id"":123456789,
         ""to_user_id_str"":""123456789"",
         ""to_user_name"":""Joe Mayo""
      },
      {
         ""created_at"":""Sat, 07 Jan 2012 22:27:21 +0000"",
         ""entities"":{
            ""hashtags"":[
               {
                  ""text"":""Presidential"",
                  ""indices"":[
                     0,
                     13
                  ]
               },
               {
                  ""text"":""Newt"",
                  ""indices"":[
                     62,
                     67
                  ]
               }
            ]
         },
         ""from_user"":""cu_mr2ducks"",
         ""from_user_id"":27061351,
         ""from_user_id_str"":""27061351"",
         ""from_user_name"":""cu_mr2ducks"",
         ""geo"":null,
         ""id"":155777569373945856,
         ""id_str"":""155777569373945856"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a3.twimg.com/profile_images/1508391830/IMG_0004_normal.JPG"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/1508391830/IMG_0004_normal.JPG"",
         ""source"":""&lt;a href=&quot;http://twitter.com/#!/download/iphone&quot; rel=&quot;nofollow&quot;&gt;Twitter for iPhone&lt;/a&gt;"",
         ""text"":""#Presidential Race -Intelligence without character is hallow. #Newt multiple affairs. Trust? Even Blue Angels are to be faithful."",
         ""to_user"":null,
         ""to_user_id"":null,
         ""to_user_id_str"":null,
         ""to_user_name"":null
      },
      {
         ""created_at"":""Sat, 07 Jan 2012 21:24:50 +0000"",
         ""entities"":{
            ""user_mentions"":[
               {
                  ""screen_name"":""DesharThomas30"",
                  ""name"":""DeShar Thomas"",
                  ""id"":323629022,
                  ""id_str"":""323629022"",
                  ""indices"":[
                     0,
                     15
                  ]
               }
            ]
         },
         ""from_user"":""OurtneyLamie"",
         ""from_user_id"":280234351,
         ""from_user_id_str"":""280234351"",
         ""from_user_name"":""Court Lamie"",
         ""geo"":null,
         ""id"":155761836736786432,
         ""id_str"":""155761836736786432"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a2.twimg.com/profile_images/1730998374/image_normal.jpg"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/1730998374/image_normal.jpg"",
         ""source"":""&lt;a href=&quot;http://twitter.com/#!/download/iphone&quot; rel=&quot;nofollow&quot;&gt;Twitter for iPhone&lt;/a&gt;"",
         ""text"":""@DesharThomas30 Ohhh haha Blue devils are mean because they are devils. And umm UOFM are nice like angels (:"",
         ""to_user"":""DesharThomas30"",
         ""to_user_id"":323629022,
         ""to_user_id_str"":""323629022"",
         ""to_user_name"":""DeShar Thomas"",
         ""in_reply_to_status_id"":155761481038835713,
         ""in_reply_to_status_id_str"":""155761481038835713""
      },
      {
         ""created_at"":""Sat, 07 Jan 2012 18:42:36 +0000"",
         ""entities"":{
            ""user_mentions"":[
               {
                  ""screen_name"":""rschu"",
                  ""name"":""Ren\u00e9 Schulte"",
                  ""id"":18668342,
                  ""id_str"":""18668342"",
                  ""indices"":[
                     3,
                     9
                  ]
               },
               {
                  ""screen_name"":""PicturesLab"",
                  ""name"":""Pictures Lab"",
                  ""id"":195138719,
                  ""id_str"":""195138719"",
                  ""indices"":[
                     37,
                     49
                  ]
               }
            ],
            ""media"":[
               {
                  ""id"":155683816676134913,
                  ""id_str"":""155683816676134913"",
                  ""indices"":[
                     59,
                     79
                  ],
                  ""media_url"":""http://p.twimg.com/AikZmz5CEAESBHD.jpg"",
                  ""media_url_https"":""https://p.twimg.com/AikZmz5CEAESBHD.jpg"",
                  ""url"":""http://t.co/36MZIOyW"",
                  ""display_url"":""pic.twitter.com/36MZIOyW"",
                  ""expanded_url"":""http://twitter.com/rschu/status/155683816671940609/photo/1"",
                  ""type"":""photo"",
                  ""sizes"":{
                     ""orig"":{
                        ""w"":1161,
                        ""h"":925,
                        ""resize"":""fit""
                     },
                     ""thumb"":{
                        ""w"":150,
                        ""h"":150,
                        ""resize"":""crop""
                     },
                     ""large"":{
                        ""w"":1024,
                        ""h"":816,
                        ""resize"":""fit""
                     },
                     ""small"":{
                        ""w"":340,
                        ""h"":271,
                        ""resize"":""fit""
                     },
                     ""medium"":{
                        ""w"":600,
                        ""h"":478,
                        ""resize"":""fit""
                     }
                  }
               }
            ]
         },
         ""from_user"":""PicturesLab"",
         ""from_user_id"":195138719,
         ""from_user_id_str"":""195138719"",
         ""from_user_name"":""Pictures Lab"",
         ""geo"":null,
         ""id"":155721009704599552,
         ""id_str"":""155721009704599552"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a3.twimg.com/profile_images/1138811413/VideoLogo_ohne_Text_400x400_normal.png"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/1138811413/VideoLogo_ohne_Text_400x400_normal.png"",
         ""source"":""&lt;a href=&quot;http://pictureslab.rene-schulte.info&quot; rel=&quot;nofollow&quot;&gt;Pictures Lab&lt;/a&gt;"",
         ""text"":""RT @rschu: Goodbye Hygiene Museum. | @PicturesLab Sepia FX http://t.co/36MZIOyW"",
         ""to_user"":null,
         ""to_user_id"":null,
         ""to_user_id_str"":null,
         ""to_user_name"":null
      }
   ],
   ""results_per_page"":15,
   ""since_id"":3,
   ""since_id_str"":""3""
}";
    }
}
