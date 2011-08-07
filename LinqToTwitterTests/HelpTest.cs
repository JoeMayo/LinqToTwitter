using System.Linq;
using System.Xml.Linq;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    
    
    /// <summary>
    ///This is a test class for HelpTest and is intended
    ///to contain all HelpTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HelpTest
    {
        #region Test Data

        private string helpConfigurationXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <short_url_length>19</short_url_length>
  <short_url_length_https>20</short_url_length_https>
  <non_username_paths type=""array"">
    <non_username_path>about</non_username_path>
    <non_username_path>account</non_username_path>
    <non_username_path>accounts</non_username_path>
    <non_username_path>activity</non_username_path>
    <non_username_path>all</non_username_path>
    <non_username_path>announcements</non_username_path>
    <non_username_path>anywhere</non_username_path>
    <non_username_path>api_rules</non_username_path>
    <non_username_path>api_terms</non_username_path>
    <non_username_path>apirules</non_username_path>
    <non_username_path>apps</non_username_path>
    <non_username_path>auth</non_username_path>
    <non_username_path>badges</non_username_path>
    <non_username_path>blog</non_username_path>
    <non_username_path>business</non_username_path>
    <non_username_path>buttons</non_username_path>
    <non_username_path>contacts</non_username_path>
    <non_username_path>devices</non_username_path>
    <non_username_path>direct_messages</non_username_path>
    <non_username_path>download</non_username_path>
    <non_username_path>downloads</non_username_path>
    <non_username_path>edit_announcements</non_username_path>
    <non_username_path>faq</non_username_path>
    <non_username_path>favorites</non_username_path>
    <non_username_path>find_sources</non_username_path>
    <non_username_path>find_users</non_username_path>
    <non_username_path>followers</non_username_path>
    <non_username_path>following</non_username_path>
    <non_username_path>friend_request</non_username_path>
    <non_username_path>friendrequest</non_username_path>
    <non_username_path>friends</non_username_path>
    <non_username_path>goodies</non_username_path>
    <non_username_path>help</non_username_path>
    <non_username_path>home</non_username_path>
    <non_username_path>im_account</non_username_path>
    <non_username_path>inbox</non_username_path>
    <non_username_path>invitations</non_username_path>
    <non_username_path>invite</non_username_path>
    <non_username_path>jobs</non_username_path>
    <non_username_path>list</non_username_path>
    <non_username_path>login</non_username_path>
    <non_username_path>logout</non_username_path>
    <non_username_path>me</non_username_path>
    <non_username_path>mentions</non_username_path>
    <non_username_path>messages</non_username_path>
    <non_username_path>newtwitter</non_username_path>
    <non_username_path>notifications</non_username_path>
    <non_username_path>nudge</non_username_path>
    <non_username_path>oauth</non_username_path>
    <non_username_path>phoenix_search</non_username_path>
    <non_username_path>positions</non_username_path>
    <non_username_path>privacy</non_username_path>
    <non_username_path>public_timeline</non_username_path>
    <non_username_path>related_tweets</non_username_path>
    <non_username_path>replies</non_username_path>
    <non_username_path>retweeted_of_mine</non_username_path>
    <non_username_path>retweets</non_username_path>
    <non_username_path>retweets_by_others</non_username_path>
    <non_username_path>rules</non_username_path>
    <non_username_path>saved_searches</non_username_path>
    <non_username_path>search</non_username_path>
    <non_username_path>sent</non_username_path>
    <non_username_path>settings</non_username_path>
    <non_username_path>share</non_username_path>
    <non_username_path>signup</non_username_path>
    <non_username_path>signin</non_username_path>
    <non_username_path>similar_to</non_username_path>
    <non_username_path>statistics</non_username_path>
    <non_username_path>terms</non_username_path>
    <non_username_path>tos</non_username_path>
    <non_username_path>translate</non_username_path>
    <non_username_path>trends</non_username_path>
    <non_username_path>tweetbutton</non_username_path>
    <non_username_path>twttr</non_username_path>
    <non_username_path>update_discoverability</non_username_path>
    <non_username_path>users</non_username_path>
    <non_username_path>welcome</non_username_path>
    <non_username_path>who_to_follow</non_username_path>
    <non_username_path>widgets</non_username_path>
    <non_username_path>zendesk_auth</non_username_path>
    <non_username_path>media_signup</non_username_path>
    <non_username_path>phoenix_qunit_tests</non_username_path>
  </non_username_paths>
  <photo_size_limit>3145728</photo_size_limit>
  <max_media_per_upload>1</max_media_per_upload>
  <characters_reserved_per_media>20</characters_reserved_per_media>
  <photo_sizes>
    <thumb>
      <w>150</w>
      <h>150</h>
      <resize>crop</resize>
    </thumb>
    <small>
      <w>340</w>
      <h>480</h>
      <resize>fit</resize>
    </small>
    <medium>
      <w>600</w>
      <h>1200</h>
      <resize>fit</resize>
    </medium>
    <large>
      <w>1024</w>
      <h>2048</h>
      <resize>fit</resize>
    </large>
  </photo_sizes>
</configuration>";

        private string helpLanguagesXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<languages>
  <language>
    <name>Portuguese</name>
    <code>pt</code>
    <status>production</status>
  </language>
  <language>
    <name>Indonesian</name>
    <code>id</code>
    <status>production</status>
  </language>
  <language>
    <name>Italian</name>
    <code>it</code>
    <status>production</status>
  </language>
  <language>
    <name>Spanish</name>
    <code>es</code>
    <status>production</status>
  </language>
  <language>
    <name>Turkish</name>
    <code>tr</code>
    <status>production</status>
  </language>
  <language>
    <name>English</name>
    <code>en</code>
    <status>production</status>
  </language>
  <language>
    <name>Korean</name>
    <code>ko</code>
    <status>production</status>
  </language>
  <language>
    <name>French</name>
    <code>fr</code>
    <status>production</status>
  </language>
  <language>
    <name>Dutch</name>
    <code>nl</code>
    <status>production</status>
  </language>
  <language>
    <name>Russian</name>
    <code>ru</code>
    <status>production</status>
  </language>
  <language>
    <name>German</name>
    <code>de</code>
    <status>production</status>
  </language>
  <language>
    <name>Japanese</name>
    <code>ja</code>
    <status>production</status>
  </language>
</languages>";

        #endregion

        private TestContext testContextInstance;

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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
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
        public void Create_Populates_Test_Results()
        {
            XElement helpXml = XElement.Parse("<ok>true</ok>");

            Help helpResult = Help.Create(helpXml);

            Assert.IsTrue(helpResult.OK);
        }

        [TestMethod]
        public void Create_Populates_Configuration_Results()
        {
            XElement helpXml = XElement.Parse(this.helpConfigurationXml);

            Help helpResult = Help.Create(helpXml);

            Configuration cfg = helpResult.Configuration;

            Assert.AreEqual(19, cfg.ShortUrlLength);
            Assert.AreEqual(20, cfg.ShortUrlLengthHttps);
            Assert.AreEqual(82, cfg.NonUserNamePaths.Count);
            Assert.AreEqual("about", cfg.NonUserNamePaths.First());
            Assert.AreEqual(3145728, cfg.PhotoSizeLimit);
            Assert.AreEqual(1, cfg.MaxMediaPerUpload);
            Assert.AreEqual(20, cfg.CharactersReservedPerMedia);
            Assert.AreEqual(4, cfg.PhotoSizes.Count);
            Assert.AreEqual("thumb", cfg.PhotoSizes.First().Type);
            Assert.AreEqual(150, cfg.PhotoSizes.First().Width);
            Assert.AreEqual(150, cfg.PhotoSizes.First().Height);
            Assert.AreEqual("crop", cfg.PhotoSizes.First().Resize);
        }

        [TestMethod]
        public void Create_Populates_Languages_Results()
        {
            XElement helpXml = XElement.Parse(this.helpLanguagesXml);

            Help helpResult = Help.Create(helpXml);

            Assert.AreEqual(12, helpResult.Languages.Count);
            Assert.AreEqual("Portuguese", helpResult.Languages.First().Name);
            Assert.AreEqual("pt", helpResult.Languages.First().Code);
            Assert.AreEqual("production", helpResult.Languages.First().Status);
        }
    }
}
