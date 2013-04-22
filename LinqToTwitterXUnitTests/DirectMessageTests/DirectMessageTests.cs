using System;

using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;

using LinqToTwitterXUnitTests.Common;

using LitJson;

using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class DirectMessageTests
    {
        public DirectMessageTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void DirectMessage_Constructor_Parses_Json()
        {
            var dmJson = JsonMapper.ToObject(DirectMessagesResponse);

            var dm = new DirectMessage(dmJson);

            Assert.Equal(new DateTimeOffset(2012, 7, 22, 18, 38, 09, new TimeSpan(0L)).DateTime, dm.CreatedAt);
            Assert.Equal(15411837ul, dm.SenderID);
            Assert.Equal("JoeMayo", dm.SenderScreenName);
            Assert.NotNull(dm.Sender);
            Assert.Equal(16761255ul, dm.RecipientID);
            Assert.Equal("Linq2Tweeter", dm.RecipientScreenName);
            Assert.NotNull(dm.Recipient);
            Assert.Equal(227110300510461952ul, dm.IDResponse);
            Assert.Equal("227110300510461952", dm.IDString);
            Assert.Equal("Testing entities from @JoeMayo : http://t.co/Q1CQHi2 #entitytest", dm.Text);
            Entities entities = dm.Entities;
            Assert.NotNull(entities);
            List<UserMentionEntity> userMentions = entities.UserMentionEntities;
            Assert.NotNull(userMentions);
            Assert.NotEmpty(userMentions);
            UserMentionEntity usrMention = userMentions.FirstOrDefault();
            Assert.NotNull(usrMention);
            Assert.Equal("Joe Mayo", usrMention.Name);
        }

        const string DirectMessagesResponse = @"{
      ""recipient"":{
         ""id"":16761255,
         ""verified"":false,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2182212102\/200xColor_2_normal.png"",
         ""geo_enabled"":true,
         ""default_profile"":false,
         ""statuses_count"":103,
         ""location"":""Denver, CO"",
         ""profile_background_color"":""FFF04D"",
         ""friends_count"":5,
         ""show_all_inline_media"":true,
         ""notifications"":false,
         ""follow_request_sent"":false,
         ""utc_offset"":-25200,
         ""listed_count"":4,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme19\/bg.gif"",
         ""name"":""LINQ to Tweeter"",
         ""profile_link_color"":""0099CC"",
         ""protected"":false,
         ""screen_name"":""Linq2Tweeter"",
         ""is_translator"":false,
         ""profile_use_background_image"":true,
         ""lang"":""en"",
         ""profile_text_color"":""333333"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""description"":""Testing the Account Profile Update with LINQ to Twitter."",
         ""default_profile_image"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme19\/bg.gif"",
         ""favourites_count"":2,
         ""profile_sidebar_border_color"":""fff8ad"",
         ""url"":""http:\/\/linqtotwitter.codeplex.com"",
         ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
         ""id_str"":""16761255"",
         ""profile_background_tile"":false,
         ""profile_sidebar_fill_color"":""f6ffd1"",
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2182212102\/200xColor_2_normal.png"",
         ""following"":false,
         ""followers_count"":26,
         ""contributors_enabled"":false
      },
      ""recipient_id"":16761255,
      ""id_str"":""227110300510461952"",
      ""sender"":{
         ""id"":15411837,
         ""verified"":false,
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""geo_enabled"":true,
         ""default_profile"":false,
         ""statuses_count"":2064,
         ""location"":""Denver, CO"",
         ""profile_background_color"":""FFF04D"",
         ""friends_count"":221,
         ""show_all_inline_media"":false,
         ""notifications"":false,
         ""follow_request_sent"":false,
         ""utc_offset"":-25200,
         ""listed_count"":122,
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/579400467\/200xColor_2.png"",
         ""name"":""Joe Mayo"",
         ""profile_link_color"":""0099CC"",
         ""protected"":false,
         ""screen_name"":""JoeMayo"",
         ""is_translator"":false,
         ""profile_use_background_image"":true,
         ""lang"":""en"",
         ""profile_text_color"":""333333"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP; author of LINQ to Twitter - http:\/\/linqtotwitter.codeplex.com\/"",
         ""default_profile_image"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/579400467\/200xColor_2.png"",
         ""favourites_count"":51,
         ""profile_sidebar_border_color"":""FFF8AD"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""id_str"":""15411837"",
         ""profile_background_tile"":false,
         ""profile_sidebar_fill_color"":""F6FFD1"",
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""following"":false,
         ""followers_count"":1174,
         ""contributors_enabled"":false
      },
      ""sender_screen_name"":""JoeMayo"",
      ""sender_id"":15411837,
      ""entities"":{
         ""user_mentions"":[
            {
               ""name"":""Joe Mayo"",
               ""screen_name"":""JoeMayo"",
               ""id_str"":""15411837"",
               ""indices"":[
                  22,
                  30
               ],
               ""id"":15411837
            }
         ],
         ""hashtags"":[
            {
               ""indices"":[
                  53,
                  64
               ],
               ""text"":""entitytest""
            }
         ],
         ""urls"":[
            {
               ""display_url"":""linqtotwitter.codeplex.com"",
               ""url"":""http:\/\/t.co\/Q1CQHi2"",
               ""indices"":[
                  33,
                  52
               ],
               ""expanded_url"":""http:\/\/linqtotwitter.codeplex.com\/""
            }
         ]
      },
      ""recipient_screen_name"":""Linq2Tweeter"",
      ""id"":227110300510461952,
      ""created_at"":""Sun Jul 22 18:38:09 +0000 2012"",
      ""text"":""Testing entities from @JoeMayo : http:\/\/t.co\/Q1CQHi2 #entitytest""
   }";
    }
}
