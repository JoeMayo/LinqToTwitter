using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.HelpTests
{
    [TestClass]
    public class HelpRequestProcessorTests
    {
        public HelpRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Parses_Parameters()
        {
            var helpReqProc = new HelpRequestProcessor<Help>();
            Expression<Func<Help, bool>> expression =
                help =>
                    help.Type == HelpType.RateLimits &&
                    help.Resources == "search";

            var lambdaExpression = expression as LambdaExpression;

            var queryParams = helpReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)HelpType.RateLimits).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Resources", "search")));
        }

        [TestMethod]
        public void BuildUrl_Generates_Configuration_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/help/configuration.json";
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) HelpType.Configuration).ToString()}
             };

            Request req = helpReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Generates_Languages_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/help/languages.json";
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) HelpType.Languages).ToString()}
             };

            Request req = helpReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Generates_RateLimits_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/application/rate_limit_status.json?resources=search%2Cusers";
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) HelpType.RateLimits).ToString()},
                 {"Resources", "search,users"}
             };

            Request req = helpReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Removes_Parameter_Spaces_In_RateLimits_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/application/rate_limit_status.json?resources=search%2Cusers";
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) HelpType.RateLimits).ToString()},
                 {"Resources", "search, users"}
             };

            Request req = helpReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_With_No_Type()
        {
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                //{"Type", ((int) HelpType.Languages).ToString()}
            };

            //var ex = Assert.Throws<ArgumentException>(() => helpReqProc.BuildUrl(parameters));

            //Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void HelpRequestProcessor_Works_With_Json_Format_Data()
        {
            var helpReqProc = new HelpRequestProcessor<Help>();

            Assert.IsInstanceOfType(helpReqProc, typeof(IRequestProcessorWantsJson));
        }

        [TestMethod]
        public void ProcessResults_Handles_Configuration_Results()
        {
            const int ExpectedCharsRsvpPerMedia = 21;
            const int ExpectedPhotoSizesCount = 4;
            const string ExpectedPhotoSizeType = "thumb";
            const int ExpectedPhotoSizeHeight = 150;
            const int ExpectedPhotoSizeWidth = 150;
            const string ExpectedPhotoSizeResize = "crop";
            const int ExpectedShortUrlLength = 20;
            const int ExpectedPhotoSizeLimit = 3145728;
            const int ExpectedNonUsernamePathsCount = 82;
            const string ExpectedNonUsernamePathsFirstItem = "about";
            const int ExpectedMaxMediaPerUpload = 1;
            const int ExpectedShortUrlLengthHttps = 21;
            var helpReqProc = new HelpRequestProcessor<Help> 
            {
                Type = HelpType.Configuration,
                BaseUrl = "https://api.twitter.com/1.1/" 
            };

            List<Help> helpList = helpReqProc.ProcessResults(HelpConfigurationResponse);

            Assert.IsNotNull(helpList);
            Assert.IsNotNull(helpList.SingleOrDefault());
            Help help = helpList.Single();
            Assert.AreEqual(HelpType.Configuration, help.Type);
            Assert.IsNotNull(help.Configuration);
            Configuration config = help.Configuration;
            Assert.AreEqual(ExpectedCharsRsvpPerMedia, config.CharactersReservedPerMedia);
            List<PhotoSize> photoSizes = config.PhotoSizes;
            Assert.IsNotNull(photoSizes);
            Assert.AreEqual(ExpectedPhotoSizesCount, photoSizes.Count);
            PhotoSize photoSize = photoSizes.First();
            Assert.AreEqual(ExpectedPhotoSizeType, photoSize.Type);
            Assert.AreEqual(ExpectedPhotoSizeHeight, photoSize.Height);
            Assert.AreEqual(ExpectedPhotoSizeWidth, photoSize.Width);
            Assert.AreEqual(ExpectedPhotoSizeResize, photoSize.Resize);
            Assert.AreEqual(ExpectedShortUrlLength, config.ShortUrlLength);
            Assert.AreEqual(ExpectedPhotoSizeLimit, config.PhotoSizeLimit);
            List<string> nonUsernamePaths = config.NonUserNamePaths;
            Assert.IsNotNull(nonUsernamePaths);
            Assert.AreEqual(ExpectedNonUsernamePathsCount, nonUsernamePaths.Count);
            Assert.AreEqual(ExpectedNonUsernamePathsFirstItem, nonUsernamePaths.First());
            Assert.AreEqual(ExpectedMaxMediaPerUpload, config.MaxMediaPerUpload);
            Assert.AreEqual(ExpectedShortUrlLengthHttps, config.ShortUrlLengthHttps);
        }

        [TestMethod]
        public void ProcessResults_Handles_Languages_Results()
        {
            const int ExpectedLanguagesCount = 28;
            const string ExpectedLanguageName = "Hungarian";
            const string ExpectedLanguageStatus = "production";
            const string ExpectedLanguageCode = "hu";
            var helpReqProc = new HelpRequestProcessor<Help> 
            {
                Type = HelpType.Languages,
                BaseUrl = "https://api.twitter.com/1.1/" 
            };

            List<Help> helpList = helpReqProc.ProcessResults(HelpLanguagesXml);

            Assert.IsNotNull(helpList);
            Assert.IsNotNull(helpList.SingleOrDefault());
            Help help = helpList.Single();
            Assert.AreEqual(HelpType.Languages, help.Type);
            List<Language> languages = help.Languages;
            Assert.IsNotNull(languages);
            Assert.AreEqual(ExpectedLanguagesCount, languages.Count);
            Language language = languages.First();
            Assert.AreEqual(ExpectedLanguageName, language.Name);
            Assert.AreEqual(ExpectedLanguageStatus, language.Status);
            Assert.AreEqual(ExpectedLanguageCode, language.Code);
        }

        [TestMethod]
        public void ProcessResults_Handles_RateLimits_Results()
        {
            var helpReqProc = new HelpRequestProcessor<Help>
            {
                Type = HelpType.RateLimits,
                BaseUrl = "https://api.twitter.com/1.1/"
            };

            List<Help> helpList = helpReqProc.ProcessResults(RateLimitsResponse);

            Assert.IsNotNull(helpList);
            Assert.IsNotNull(helpList.SingleOrDefault());
            Help help = helpList.Single();
            Assert.AreEqual(HelpType.RateLimits, help.Type);
            Assert.AreEqual("15411837-3wGGrD7CY0Hb0tguLA3pSH7EMwSWWcnuD3DEQ1E27", help.RateLimitAccountContext);
            Assert.IsNotNull(help.RateLimits);
            Assert.IsTrue(help.RateLimits.Any());
            Dictionary<string, List<RateLimits>> rateLimits = help.RateLimits;
            Assert.IsTrue(rateLimits.ContainsKey("lists"));
            List<RateLimits> limitsList = rateLimits["lists"];
            Assert.IsNotNull(limitsList);
            Assert.IsTrue(limitsList.Any());
            RateLimits limits = limitsList.First();
            Assert.IsNotNull(limits);
            Assert.AreEqual("/lists/subscriptions", limits.Resource);
            Assert.AreEqual(15, limits.Limit);
            Assert.AreEqual(15, limits.Remaining);
            Assert.AreEqual(1348087186ul, limits.Reset);
        }

        const string HelpConfigurationResponse = @"{
   ""characters_reserved_per_media"":21,
   ""photo_sizes"":{
      ""thumb"":{
         ""h"":150,
         ""w"":150,
         ""resize"":""crop""
      },
      ""small"":{
         ""h"":480,
         ""w"":340,
         ""resize"":""fit""
      },
      ""large"":{
         ""h"":2048,
         ""w"":1024,
         ""resize"":""fit""
      },
      ""medium"":{
         ""h"":1200,
         ""w"":600,
         ""resize"":""fit""
      }
   },
   ""short_url_length"":20,
   ""photo_size_limit"":3145728,
   ""non_username_paths"":[
      ""about"",
      ""account"",
      ""accounts"",
      ""activity"",
      ""all"",
      ""announcements"",
      ""anywhere"",
      ""api_rules"",
      ""api_terms"",
      ""apirules"",
      ""apps"",
      ""auth"",
      ""badges"",
      ""blog"",
      ""business"",
      ""buttons"",
      ""contacts"",
      ""devices"",
      ""direct_messages"",
      ""download"",
      ""downloads"",
      ""edit_announcements"",
      ""faq"",
      ""favorites"",
      ""find_sources"",
      ""find_users"",
      ""followers"",
      ""following"",
      ""friend_request"",
      ""friendrequest"",
      ""friends"",
      ""goodies"",
      ""help"",
      ""home"",
      ""im_account"",
      ""inbox"",
      ""invitations"",
      ""invite"",
      ""jobs"",
      ""list"",
      ""login"",
      ""logout"",
      ""me"",
      ""mentions"",
      ""messages"",
      ""mockview"",
      ""newtwitter"",
      ""notifications"",
      ""nudge"",
      ""oauth"",
      ""phoenix_search"",
      ""positions"",
      ""privacy"",
      ""public_timeline"",
      ""related_tweets"",
      ""replies"",
      ""retweeted_of_mine"",
      ""retweets"",
      ""retweets_by_others"",
      ""rules"",
      ""saved_searches"",
      ""search"",
      ""sent"",
      ""settings"",
      ""share"",
      ""signup"",
      ""signin"",
      ""similar_to"",
      ""statistics"",
      ""terms"",
      ""tos"",
      ""translate"",
      ""trends"",
      ""tweetbutton"",
      ""twttr"",
      ""update_discoverability"",
      ""users"",
      ""welcome"",
      ""who_to_follow"",
      ""widgets"",
      ""zendesk_auth"",
      ""media_signup""
   ],
   ""max_media_per_upload"":1,
   ""short_url_length_https"":21
}";

        const string HelpLanguagesXml = @"[
   {
      ""name"":""Hungarian"",
      ""status"":""production"",
      ""code"":""hu""
   },
   {
      ""name"":""Finnish"",
      ""status"":""production"",
      ""code"":""fi""
   },
   {
      ""name"":""Swedish"",
      ""status"":""production"",
      ""code"":""sv""
   },
   {
      ""name"":""Norwegian"",
      ""status"":""production"",
      ""code"":""no""
   },
   {
      ""name"":""Hebrew"",
      ""status"":""production"",
      ""code"":""he""
   },
   {
      ""name"":""Korean"",
      ""status"":""production"",
      ""code"":""ko""
   },
   {
      ""name"":""Portuguese"",
      ""status"":""production"",
      ""code"":""pt""
   },
   {
      ""name"":""French"",
      ""status"":""production"",
      ""code"":""fr""
   },
   {
      ""name"":""German"",
      ""status"":""production"",
      ""code"":""de""
   },
   {
      ""name"":""Arabic"",
      ""status"":""production"",
      ""code"":""ar""
   },
   {
      ""name"":""Russian"",
      ""status"":""production"",
      ""code"":""ru""
   },
   {
      ""name"":""Dutch"",
      ""status"":""production"",
      ""code"":""nl""
   },
   {
      ""name"":""Indonesian"",
      ""status"":""production"",
      ""code"":""id""
   },
   {
      ""name"":""Traditional Chinese"",
      ""status"":""production"",
      ""code"":""zh-tw""
   },
   {
      ""name"":""Italian"",
      ""status"":""production"",
      ""code"":""it""
   },
   {
      ""name"":""Hindi"",
      ""status"":""production"",
      ""code"":""hi""
   },
   {
      ""name"":""English"",
      ""status"":""production"",
      ""code"":""en""
   },
   {
      ""name"":""Filipino"",
      ""status"":""production"",
      ""code"":""fil""
   },
   {
      ""name"":""Japanese"",
      ""status"":""production"",
      ""code"":""ja""
   },
   {
      ""name"":""Thai"",
      ""status"":""production"",
      ""code"":""th""
   },
   {
      ""name"":""Urdu"",
      ""status"":""production"",
      ""code"":""ur""
   },
   {
      ""name"":""Polish"",
      ""status"":""production"",
      ""code"":""pl""
   },
   {
      ""name"":""Simplified Chinese"",
      ""status"":""production"",
      ""code"":""zh-cn""
   },
   {
      ""name"":""Turkish"",
      ""status"":""production"",
      ""code"":""tr""
   },
   {
      ""name"":""Farsi"",
      ""status"":""production"",
      ""code"":""fa""
   },
   {
      ""name"":""Danish"",
      ""status"":""production"",
      ""code"":""da""
   },
   {
      ""name"":""Malay"",
      ""status"":""production"",
      ""code"":""msa""
   },
   {
      ""name"":""Spanish"",
      ""status"":""production"",
      ""code"":""es""
   }
]";

        const string RateLimitsResponse = @"{
   ""rate_limit_context"":{
      ""access_token"":""15411837-3wGGrD7CY0Hb0tguLA3pSH7EMwSWWcnuD3DEQ1E27""
   },
   ""resources"":{
      ""lists"":{
         ""/lists/subscriptions"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/subscribers/show"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/members"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/subscribers"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/list"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/memberships"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/show"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/statuses"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/lists/members/show"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""friendships"":{
         ""/friendships/incoming"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/friendships/show"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/friendships/lookup"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/friendships/outgoing"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""blocks"":{
         ""/blocks/ids"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/blocks/list"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""geo"":{
         ""/geo/id/:place_id"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/geo/reverse_geocode"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/geo/search"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/geo/similar_places"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""users"":{
         ""/users/suggestions/:slug/members"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/users/search"":{
            ""limit"":180,
            ""remaining"":180,
            ""reset"":1348087186
         },
         ""/users/show"":{
            ""limit"":180,
            ""remaining"":180,
            ""reset"":1348087186
         },
         ""/users/contributees"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/users/contributors"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/users/suggestions"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/users/lookup"":{
            ""limit"":180,
            ""remaining"":180,
            ""reset"":1348087186
         },
         ""/users/suggestions/:slug"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""followers"":{
         ""/followers/ids"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""statuses"":{
         ""/statuses/home_timeline"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/statuses/mentions_timeline"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/statuses/show/:id"":{
            ""limit"":180,
            ""remaining"":180,
            ""reset"":1348087186
         },
         ""/statuses/retweets/:id"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/statuses/user_timeline"":{
            ""limit"":180,
            ""remaining"":180,
            ""reset"":1348087186
         },
         ""/statuses/oembed"":{
            ""limit"":180,
            ""remaining"":180,
            ""reset"":1348087186
         }
      },
      ""help"":{
         ""/help/privacy"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/help/tos"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/help/configuration"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/help/languages"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""friends"":{
         ""/friends/ids"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""direct_messages"":{
         ""/direct_messages"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/direct_messages/show"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/direct_messages/sent"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""account"":{
         ""/account/verify_credentials"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/account/settings"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""favorites"":{
         ""/favorites/list"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""saved_searches"":{
         ""/saved_searches/list"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/saved_searches/show/:id"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      },
      ""search"":{
         ""/search/tweets"":{
            ""limit"":180,
            ""remaining"":180,
            ""reset"":1348087186
         }
      },
      ""trends"":{
         ""/trends/available"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/trends/closest"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         },
         ""/trends/place"":{
            ""limit"":15,
            ""remaining"":15,
            ""reset"":1348087186
         }
      }
   }
}";
    }
}
