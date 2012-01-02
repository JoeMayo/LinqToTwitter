using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Xml.Linq;

namespace LinqToTwitterXUnitTests.ListTests
{
    class ListTests
    {
        [Fact]
        void Create_Does_Not_Populate_ListID()
        {
            XElement list = XElement.Parse(this.listXml).Element("lists").Elements("list").First();
            XElement cursor = XElement.Parse(this.listXml);

            var result = LinqToTwitter.List.CreateList(list, cursor);

            Assert.Null(result.ListID);
        }

        [Fact]
        void Create_Populates_ListIDResult()
        {
            XElement list = XElement.Parse(this.listXml).Element("lists").Elements("list").First();
            XElement cursor = XElement.Parse(this.listXml);

            var result = LinqToTwitter.List.CreateList(list, cursor);

            Assert.NotNull(result.ListIDResult);
            Assert.Equal("3897042", result.ListIDResult);
        }

        [Fact]
        void Create_Does_Not_Populate_Slug()
        {
            XElement list = XElement.Parse(this.listXml).Element("lists").Elements("list").First();
            XElement cursor = XElement.Parse(this.listXml);

            var result = LinqToTwitter.List.CreateList(list, cursor);

            Assert.Null(result.Slug);
        }

        [Fact]
        void Create_Populates_SlugResult()
        {
            XElement list = XElement.Parse(this.listXml).Element("lists").Elements("list").First();
            XElement cursor = XElement.Parse(this.listXml);

            var result = LinqToTwitter.List.CreateList(list, cursor);

            Assert.NotNull(result.ListIDResult);
            Assert.Equal("privatelist", result.SlugResult);
        }

        string listXml = @"<lists_list>
  <lists type=""array"">
    <list>
      <id>3897042</id>
      <name>Privatelist</name>
      <full_name>@LinqToTweeter/privatelist</full_name>
      <slug>privatelist</slug>
      <description>This is a private list for testing</description>
      <subscriber_count>0</subscriber_count>
      <member_count>1</member_count>
      <uri>/LinqToTweeter/privatelist</uri>
      <mode>private</mode>
      <user>
        <id>16761255</id>
        <name>LtoT Test</name>
        <screen_name>LinqToTweeter</screen_name>
        <location>Anywhere In The World</location>
        <description>Testing the LINQ to Twitter Account Profile Update.</description>
        <profile_image_url>http://a3.twimg.com/profile_images/197870807/JoeTwitterBW_normal.jpg</profile_image_url>
        <url>http://linqtotwitter.codeplex.com</url>
        <protected>false</protected>
        <followers_count>26</followers_count>
        <profile_background_color>0099B9</profile_background_color>
        <profile_text_color>3C3940</profile_text_color>
        <profile_link_color>0099B9</profile_link_color>
        <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
        <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
        <friends_count>1</friends_count>
        <created_at>Wed Oct 15 05:15:40 +0000 2008</created_at>
        <favourites_count>0</favourites_count>
        <utc_offset>-25200</utc_offset>
        <time_zone>Mountain Time (US &amp; Canada)</time_zone>
        <profile_background_image_url>http://a3.twimg.com/profile_background_images/56043243/200xColor_2.png</profile_background_image_url>
        <profile_background_tile>false</profile_background_tile>
        <statuses_count>77</statuses_count>
        <notifications>false</notifications>
        <geo_enabled>false</geo_enabled>
        <verified>false</verified>
        <following>false</following>
      </user>
    </list>
  </lists>
  <next_cursor>0</next_cursor>
  <previous_cursor>0</previous_cursor>
</lists_list>";

    }
}
