using System;
using LinqToTwitter.Common;
using LinqToTwitterTests.Common;
using LitJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    [TestClass]
    public class TypeConversionExtensionsTest
    {
        public TypeConversionExtensionsTest()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetValue_Returns_Decimal()
        {
            JsonData search = JsonMapper.ToObject(SearchJson);

            var val = search.GetValue<decimal>("completed_in");

            Assert.AreEqual(.057m, val);
        }

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
