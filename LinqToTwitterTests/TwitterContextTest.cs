using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;
using NMock2;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for TwitterContextTest and is intended
    ///to contain all TwitterContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TwitterContextTest
    {
        private Mockery m_mocks;
        private TwitterContext m_ctx;
        private ITwitterExecute m_twitterExecute;
        private TestContext m_testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return m_testContextInstance;
            }
            set
            {
                m_testContextInstance = value;
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            m_mocks = new Mockery();
            m_twitterExecute = m_mocks.NewMock<ITwitterExecute>();
            var authorizedClient = m_mocks.NewMock<ITwitterAuthorization>();
            Expect.Once.On(m_twitterExecute).GetProperty("AuthorizedClient").Will(Return.Value(authorizedClient));
            Expect.Once.On(authorizedClient).SetProperty("AuthenticationTarget");
            m_ctx = new TwitterContext(m_twitterExecute);
        }
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///1 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void OneParamCtorDefaults()
        {
            string baseUrl = "http://twitter.com/";
            string searchUrl = "http://search.twitter.com/";
            ITwitterAuthorization authorizedClient = new UsernamePasswordAuthorization();
            TwitterContext ctx = new TwitterContext(authorizedClient);

            Assert.AreSame(authorizedClient, ctx.AuthorizedClient);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///3 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void ThreeParamCtorDefaults()
        {
            ITwitterExecute execute = new TwitterExecute();
            string baseUrl = "http://www.twitter.com/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx = new TwitterContext(execute, baseUrl, searchUrl);

            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///3 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void ObjectInitializerTest()
        {
            string baseUrl = "http://www.twitter.com/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx =
                new TwitterContext
                {
                    BaseUrl = baseUrl,
                    SearchUrl = searchUrl,
                };

            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        public void CreateRequestProcessorTestHelper<T>()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            var statusProc = ctx.CreateRequestProcessor(publicQuery.Expression, false);
            Assert.IsInstanceOfType(statusProc, typeof(StatusRequestProcessor));
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void CreateRequestProcessorTest()
        {
            CreateRequestProcessorTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            var ctx = new TwitterContext();

            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            var actual = ctx.Execute(publicQuery.Expression, true);
            var tweets = actual as IList<Status>;
            Assert.IsNotNull(tweets);
            Assert.IsTrue(tweets.Count > 0);
        }

        /// <summary>
        ///A test for UpdateStatus
        ///</summary>
        [TestMethod()]
        public void UpdateStatusTest1()
        {
            string status = "Hello";
            string inReplyToStatusID = "1";
            Status expected = new Status();
            var statusQueryable =
                new List<Status>()
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(statusQueryable));

            Status actual = m_ctx.UpdateStatus(status, inReplyToStatusID);

            m_mocks.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateStatus
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusNullStatusTest1()
        {
            string status = null;
            string inReplyToStatusID = "1";
            Status expected = new Status();
            var statusQueryable =
                new List<Status>()
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(statusQueryable));

            Status actual = m_ctx.UpdateStatus(status, inReplyToStatusID);

            m_mocks.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateStatus
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusOver140Test1()
        {
            string status = new string(Enumerable.Repeat<char>('x', 141).ToArray());
            string inReplyToStatusID = "1";
            Status expected = new Status();
            var statusQueryable =
                new List<Status>()
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(statusQueryable));

            Status actual = m_ctx.UpdateStatus(status, inReplyToStatusID);

            m_mocks.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateStatus
        ///</summary>
        [TestMethod()]
        public void UpdateStatusTest()
        {
            string status = "Hello";
            Status expected = new Status();
            var statusQueryable =
                new List<Status>()
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(statusQueryable));

            Status actual = m_ctx.UpdateStatus(status);

            m_mocks.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateStatus
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusNullStatusTest()
        {
            string status = null;
            Status expected = new Status();
            var statusQueryable =
                new List<Status>()
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(statusQueryable));

            Status actual = m_ctx.UpdateStatus(status);

            m_mocks.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateStatus
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusOver140Test()
        {
            string status = new string(Enumerable.Repeat<char>('x', 141).ToArray());
            Status expected = new Status();
            var statusQueryable =
                new List<Status>()
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(statusQueryable));

            Status actual = m_ctx.UpdateStatus(status);

            m_mocks.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountProfile
        ///</summary>
        [TestMethod()]
        public void UpdateAccountProfileTest()
        {
            string name = "Joe";
            string email = "Joe@LinqToTwitter.com";
            string url = "http://www.csharp-station.com";
            string location = "Denver, CO";
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountProfile(name, email, url, location, description);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountProfile
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileNullInputTest()
        {
            string name = string.Empty;
            string email = string.Empty;
            string url = string.Empty;
            string location = string.Empty;
            string description = string.Empty;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountProfile(name, email, url, location, description);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountProfile
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileNameOver20Test()
        {
            string name = new string(Enumerable.Repeat<char>('x', 21).ToArray());
            string email = "Joe@LinqToTwitter.com";
            string url = "http://www.csharp-station.com";
            string location = "Denver, CO";
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountProfile(name, email, url, location, description);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountProfile
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileEmailOver40Test()
        {
            string name = "Joe";
            string email = new string(Enumerable.Repeat<char>('x', 41).ToArray());
            string url = "http://www.csharp-station.com";
            string location = "Denver, CO";
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountProfile(name, email, url, location, description);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountProfile
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileUrlOver100Test()
        {
            string name = "Joe";
            string email = "Joe@LinqToTwitter.com";
            string url = new string(Enumerable.Repeat<char>('x', 101).ToArray()); ;
            string location = "Denver, CO";
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountProfile(name, email, url, location, description);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountProfile
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileLocationOver30Test()
        {
            string name = "Joe";
            string email = "Joe@LinqToTwitter.com";
            string url = "http://www.csharp-station.com";
            string location = new string(Enumerable.Repeat<char>('x', 31).ToArray()); ;
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountProfile(name, email, url, location, description);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountProfile
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileDescriptionOver160Test()
        {
            string name = "Joe";
            string email = "Joe@LinqToTwitter.com";
            string url = "http://www.csharp-station.com";
            string location = "Denver, CO";
            string description = new string(Enumerable.Repeat<char>('x', 161).ToArray());
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountProfile(name, email, url, location, description);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountImage
        ///</summary>
        [TestMethod()]
        public void UpdateAccountImageTest()
        {
            string imageFilePath = "c:\\image.jpg";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("PostTwitterFile")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountImage(imageFilePath);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountImage
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountImageNullPathTest()
        {
            string imageFilePath = null;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("PostTwitterFile")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountImage(imageFilePath);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountDeliveryDevice
        ///</summary>
        [TestMethod()]
        public void UpdateAccountDeliveryDeviceTest()
        {
            DeviceType device = new DeviceType();
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountDeliveryDevice(device);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountColors
        ///</summary>
        [TestMethod()]
        public void UpdateAccountColorsTest()
        {
            string background = "9ae4e8";
            string text = "#000000";
            string link = "#0000ff";
            string sidebarFill = "#e0ff92";
            string sidebarBorder = "#87bc44";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountColors(background, text, link, sidebarFill, sidebarBorder);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountColors
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountColorsNoInputTest()
        {
            string background = null;
            string text = null;
            string link = null;
            string sidebarFill = null;
            string sidebarBorder = null;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.UpdateAccountColors(background, text, link, sidebarFill, sidebarBorder);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountBackgroundImage
        ///</summary>
        [TestMethod()]
        public void UpdateAccountBackgroundImageTest()
        {
            string imageFilePath = "C:\\image.png";
            bool tile = false;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("PostTwitterFile")
                .Will(Return.Value(expectedList));

            User actual;
            actual = m_ctx.UpdateAccountBackgroundImage(imageFilePath, tile);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateAccountBackgroundImage
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountBackgroundImageNullPathTest()
        {
            string imageFilePath = string.Empty;
            bool tile = false;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("PostTwitterFile")
                .Will(Return.Value(expectedList));

            User actual;
            actual = m_ctx.UpdateAccountBackgroundImage(imageFilePath, tile);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for NewDirectMessage
        ///</summary>
        [TestMethod()]
        public void NewDirectMessageTest()
        {
            string userID = "1";
            string text = "Hi";
            DirectMessage expected = new DirectMessage();
            var expectedList =
                new List<DirectMessage>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            DirectMessage actual = m_ctx.NewDirectMessage(userID, text);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for NewDirectMessage
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void NewDirectMessageNullMessageTest()
        {
            string userID = "1";
            string text = null;
            DirectMessage expected = new DirectMessage();
            var expectedList =
                new List<DirectMessage>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            DirectMessage actual = m_ctx.NewDirectMessage(userID, text);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for NewDirectMessage
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void NewDirectMessageNullUserIDTest()
        {
            string userID = string.Empty;
            string text = "Test Text";
            DirectMessage expected = new DirectMessage();
            var expectedList =
                new List<DirectMessage>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            DirectMessage actual = m_ctx.NewDirectMessage(userID, text);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for HelpTest
        ///</summary>
        [TestMethod()]
        public void HelpTestTest()
        {
            bool expected = false;
            var expectedList =
                new List<bool>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            bool actual = m_ctx.HelpTest();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EndAccountSession
        ///</summary>
        [TestMethod()]
        public void EndAccountSessionTest()
        {
            TwitterHashResponse expected = new TwitterHashResponse
            {
                Error = "Session Ended",
                Request = "http://twitter.com"
            };
            Account acct = new Account { EndSessionStatus = expected };
            var expectedList =
                new List<Account>
                {
                    acct
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            TwitterHashResponse actual = m_ctx.EndAccountSession();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EnableNotifications
        ///</summary>
        [TestMethod()]
        public void EnableNotificationsTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.EnableNotifications(id, userID, screenName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EnableNotifications
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void EnableNotificationsNoInputTest()
        {
            string id = null;
            string userID = null;
            string screenName = string.Empty;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.EnableNotifications(id, userID, screenName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DisableNotifications
        ///</summary>
        [TestMethod()]
        public void DisableNotificationsTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.DisableNotifications(id, userID, screenName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DisableNotifications
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DisableNotificationsNoInputTest()
        {
            string id = string.Empty;
            string userID = null;
            string screenName = string.Empty;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.DisableNotifications(id, userID, screenName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyStatus
        ///</summary>
        [TestMethod()]
        public void DestroyStatusTest()
        {
            string id = "1";
            Status expected = new Status();
            var expectedList =
                new List<Status>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            Status actual = m_ctx.DestroyStatus(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyStatus
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyStatusNullStatusTest()
        {
            string id = string.Empty;
            Status expected = new Status();
            var expectedList =
                new List<Status>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            Status actual = m_ctx.DestroyStatus(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyFriendship
        ///</summary>
        [TestMethod()]
        public void DestroyFriendshipTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.DestroyFriendship(id, userID, screenName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyFriendship
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyFriendshipNoInputTest()
        {
            string id = null;
            string userID = string.Empty;
            string screenName = null;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.DestroyFriendship(id, userID, screenName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyFavorite
        ///</summary>
        [TestMethod()]
        public void DestroyFavoriteTest()
        {
            string id = "1";
            Status expected = new Status();
            var expectedList =
                new List<Status>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            Status actual = m_ctx.DestroyFavorite(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyFavorite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyFavoriteNullIDTest()
        {
            string id = string.Empty;
            Status expected = new Status();
            var expectedList =
                new List<Status>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            Status actual = m_ctx.DestroyFavorite(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyDirectMessage
        ///</summary>
        [TestMethod()]
        public void DestroyDirectMessageTest()
        {
            string id = "1";
            DirectMessage expected = new DirectMessage();
            var expectedList =
                new List<DirectMessage>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            DirectMessage actual = m_ctx.DestroyDirectMessage(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyDirectMessage
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyDirectMessageNullIDTest()
        {
            string id = null;
            DirectMessage expected = new DirectMessage();
            var expectedList =
                new List<DirectMessage>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            DirectMessage actual = m_ctx.DestroyDirectMessage(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyBlock
        ///</summary>
        [TestMethod()]
        public void DestroyBlockTest()
        {
            string id = "1";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            User actual = m_ctx.DestroyBlock(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DestroyBlock
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyBlockNullIDTest()
        {
            string id = string.Empty;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            User actual = m_ctx.DestroyBlock(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateStatusRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Status select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, false);
            Assert.IsInstanceOfType(actual, typeof(StatusRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateAccountRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Account select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(AccountRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateBlocksRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Blocks select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(BlocksRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateDirectMessageRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.DirectMessage select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(DirectMessageRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateFavoritesRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Favorites select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(FavoritesRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateFriendshipRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Friendship select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(FriendshipRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateSearchRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Search select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(SearchRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateSocialGraphRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.SocialGraph select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(SocialGraphRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateTrendRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Trends select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(TrendRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateUserRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.User select tweet;

            IRequestProcessor actual = ctx.CreateRequestProcessor(queryResult.Expression, true);
            Assert.IsInstanceOfType(actual, typeof(UserRequestProcessor));
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        [TestMethod()]
        public void CreateRequestProcessorNullExpressionTest1()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            IRequestProcessor actual = ctx.CreateRequestProcessor(null, true);
            Assert.IsInstanceOfType(actual, typeof(StatusRequestProcessor));
        }

        /// <summary>
        ///A test for CreateFriendship
        ///</summary>
        [TestMethod()]
        public void CreateFriendshipTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            bool follow = false;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.CreateFriendship(id, userID, screenName, follow);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateFriendship
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFriendshipNoInputTest()
        {
            string id = string.Empty;
            string userID = string.Empty;
            string screenName = null;
            bool follow = false;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.CreateFriendship(id, userID, screenName, follow);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateFavorite
        ///</summary>
        [TestMethod()]
        public void CreateFavoriteTest()
        {
            string id = "1";
            Status expected = new Status();
            var expectedList =
                new List<Status>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            Status actual = m_ctx.CreateFavorite(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateFavorite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFavoriteNoIDTest()
        {
            string id = null;
            Status expected = new Status();
            var expectedList =
                new List<Status>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));
            Status actual = m_ctx.CreateFavorite(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateBlock
        ///</summary>
        [TestMethod()]
        public void CreateBlockTest()
        {
            string id = "1";
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.CreateBlock(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateBlock
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateBlockNoIDTest()
        {
            string id = string.Empty;
            User expected = new User();
            var expectedList =
                new List<User>
                {
                    expected
                };

            Expect.Once.On(m_twitterExecute)
                .Method("ExecuteTwitter")
                .Will(Return.Value(expectedList));

            User actual = m_ctx.CreateBlock(id);
            Assert.AreEqual(expected, actual);
        }
    }
}
