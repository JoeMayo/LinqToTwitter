using System;
using System.Linq;
using LinqToTwitterTests.Common;
using Xunit;
using LinqToTwitter;
using System.Xml.Linq;

namespace LinqToTwitterXUnitTests.FriendshipTests
{
    class RelationshipTests
    {
        public RelationshipTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void Create_Does_Not_Populate_Following_And_FollowedBy_With_Connection_None()
        {
            XElement noRelationship = XElement.Parse(lookupXml).Elements("relationship").First();

            var relationship = Relationship.CreateRelationship(noRelationship);

            Assert.False(relationship.FollowedBy);
            Assert.False(relationship.Following);
        }

        [Fact]
        public void Create_Populates_Following_And_FollowedBy_With_Connection_Info()
        {
            XElement yesRelationship = XElement.Parse(lookupXml).Elements("relationship").Last();

            var relationship = Relationship.CreateRelationship(yesRelationship);

            Assert.True(relationship.FollowedBy);
            Assert.True(relationship.Following);
        }

        readonly string lookupXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
 <relationships>
 <relationship id=""16761255"">
 <id>16761255</id>
 <name>LINQ to Tweeter Test</name>
 <screen_name>Linq2Tweeter</screen_name>
 <connections>
 <connection>none</connection>
 </connections>
 </relationship>
 <relationship id=""15411837"">
 <id>15411837</id>
 <name>Joe Mayo</name>
 <screen_name>JoeMayo</screen_name>
 <connections>
 <connection>following</connection>
 <connection>followed_by</connection>
 </connections>
 </relationship>
 </relationships>";
    }
}
