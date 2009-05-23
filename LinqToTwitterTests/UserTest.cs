using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for UserTest and is intended
    ///to contain all UserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserTest
    {
        //
        // User Response Example as of 5/23/09
        //

        string m_userXml = @"<user>
          <id>15411837</id>
          <name>Joe Mayo</name>
          <screen_name>JoeMayo</screen_name>
          <location>Denver, CO</location>
          <description>Created LINQ to Twitter, author of LINQ Programming/McGraw-Hill, .NET Consulting, and Microsoft MVP</description>
          <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/197875757/JoeTwitterBW_normal.jpg</profile_image_url>
          <url>http://linqtotwitter.codeplex.com/</url>
          <protected>false</protected>
          <followers_count>105</followers_count>
          <profile_background_color>0099B9</profile_background_color>
          <profile_text_color>3C3940</profile_text_color>
          <profile_link_color>0099B9</profile_link_color>
          <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
          <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
          <friends_count>33</friends_count>
          <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
          <favourites_count>4</favourites_count>
          <utc_offset>-25200</utc_offset>
          <time_zone>Mountain Time (US &amp; Canada)</time_zone>
          <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/13330711/200xColor_2.png</profile_background_image_url>
          <profile_background_tile>false</profile_background_tile>
          <statuses_count>198</statuses_count>
          <notifications>false</notifications>
          <following>false</following>
          <status>
            <created_at>Sat May 23 20:07:52 +0000 2009</created_at>
            <id>1896163291</id>
            <text>Twitter API Group on Facebook: http://bit.ly/HARSx</text>
            <source>web</source>
            <truncated>false</truncated>
            <in_reply_to_status_id></in_reply_to_status_id>
            <in_reply_to_user_id></in_reply_to_user_id>
            <favorited>false</favorited>
            <in_reply_to_screen_name></in_reply_to_screen_name>
          </status>
        </user>";
        
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

        /// <summary>
        ///A test for CreateUser
        ///</summary>
        [TestMethod()]
        public void CreateNullUserTest()
        {
            User actual = new User().CreateUser(null);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for CreateUser
        ///</summary>
        [TestMethod()]
        public void CreateUserTest()
        {
            User target = new User();
            XElement user = XElement.Parse(m_userXml);
            User expected = new User()
            {
                 CreatedAt = new DateTime(2008, 7, 13, 4, 35, 50),
                 Description = "Created LINQ to Twitter, author of LINQ Programming/McGraw-Hill, .NET Consulting, and Microsoft MVP",
                 Name = "Joe Mayo",
                 Following = false
            };

            User actual = target.CreateUser(user);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Following, actual.Following);
        }
    }
}
