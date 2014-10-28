using System.Text;
using System.Xml;
using System.Xml.Serialization;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class XmlSerializerTests
    {
        public XmlSerializerTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void Account_Can_Serialize()
        {
            var acct = new Account();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Account));
            xmlSerializer.Serialize(writer, acct);
        }

        [Fact]
        public void Blocks_Can_Serialize()
        {
            var block = new Blocks();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Blocks));
            xmlSerializer.Serialize(writer, block);
        }

        [Fact]
        public void DirectMessage_Can_Serialize()
        {
            var dm = new DirectMessage();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(DirectMessage));
            xmlSerializer.Serialize(writer, dm);
        }

        [Fact]
        public void Favorites_Can_Serialize()
        {
            var favorite = new Favorites();
            var stringBuilder = new StringBuilder();
            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Favorites));

            xmlSerializer.Serialize(writer, favorite);
        }

        [Fact]
        public void Friendship_Can_Serialize()
        {
            var friend = new Friendship();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Friendship));
            xmlSerializer.Serialize(writer, friend);
        }

        [Fact]
        public void Place_Can_Serialize()
        {
            var place = new Place();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Place));
            xmlSerializer.Serialize(writer, place);
        }

        [Fact]
        public void Geometry_Can_Serialize()
        {
            var geo = new Geometry();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Geometry));
            xmlSerializer.Serialize(writer, geo);
        }

        [Fact]
        public void IDList_Can_Serialize()
        {
            var ids = new IDList();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(IDList));
            xmlSerializer.Serialize(writer, ids);
        }

        [Fact]
        public void List_Can_Serialize()
        {
            var list = new List();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(List));
            xmlSerializer.Serialize(writer, list);
        }

        [Fact]
        public void Raw_Can_Serialize()
        {
            var raw = new Raw();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Raw));
            xmlSerializer.Serialize(writer, raw);
        }

        [Fact]
        public void SavedSearch_Can_Serialize()
        {
            var saveSrch = new SavedSearch();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(SavedSearch));
            xmlSerializer.Serialize(writer, saveSrch);
        }

        [Fact]
        public void Search_Can_Serialize()
        {
            var search = new Search();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Search));
            xmlSerializer.Serialize(writer, search);
        }

        [Fact]
        public void SocialGraph_Can_Serialize()
        {
            var graph = new SocialGraph();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(SocialGraph));
            xmlSerializer.Serialize(writer, graph);
        }

        [Fact]
        public void Status_Can_Serialize()
        {
            var tweet = new Status {Type = StatusType.Home};
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Status));
            xmlSerializer.Serialize(writer, tweet);
        }

        [Fact]
        public void Trend_Can_Serialize()
        {
            var trend = new Trend();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(Trend));
            xmlSerializer.Serialize(writer, trend);
        }

        [Fact]
        public void User_Can_Serialize()
        {
            var user = new User();
            var stringBuilder = new StringBuilder();

            var writer = XmlWriter.Create(stringBuilder);
            var xmlSerializer = new XmlSerializer(typeof(User));
            xmlSerializer.Serialize(writer, user);
        }
    }
}
