using System;

using LinqToTwitter;

using LinqToTwitterXUnitTests.Common;

using LitJson;

using Xunit;

namespace LinqToTwitterXUnitTests.FavoritesTests
{
    public class FavoritesCtorTests
    {
        public FavoritesCtorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void Favorites_Ctor_Parses_JsonData()
        {
            var favJson = JsonMapper.ToObject(TwitterResponse);

            var favorite = new Favorites(favJson);

            Assert.Equal("1234", favorite.InReplyToStatusID);
            Assert.Equal(new DateTimeOffset(2012, 4, 5, 16, 44, 8, new TimeSpan(0)).DateTime, favorite.CreatedAt);
            Assert.False(favorite.Truncated);
            Assert.Equal(false, favorite.Retweeted);
            Assert.NotNull(favorite.User);
        }

        const string TwitterResponse = @"
   {
      ""in_reply_to_status_id_str"":""1234"",
      ""in_reply_to_status_id"":1234,
      ""created_at"":""Thu Apr 05 16:44:08 +0000 2012"",
      ""id_str"":""187943716558864384"",
      ""in_reply_to_user_id_str"":""5678"",
      ""coordinates"":null,
      ""truncated"":false,
      ""place"":null,
      ""retweeted"":false,
      ""user"":{
         ""is_translator"":false,
         ""show_all_inline_media"":true,
         ""id"":24358412,
         ""statuses_count"":1921,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""follow_request_sent"":false,
         ""default_profile_image"":false,
         ""favourites_count"":10,
         ""utc_offset"":null,
         ""profile_background_color"":""C0DEED"",
         ""verified"":false,
         ""name"":""Mark Russinovich"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/198832493\/ZeroDayCover.jpg"",
         ""protected"":false,
         ""profile_link_color"":""0084B4"",
         ""lang"":""en"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1497556516\/markrussinovich_normal.jpg"",
         ""url"":""http:\/\/www.zerodaythebook.com"",
         ""created_at"":""Sat Mar 14 11:43:34 +0000 2009"",
         ""default_profile"":false,
         ""profile_use_background_image"":true,
         ""description"":""Technical Fellow at Microsoft in Windows Azure, author of Zero Day: A Novel, Windows Internals, Sysinternals Admin Reference and the Sysinternals utilities. "",
         ""id_str"":""24358412"",
         ""profile_text_color"":""333333"",
         ""time_zone"":null,
         ""following"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""location"":"""",
         ""notifications"":false,
         ""listed_count"":596,
         ""geo_enabled"":false,
         ""friends_count"":70,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1497556516\/markrussinovich_normal.jpg"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/198832493\/ZeroDayCover.jpg"",
         ""followers_count"":11740,
         ""screen_name"":""markrussinovich"",
         ""profile_background_tile"":true
      }
   }";
    }
}
