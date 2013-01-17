using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.RawTests
{
    public class RawRequestProcessorTests
    {
        public RawRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void GetParameters_Returns_Parameters()
        {
            var rawReqProc = new RawRequestProcessor<Raw>();
            Expression<Func<Raw, bool>> expression =
                raw =>
                    raw.QueryString == "/statuses/public_timeline.json";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = rawReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("QueryString", "/statuses/public_timeline.json")));
        }

        [Fact]
        public void BuildUrl_Returns_Full_Url()
        {
            var rawReqProc = new RawRequestProcessor<Raw> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"QueryString", "statuses/public_timeline.json"},
             };
            string expected = "https://api.twitter.com/1.1/statuses/public_timeline.json";

            Request req = rawReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Requires_NonNull_QueryString()
        {
            var rawReqProc = new RawRequestProcessor<Raw> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                //{"QueryString", "statuses/public_timeline.json"},
            };

            var ex = Assert.Throws<ArgumentNullException>(() => rawReqProc.BuildUrl(parameters));

            Assert.Equal("QueryString", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Requires_NonBlank_QueryString()
        {
            var rawReqProc = new RawRequestProcessor<Raw> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                {"QueryString", " "},
            };

            var ex = Assert.Throws<ArgumentException>(() => rawReqProc.BuildUrl(parameters));

            Assert.Equal("QueryString", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Resolves_Extra_Slash_Between_BaseUrl_And_QueryString()
        {
            string baseUrlWithTrailingSlash = "https://api.twitter.com/1.1/";
            string queryStringWithPrecedingSlash = "/statuses/public_timeline.json";

            var rawReqProc = new RawRequestProcessor<Raw> { BaseUrl = baseUrlWithTrailingSlash };
            var parameters = new Dictionary<string, string>
            {
                {"QueryString", queryStringWithPrecedingSlash},
            };
            string expected = "https://api.twitter.com/1.1/statuses/public_timeline.json";

            Request req = rawReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Resolves_No_Slash_Between_BaseUrl_And_QueryString()
        {
            string baseUrlWithoutTrailingSlash = "https://api.twitter.com/1.1";
            string queryStringWithoutPrecedingSlash = "statuses/public_timeline.json";

            var rawReqProc = new RawRequestProcessor<Raw> { BaseUrl = baseUrlWithoutTrailingSlash };
            var parameters = new Dictionary<string, string>
            {
                {"QueryString", queryStringWithoutPrecedingSlash},
            };
            string expected = "https://api.twitter.com/1.1/statuses/public_timeline.json";

            Request req = rawReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void ProcessIDResultsTest()
        {
            var rawReqProc = new RawRequestProcessor<Raw>();

            List<Raw> result = rawReqProc.ProcessResults(QueryResponse);

            Assert.Equal(QueryResponse, result.First().Result);
        }

        const string QueryResponse = @"<statuses type=""array"">
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906745</id>
    <text>ah,vou lá comer</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>77880019</id>
      <name>caah </name>
      <screen_name>caahbuss</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://a1.twimg.com/profile_images/440024240/d_normal.JPG</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>48</followers_count>
      <profile_background_color>131516</profile_background_color>
      <profile_text_color>333333</profile_text_color>
      <profile_link_color>009999</profile_link_color>
      <profile_sidebar_fill_color>efefef</profile_sidebar_fill_color>
      <profile_sidebar_border_color>eeeeee</profile_sidebar_border_color>
      <friends_count>47</friends_count>
      <created_at>Mon Sep 28 00:47:48 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset></utc_offset>
      <time_zone></time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme14/bg.gif</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>211</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906740</id>
    <text>É só ir no site e participar... http://tinyurl.com/ygvepg5</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>76732695</id>
      <name>Embarque Imediato</name>
      <screen_name>EmbarqueNoFilme</screen_name>
      <location></location>
      <description>Twitter oficial do filme Embarque Imediato autorizado pela Europa Filmes.</description>
      <profile_image_url>http://a1.twimg.com/profile_images/473272502/poster_embarque_imediato_rostoatores_2_normal.jpg</profile_image_url>
      <url>http://embarqueimediatoofilme.blogspot.com/</url>
      <protected>false</protected>
      <followers_count>401</followers_count>
      <profile_background_color>C0DEED</profile_background_color>
      <profile_text_color>333333</profile_text_color>
      <profile_link_color>0084B4</profile_link_color>
      <profile_sidebar_fill_color>DDEEF6</profile_sidebar_fill_color>
      <profile_sidebar_border_color>C0DEED</profile_sidebar_border_color>
      <friends_count>381</friends_count>
      <created_at>Wed Sep 23 19:33:49 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset>-14400</utc_offset>
      <time_zone>Santiago</time_zone>
      <profile_background_image_url>http://a3.twimg.com/profile_background_images/45672389/twitter08.jpg</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>224</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906722</id>
    <text>Se pá, ir em aniversário rico, onde deve haver muitas etiquetas, e eu odeio, pois gosto de aniversário de pobre onde é churras de havaiana</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>55657026</id>
      <name>bee</name>
      <screen_name>beemk</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://a1.twimg.com/profile_images/463563734/Imagem_004_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>76</followers_count>
      <profile_background_color>1A1B1F</profile_background_color>
      <profile_text_color>666666</profile_text_color>
      <profile_link_color>2FC2EF</profile_link_color>
      <profile_sidebar_fill_color>252429</profile_sidebar_fill_color>
      <profile_sidebar_border_color>181A1E</profile_sidebar_border_color>
      <friends_count>36</friends_count>
      <created_at>Fri Jul 10 20:34:52 +0000 2009</created_at>
      <favourites_count>1</favourites_count>
      <utc_offset>-10800</utc_offset>
      <time_zone>Brasilia</time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme9/bg.gif</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>819</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906718</id>
    <text>@BruBloinski é... digamos que de tradicional só os mesmos shows horríveis de sempre. só.</text>
    <source>&lt;a href=""http://echofon.com/"" rel=""nofollow""&gt;Echofon&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id>6118865157</in_reply_to_status_id>
    <in_reply_to_user_id>60946427</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>BruBloinski</in_reply_to_screen_name>
    <user>
      <id>16219715</id>
      <name>wickybr</name>
      <screen_name>wickybr</screen_name>
      <location></location>
      <description>25 anos, publicidade, cerveja Original. Blogueiro, curioso, as vezes nervoso, nem sempre calmo. Leitor, afinador e desajeitador.</description>
      <profile_image_url>http://a1.twimg.com/profile_images/287234140/fbranco_copy_normal.jpg</profile_image_url>
      <url>http://www.wickybr.blogspot.com</url>
      <protected>false</protected>
      <followers_count>41</followers_count>
      <profile_background_color>642D8B</profile_background_color>
      <profile_text_color>3D1957</profile_text_color>
      <profile_link_color>FF0000</profile_link_color>
      <profile_sidebar_fill_color>7AC3EE</profile_sidebar_fill_color>
      <profile_sidebar_border_color>65B0DA</profile_sidebar_border_color>
      <friends_count>59</friends_count>
      <created_at>Wed Sep 10 11:58:16 +0000 2008</created_at>
      <favourites_count>1</favourites_count>
      <utc_offset>-10800</utc_offset>
      <time_zone>Brasilia</time_zone>
      <profile_background_image_url>http://a3.twimg.com/profile_background_images/28744035/base-back-twitter2.jpg</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>586</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906715</id>
    <text>Завтра ""Саломея"" Виктюка ^^</text>
    <source>&lt;a href=""http://www.tweetdeck.com/"" rel=""nofollow""&gt;TweetDeck&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>45792079</id>
      <name>Нестерова Валерия</name>
      <screen_name>Valeriya22</screen_name>
      <location>Russia, Kazan</location>
      <description>Sunshine Cowboy</description>
      <profile_image_url>http://a3.twimg.com/profile_images/511281997/IMG_6486-_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>30</followers_count>
      <profile_background_color>9AE4E8</profile_background_color>
      <profile_text_color>333333</profile_text_color>
      <profile_link_color>0084B4</profile_link_color>
      <profile_sidebar_fill_color>DDFFCC</profile_sidebar_fill_color>
      <profile_sidebar_border_color>BDDCAD</profile_sidebar_border_color>
      <friends_count>31</friends_count>
      <created_at>Tue Jun 09 07:59:17 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset>-18000</utc_offset>
      <time_zone>Quito</time_zone>
      <profile_background_image_url>http://a1.twimg.com/profile_background_images/17289836/22334353_lll01.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>241</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906713</id>
    <text>Diferente C S!</text>
    <source>&lt;a href=""http://www.myspace.com/sync"" rel=""nofollow""&gt;MySpace&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>83531026</id>
      <name>Abraham</name>
      <screen_name>yosoeelabraham</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://a1.twimg.com/profile_images/479193636/bleach_chad0005_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>0</followers_count>
      <profile_background_color>9ae4e8</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>0000ff</profile_link_color>
      <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
      <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
      <friends_count>0</friends_count>
      <created_at>Mon Oct 19 05:38:34 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset></utc_offset>
      <time_zone></time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme1/bg.png</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>1</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906712</id>
    <text>@REL407 you sound like a Disney original! I was scared for the entire summer!!!! Smdh</text>
    <source>&lt;a href=""http://ubertwitter.com"" rel=""nofollow""&gt;UberTwitter&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id>6118659350</in_reply_to_status_id>
    <in_reply_to_user_id>33132386</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>REL407</in_reply_to_screen_name>
    <user>
      <id>91234221</id>
      <name>Calesha Thompson</name>
      <screen_name>missKILLAmouse</screen_name>
      <location>ÜT: 39.739345,-104.97695</location>
      <description>La plus belle. KILLA. Never change</description>
      <profile_image_url>http://a3.twimg.com/profile_images/544207431/135152_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>13</followers_count>
      <profile_background_color>030303</profile_background_color>
      <profile_text_color>8c878c</profile_text_color>
      <profile_link_color>e30417</profile_link_color>
      <profile_sidebar_fill_color>393f42</profile_sidebar_fill_color>
      <profile_sidebar_border_color>050505</profile_sidebar_border_color>
      <friends_count>18</friends_count>
      <created_at>Fri Nov 20 00:53:08 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset>-18000</utc_offset>
      <time_zone>Eastern Time (US &amp; Canada)</time_zone>
      <profile_background_image_url>http://a1.twimg.com/profile_background_images/56030928/killainstincts.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>108</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906709</id>
    <text>45 I use http://x2t.com/6145 to get 100 followers a day. It work great</text>
    <source>&lt;a href=""http://apiwiki.twitter.com/"" rel=""nofollow""&gt;API&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>50541031</id>
      <name>oaiden;</name>
      <screen_name>helloaiden17</screen_name>
      <location>Philadelphia</location>
      <description>by the looks of my twitter picture, you can call me Lindsey Lohan.</description>
      <profile_image_url>http://a3.twimg.com/profile_images/533420177/151033_normal.jpg</profile_image_url>
      <url>http://www.myspace.com/omgaiden</url>
      <protected>false</protected>
      <followers_count>92</followers_count>
      <profile_background_color>fa3483</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>fa3483</profile_link_color>
      <profile_sidebar_fill_color>ffffff</profile_sidebar_fill_color>
      <profile_sidebar_border_color>000000</profile_sidebar_border_color>
      <friends_count>202</friends_count>
      <created_at>Thu Jun 25 03:45:43 +0000 2009</created_at>
      <favourites_count>4</favourites_count>
      <utc_offset>-18000</utc_offset>
      <time_zone>Quito</time_zone>
      <profile_background_image_url>http://a1.twimg.com/profile_background_images/54654224/1111111111.png</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>706</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906708</id>
    <text>Police parked outside my house #ilovemyneighbourhood</text>
    <source>&lt;a href=""http://echofon.com/"" rel=""nofollow""&gt;Echofon&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>23310378</id>
      <name>Natalie Dye</name>
      <screen_name>natalie_xo</screen_name>
      <location>Yorkshire, its a state of mind</location>
      <description>Listen to the sound of the world then watch it turn.</description>
      <profile_image_url>http://a1.twimg.com/profile_images/525311356/Snapshot_20091026_10_normal.jpg</profile_image_url>
      <url>http://www.facebook.com/natalie0x</url>
      <protected>false</protected>
      <followers_count>102</followers_count>
      <profile_background_color>000000</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>4597d6</profile_link_color>
      <profile_sidebar_fill_color>2b3575</profile_sidebar_fill_color>
      <profile_sidebar_border_color>000000</profile_sidebar_border_color>
      <friends_count>93</friends_count>
      <created_at>Sun Mar 08 14:05:12 +0000 2009</created_at>
      <favourites_count>21</favourites_count>
      <utc_offset></utc_offset>
      <time_zone></time_zone>
      <profile_background_image_url>http://a3.twimg.com/profile_background_images/55410721/Desktop_Background.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>1794</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906705</id>
    <text>@robertabachert ta eu vo tenta adiantar algumas coisas mais eu preciso das fotos</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id>6118781972</in_reply_to_status_id>
    <in_reply_to_user_id>61624985</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>robertabachert</in_reply_to_screen_name>
    <user>
      <id>61279770</id>
      <name>Patrícia Ferrari</name>
      <screen_name>PatyFerrariC</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://a3.twimg.com/profile_images/534284147/patttttttty_normal.png</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>35</followers_count>
      <profile_background_color>000000</profile_background_color>
      <profile_text_color>050505</profile_text_color>
      <profile_link_color>4978d6</profile_link_color>
      <profile_sidebar_fill_color>ffffff</profile_sidebar_fill_color>
      <profile_sidebar_border_color>090a0a</profile_sidebar_border_color>
      <friends_count>51</friends_count>
      <created_at>Wed Jul 29 19:53:07 +0000 2009</created_at>
      <favourites_count>6</favourites_count>
      <utc_offset>-10800</utc_offset>
      <time_zone>Brasilia</time_zone>
      <profile_background_image_url>http://a1.twimg.com/profile_background_images/26221650/16587349.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>518</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906704</id>
    <text>RT @ALuizCosta: Robert Fisk comenta o nada profissional calote de Dubai e suas relações com Abu Dhabi e Índia http://is.gd/54CEg</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <retweeted_status>
      <created_at>Fri Nov 27 12:31:15 +0000 2009</created_at>
      <id>6110610973</id>
      <text>Robert Fisk comenta o nada profissional calote de Dubai e suas relações com Abu Dhabi e Índia http://is.gd/54CEg</text>
      <source>web</source>
      <truncated>false</truncated>
      <in_reply_to_status_id></in_reply_to_status_id>
      <in_reply_to_user_id></in_reply_to_user_id>
      <favorited>false</favorited>
      <in_reply_to_screen_name></in_reply_to_screen_name>
      <user>
        <id>67778641</id>
        <name>AntonioLuiz MCCosta</name>
        <screen_name>ALuizCosta</screen_name>
        <location>São Paulo, Brasil</location>
        <description>Antonio Luiz escreve na revista CartaCapital e gosta de ciência, filosofia e literatura, principalmente fantasia e ficção científica</description>
        <profile_image_url>http://a3.twimg.com/profile_images/375056565/AntonioLuiz_normal.jpg</profile_image_url>
        <url>http://www.scribd.com/people/documents/3817321-antonio-luiz-monteiro-coelho-da-costa</url>
        <protected>false</protected>
        <followers_count>460</followers_count>
        <profile_background_color>9AE4E8</profile_background_color>
        <profile_text_color>333333</profile_text_color>
        <profile_link_color>b30000</profile_link_color>
        <profile_sidebar_fill_color>DDFFCC</profile_sidebar_fill_color>
        <profile_sidebar_border_color>BDDCAD</profile_sidebar_border_color>
        <friends_count>44</friends_count>
        <created_at>Sat Aug 22 01:31:02 +0000 2009</created_at>
        <favourites_count>0</favourites_count>
        <utc_offset>-10800</utc_offset>
        <time_zone>Brasilia</time_zone>
        <profile_background_image_url>http://a1.twimg.com/profile_background_images/31703608/celebraohf2.jpg</profile_background_image_url>
        <profile_background_tile>true</profile_background_tile>
        <statuses_count>1427</statuses_count>
        <notifications>false</notifications>
        <geo_enabled>false</geo_enabled>
        <verified>false</verified>
        <following>false</following>
      </user>
      <geo />
    </retweeted_status>
    <user>
      <id>77265167</id>
      <name>SL da Silva</name>
      <screen_name>sergio_virtual</screen_name>
      <location></location>
      <description>Um brasileiro no mundo do Twitter!</description>
      <profile_image_url>http://a3.twimg.com/profile_images/460588539/DSC00180_normal.JPG</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>20</followers_count>
      <profile_background_color>9ae4e8</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>0000ff</profile_link_color>
      <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
      <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
      <friends_count>36</friends_count>
      <created_at>Fri Sep 25 17:31:11 +0000 2009</created_at>
      <favourites_count>1</favourites_count>
      <utc_offset>-10800</utc_offset>
      <time_zone>Brasilia</time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme1/bg.png</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>514</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906701</id>
    <text>@No1SassyGrl anywhere! I have to take vacay bc I have too many hours accrued at work and I want to travel!</text>
    <source>&lt;a href=""http://echofon.com/"" rel=""nofollow""&gt;Echofon&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id>6113853732</in_reply_to_status_id>
    <in_reply_to_user_id>14408045</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>No1SassyGrl</in_reply_to_screen_name>
    <user>
      <id>9408302</id>
      <name>Ian Mone</name>
      <screen_name>x5455</screen_name>
      <location>Puerto Rico</location>
      <description>Intelligent, laid back, addicted to videogames, X-Men and TV. Hopelessly romantic and hoping for a nice boy to spend time with</description>
      <profile_image_url>http://a1.twimg.com/profile_images/84361594/icon_twitter_normal.jpg</profile_image_url>
      <url>http://x5455.livejournal.com</url>
      <protected>false</protected>
      <followers_count>75</followers_count>
      <profile_background_color>8B542B</profile_background_color>
      <profile_text_color>333333</profile_text_color>
      <profile_link_color>9D582E</profile_link_color>
      <profile_sidebar_fill_color>EADEAA</profile_sidebar_fill_color>
      <profile_sidebar_border_color>D9B17E</profile_sidebar_border_color>
      <friends_count>38</friends_count>
      <created_at>Fri Oct 12 18:26:24 +0000 2007</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset>-21600</utc_offset>
      <time_zone>Central Time (US &amp; Canada)</time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme8/bg.gif</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>1194</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906696</id>
    <text>@nickjonas amor! te hiciste el twitter! jaja el otro día me lo habías dicho XD ahora unite al @teamfasofachero</text>
    <source>&lt;a href=""http://m.twitter.com/"" rel=""nofollow""&gt;mobile web&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id>56783491</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>nickjonas</in_reply_to_screen_name>
    <user>
      <id>37250104</id>
      <name>Milagros C.</name>
      <screen_name>militaaa</screen_name>
      <location>Buenos Aires, Argentina</location>
      <description>This is my crazy world... I'm just being milita. I hope you like my antics and madness, because that's me :)  </description>
      <profile_image_url>http://a3.twimg.com/profile_images/532032207/P1050370_-_copia_normal.JPG</profile_image_url>
      <url>http://www.facebook.com/profile.php?id=1010395037&amp;ref=name</url>
      <protected>false</protected>
      <followers_count>204</followers_count>
      <profile_background_color>050505</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>746278</profile_link_color>
      <profile_sidebar_fill_color>ff088c</profile_sidebar_fill_color>
      <profile_sidebar_border_color>d1bcbc</profile_sidebar_border_color>
      <friends_count>300</friends_count>
      <created_at>Sat May 02 18:17:02 +0000 2009</created_at>
      <favourites_count>21</favourites_count>
      <utc_offset>-10800</utc_offset>
      <time_zone>Buenos Aires</time_zone>
      <profile_background_image_url>http://a3.twimg.com/profile_background_images/54044533/Teen_Vogue_Collage_by_bob55_JOE.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>6718</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906693</id>
    <text>@MrPeterAndre its really good of u to set the record straight on that coz every1 believes everything they read abwt katie n its not fair!x</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id>6106899259</in_reply_to_status_id>
    <in_reply_to_user_id>24086418</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>MrPeterAndre</in_reply_to_screen_name>
    <user>
      <id>54843229</id>
      <name>ashleigh berry</name>
      <screen_name>missashleigh19</screen_name>
      <location>bradford</location>
      <description>waaasssssup :) mobile beauty therapist from bradford, probably should say leeds but that would be lying haha follow me :)</description>
      <profile_image_url>http://a3.twimg.com/profile_images/503338157/halloween_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>7</followers_count>
      <profile_background_color>FF6699</profile_background_color>
      <profile_text_color>362720</profile_text_color>
      <profile_link_color>B40B43</profile_link_color>
      <profile_sidebar_fill_color>E5507E</profile_sidebar_fill_color>
      <profile_sidebar_border_color>CC3366</profile_sidebar_border_color>
      <friends_count>16</friends_count>
      <created_at>Wed Jul 08 08:56:21 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset>0</utc_offset>
      <time_zone>London</time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme11/bg.gif</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>49</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906692</id>
    <text>RT @ArmsLikeYours Chain Reaction with a Skylit Drive Next Friday!!!</text>
    <source>&lt;a href=""http://m.twitter.com/"" rel=""nofollow""&gt;mobile web&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>18696988</id>
      <name>Faith Sugarhigh</name>
      <screen_name>Miss_Sugarhigh</screen_name>
      <location>Texas</location>
      <description>I'm a ProMoTeR/Booking Agent. I love helping bands/artists/Concerts &amp; events that inspire/change the scene. I support Skate4Cancer.</description>
      <profile_image_url>http://a3.twimg.com/profile_images/367961955/pink_pink_003_normal.JPG</profile_image_url>
      <url>http://www.myspace.com/miss_sugarhigh</url>
      <protected>false</protected>
      <followers_count>1172</followers_count>
      <profile_background_color>FF6699</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>373799</profile_link_color>
      <profile_sidebar_fill_color>ff3892</profile_sidebar_fill_color>
      <profile_sidebar_border_color>000000</profile_sidebar_border_color>
      <friends_count>822</friends_count>
      <created_at>Tue Jan 06 21:42:00 +0000 2009</created_at>
      <favourites_count>3</favourites_count>
      <utc_offset>-21600</utc_offset>
      <time_zone>Central Time (US &amp; Canada)</time_zone>
      <profile_background_image_url>http://a3.twimg.com/profile_background_images/30568555/btf_witness_cover400x400.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>11612</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906683</id>
    <text>64 Heyy I just got alot of followers using http://ohurl.com/0G .</text>
    <source>&lt;a href=""http://apiwiki.twitter.com/"" rel=""nofollow""&gt;API&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>61754026</id>
      <name>Samien</name>
      <screen_name>Samien501</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://s.twimg.com/a/1259091217/images/default_profile_5_normal.png</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>382</followers_count>
      <profile_background_color>9ae4e8</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>0000ff</profile_link_color>
      <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
      <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
      <friends_count>1071</friends_count>
      <created_at>Fri Jul 31 11:52:27 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset></utc_offset>
      <time_zone></time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme1/bg.png</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>208</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906674</id>
    <text>@JohnCarnell Thanks for the blog post, looking forward to getting you a proper write up. Have yourself a good evening : )</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id>39247092</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>JohnCarnell</in_reply_to_screen_name>
    <user>
      <id>29426018</id>
      <name>David Wood</name>
      <screen_name>BigDaveSB</screen_name>
      <location>Gloucester</location>
      <description>I’m a serial fundraising; capoeira playing; science loving; hat wearing; skeptically enquiring geek</description>
      <profile_image_url>http://a3.twimg.com/profile_images/359441931/twitterProfilePhoto_normal.jpg</profile_image_url>
      <url>http://justgiving.com/melonandbigdave</url>
      <protected>false</protected>
      <followers_count>177</followers_count>
      <profile_background_color>9ae4e8</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>0000ff</profile_link_color>
      <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
      <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
      <friends_count>161</friends_count>
      <created_at>Tue Apr 07 11:02:36 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset>0</utc_offset>
      <time_zone>London</time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme1/bg.png</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>1335</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906673</id>
    <text>I think it's possible my characters laugh and smile too much. Not that I need to be thinking about such things since I just need to write!</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>14297876</id>
      <name>Misty Baird</name>
      <screen_name>Langwidere</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://a3.twimg.com/profile_images/66698519/Photo_80_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>47</followers_count>
      <profile_background_color>0099B9</profile_background_color>
      <profile_text_color>3C3940</profile_text_color>
      <profile_link_color>0099B9</profile_link_color>
      <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
      <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
      <friends_count>56</friends_count>
      <created_at>Thu Apr 03 23:38:48 +0000 2008</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset>-25200</utc_offset>
      <time_zone>Mountain Time (US &amp; Canada)</time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme4/bg.gif</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>1907</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906670</id>
    <text>@bpradolovers eu nem ligo pra essas coisas de responder e pa.. pra MIM eles sao os msm desde SEMPRE e vo continuar amando eles me*</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id>6118810923</in_reply_to_status_id>
    <in_reply_to_user_id>74561853</in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name>bpradolovers</in_reply_to_screen_name>
    <user>
      <id>48044595</id>
      <name>Marie Rochebois !</name>
      <screen_name>marie_vr</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://a1.twimg.com/profile_images/541774726/Picture_066_normal.jpg</profile_image_url>
      <url>http://www.orkut.com.br/Main#Profile?uid=8665293908869110209&amp;rl=t</url>
      <protected>false</protected>
      <followers_count>239</followers_count>
      <profile_background_color>eb1717</profile_background_color>
      <profile_text_color>1f1f1d</profile_text_color>
      <profile_link_color>f01fe9</profile_link_color>
      <profile_sidebar_fill_color>8838bd</profile_sidebar_fill_color>
      <profile_sidebar_border_color>fc0ad8</profile_sidebar_border_color>
      <friends_count>160</friends_count>
      <created_at>Wed Jun 17 17:44:04 +0000 2009</created_at>
      <favourites_count>85</favourites_count>
      <utc_offset>-32400</utc_offset>
      <time_zone>Alaska</time_zone>
      <profile_background_image_url>http://a1.twimg.com/profile_background_images/53871388/caveira-de-diamantes_-amor_dios.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>7664</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906667</id>
    <text>Follow my nigga, my Lil brother @KENNETHVP he keep big shit going on in the M</text>
    <source>&lt;a href=""http://echofon.com/"" rel=""nofollow""&gt;Echofon&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>30511463</id>
      <name>Dj Hollywood Oompa </name>
      <screen_name>HollywoodOompa</screen_name>
      <location></location>
      <description>i am a cool ass person. i am a DJ and i am an entertainer. i dont keep drama around me, and i am a hard worker hell im working now </description>
      <profile_image_url>http://a1.twimg.com/profile_images/540923604/16644_568873017318_56703609_33197738_592021_n_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>278</followers_count>
      <profile_background_color>C0DEED</profile_background_color>
      <profile_text_color>333333</profile_text_color>
      <profile_link_color>0084B4</profile_link_color>
      <profile_sidebar_fill_color>DDEEF6</profile_sidebar_fill_color>
      <profile_sidebar_border_color>C0DEED</profile_sidebar_border_color>
      <friends_count>81</friends_count>
      <created_at>Sat Apr 11 19:55:17 +0000 2009</created_at>
      <favourites_count>2</favourites_count>
      <utc_offset>-18000</utc_offset>
      <time_zone>Quito</time_zone>
      <profile_background_image_url>http://a1.twimg.com/profile_background_images/55606030/mix.jpg</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>457</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
</statuses>";
    }
}
