using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Serialization;
using LinqToTwitter;
using LinqToTwitterTests.Common;

namespace LinqToTwitterTests
{
    [TestClass]
    public class XmlSerializerTests
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void Account_Can_Serialize()
        {
            var acct = new Account();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Account));
            xmlSerializer.Serialize(writer, acct);
        }

        [TestMethod]
        public void Blocks_Can_Serialize()
        {
            var block = new Blocks();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Blocks));
            xmlSerializer.Serialize(writer, block);
        }

        [TestMethod]
        public void DirectMessage_Can_Serialize()
        {
            var dm = new DirectMessage();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DirectMessage));
            xmlSerializer.Serialize(writer, dm);
        }

        [Ignore]
        [TestMethod]
        public void Favorites_Can_Serialize()
        {
            var favorite = new Favorites();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Favorites));
            xmlSerializer.Serialize(writer, favorite);
        }

        [TestMethod]
        public void Friendship_Can_Serialize()
        {
            var friend = new Friendship();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Friendship));
            xmlSerializer.Serialize(writer, friend);
        }

        [TestMethod]
        public void Geometry_Can_Serialize()
        {
            var geo = new Geometry();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Geometry));
            xmlSerializer.Serialize(writer, geo);
        }

        [TestMethod]
        public void IDList_Can_Serialize()
        {
            var ids = new IDList();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(IDList));
            xmlSerializer.Serialize(writer, ids);
        }

        [TestMethod]
        public void List_Can_Serialize()
        {
            var list = new List();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List));
            xmlSerializer.Serialize(writer, list);
        }

        [TestMethod]
        public void Raw_Can_Serialize()
        {
            var raw = new Raw();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Raw));
            xmlSerializer.Serialize(writer, raw);
        }

        [TestMethod]
        public void SavedSearch_Can_Serialize()
        {
            var saveSrch = new SavedSearch();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedSearch));
            xmlSerializer.Serialize(writer, saveSrch);
        }

        [TestMethod]
        public void Search_Can_Serialize()
        {
            var search = new Search();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Search));
            xmlSerializer.Serialize(writer, search);
        }

        [TestMethod]
        public void SocialGraph_Can_Serialize()
        {
            var graph = new SocialGraph();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SocialGraph));
            xmlSerializer.Serialize(writer, graph);
        }

        [TestMethod]
        public void Status_Can_Serialize()
        {
            var tweet = new Status();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Status));
            xmlSerializer.Serialize(writer, tweet);
        }

        [TestMethod]
        public void Trend_Can_Serialize()
        {
            var trend = new Trend();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Trend));
            xmlSerializer.Serialize(writer, trend);
        }

        [TestMethod]
        public void User_Can_Serialize()
        {
            var user = new User();
            var stringBuilder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(stringBuilder);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(User));
            xmlSerializer.Serialize(writer, user);
        }
    }
}
