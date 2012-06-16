using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.HelpTests
{
    public class HelpRequestProcessorTests
    {
        public HelpRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void GetParameters_Reads_All_Parameters()
        {
            var helpReqProc = new HelpRequestProcessor<Help>();
            Expression<Func<Help, bool>> expression =
                help => help.Type == HelpType.Test;

            var queryParams = helpReqProc.GetParameters(expression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)HelpType.Test).ToString())));
        }

        [Fact]
        public void BuildUrl_Generates_Test_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/help/test.json";
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) HelpType.Test).ToString()}
             };

            Request req = helpReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Generates_Configuration_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/help/configuration.json";
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) HelpType.Configuration).ToString()}
             };

            Request req = helpReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Generates_Languages_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/help/languages.json";
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) HelpType.Languages).ToString()}
             };

            Request req = helpReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_With_No_Type()
        {
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                //{"Type", ((int) HelpType.Languages).ToString()}
            };

            var ex = Assert.Throws<ArgumentException>(() => helpReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void HelpRequestProcessor_Works_With_Json_Format_Data()
        {
            var helpReqProc = new HelpRequestProcessor<Help>();

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(helpReqProc);
        }

        [Fact]
        public void ProcessResults_Handles_Test_Results()
        {
            var helpReqProc = new HelpRequestProcessor<Help> { BaseUrl = "https://api.twitter.com/1/" };

            List<Help> helpList = helpReqProc.ProcessResults(HelpTestResponse);

            Assert.NotNull(helpList);
            Assert.Single(helpList);
            Help help = helpList.Single();
            Assert.Equal(HelpType.Test, help.Type);
            Assert.True(help.OK);
        }

        [Fact]
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
                BaseUrl = "https://api.twitter.com/1/" 
            };

            List<Help> helpList = helpReqProc.ProcessResults(HelpConfigurationResponse);

            Assert.NotNull(helpList);
            Assert.Single(helpList);
            Help help = helpList.Single();
            Assert.Equal(HelpType.Configuration, help.Type);
            Assert.NotNull(help.Configuration);
            Configuration config = help.Configuration;
            Assert.Equal(ExpectedCharsRsvpPerMedia, config.CharactersReservedPerMedia);
            List<PhotoSize> photoSizes = config.PhotoSizes;
            Assert.NotNull(photoSizes);
            Assert.Equal(ExpectedPhotoSizesCount, photoSizes.Count);
            PhotoSize photoSize = photoSizes.First();
            Assert.Equal(ExpectedPhotoSizeType, photoSize.Type);
            Assert.Equal(ExpectedPhotoSizeHeight, photoSize.Height);
            Assert.Equal(ExpectedPhotoSizeWidth, photoSize.Width);
            Assert.Equal(ExpectedPhotoSizeResize, photoSize.Resize);
            Assert.Equal(ExpectedShortUrlLength, config.ShortUrlLength);
            Assert.Equal(ExpectedPhotoSizeLimit, config.PhotoSizeLimit);
            List<string> nonUsernamePaths = config.NonUserNamePaths;
            Assert.NotNull(nonUsernamePaths);
            Assert.Equal(ExpectedNonUsernamePathsCount, nonUsernamePaths.Count);
            Assert.Equal(ExpectedNonUsernamePathsFirstItem, nonUsernamePaths.First());
            Assert.Equal(ExpectedMaxMediaPerUpload, config.MaxMediaPerUpload);
            Assert.Equal(ExpectedShortUrlLengthHttps, config.ShortUrlLengthHttps);
        }

        [Fact]
        public void ProcessResults_Handles_Languages_Results()
        {
            const int ExpectedLanguagesCount = 28;
            const string ExpectedLanguageName = "Hungarian";
            const string ExpectedLanguageStatus = "production";
            const string ExpectedLanguageCode = "hu";
            var helpReqProc = new HelpRequestProcessor<Help> 
            {
                Type = HelpType.Languages,
                BaseUrl = "https://api.twitter.com/1/" 
            };

            List<Help> helpList = helpReqProc.ProcessResults(HelpLanguagesXml);

            Assert.NotNull(helpList);
            Assert.Single(helpList);
            Help help = helpList.Single();
            Assert.Equal(HelpType.Languages, help.Type);
            List<Language> languages = help.Languages;
            Assert.NotNull(languages);
            Assert.Equal(ExpectedLanguagesCount, languages.Count);
            Language language = languages.First();
            Assert.Equal(ExpectedLanguageName, language.Name);
            Assert.Equal(ExpectedLanguageStatus, language.Status);
            Assert.Equal(ExpectedLanguageCode, language.Code);
        }

        const string HelpTestResponse = "ok";

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
    }
}
