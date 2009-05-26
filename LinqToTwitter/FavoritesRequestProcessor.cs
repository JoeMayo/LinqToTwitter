using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Web;
using System.Collections;

namespace LinqToTwitter
{
    /// <summary>
    /// Manages request processing for favorites
    /// </summary>
    public class FavoritesRequestProcessor : IRequestProcessor
    {
        public string BaseUrl { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<Favorites>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Page",
                       "ID"
                   })
                   .Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "favorites.xml";

            url = BuildFavoritesUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters for Favorites request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildFavoritesUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (parameters.ContainsKey("Page"))
            {
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (parameters.ContainsKey("ID"))
            {
                urlParams.Add("id=" + parameters["ID"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

//        <statuses type="array">
//  <status>
//    <created_at>Sat Apr 18 19:35:19 +0000 2009</created_at>
//    <id>1552797863</id>
//    <text>Tip: Follow liberally to start. Follow at least 50 people. Also, a MUST: Add a profile photo ASAP. #newtotwitter</text>
//    <source>&lt;a href="http://www.atebits.com/"&gt;Tweetie&lt;/a&gt;</source>
//    <truncated>false</truncated>
//    <in_reply_to_status_id></in_reply_to_status_id>
//    <in_reply_to_user_id></in_reply_to_user_id>
//    <favorited>true</favorited>
//    <in_reply_to_screen_name></in_reply_to_screen_name>
//    <user>
//      <id>5676102</id>
//      <name>Scott Hanselman</name>
//      <screen_name>shanselman</screen_name>
//      <location>Oregon</location>
//      <description>Programmer, author, speaker, web guy, podcaster, starving stand-up comic, diabetic, Microsoft shill.</description>
//      <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/60143428/hanselman_larger_head_shot_normal.jpg</profile_image_url>
//      <url>http://hanselman.com</url>
//      <protected>false</protected>
//      <followers_count>10957</followers_count>
//      <profile_background_color>9ae4e8</profile_background_color>
//      <profile_text_color>696969</profile_text_color>
//      <profile_link_color>72412c</profile_link_color>
//      <profile_sidebar_fill_color>b8aa9c</profile_sidebar_fill_color>
//      <profile_sidebar_border_color>b8aa9c</profile_sidebar_border_color>
//      <friends_count>763</friends_count>
//      <created_at>Tue May 01 05:55:26 +0000 2007</created_at>
//      <favourites_count>59</favourites_count>
//      <utc_offset>-28800</utc_offset>
//      <time_zone>Pacific Time (US &amp; Canada)</time_zone>
//      <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/2036752/background.gif</profile_background_image_url>
//      <profile_background_tile>true</profile_background_tile>
//      <statuses_count>7901</statuses_count>
//      <notifications>false</notifications>
//      <following>0</following>
//    </user>
//  </status>
//  <status>
//    <created_at>Sat Apr 18 17:42:32 +0000 2009</created_at>
//    <id>1552070127</id>
//    <text>Safe URL Shortener. good for facebook and for twitter. give your friends a heads-up before sending them to a generic link.http://safeURL.to</text>
//    <source>&lt;a href="http://www.tweetdeck.com/"&gt;TweetDeck&lt;/a&gt;</source>
//    <truncated>false</truncated>
//    <in_reply_to_status_id></in_reply_to_status_id>
//    <in_reply_to_user_id></in_reply_to_user_id>
//    <favorited>true</favorited>
//    <in_reply_to_screen_name></in_reply_to_screen_name>
//    <user>
//      <id>21276610</id>
//      <name>Reuven</name>
//      <screen_name>ETZION</screen_name>
//      <location>Jerusalem - City of Gold</location>
//      <description>click the link for coolest twitter page idea ever..</description>
//      <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/183664543/bg-blu_normal.png</profile_image_url>
//      <url>http://retzion.com/?site=twitter</url>
//      <protected>false</protected>
//      <followers_count>427</followers_count>
//      <profile_background_color>3b627e</profile_background_color>
//      <profile_text_color>000000</profile_text_color>
//      <profile_link_color>17163c</profile_link_color>
//      <profile_sidebar_fill_color>6F9BCC</profile_sidebar_fill_color>
//      <profile_sidebar_border_color>000000</profile_sidebar_border_color>
//      <friends_count>927</friends_count>
//      <created_at>Thu Feb 19 04:44:37 +0000 2009</created_at>
//      <favourites_count>2</favourites_count>
//      <utc_offset>-10800</utc_offset>
//      <time_zone>Greenland</time_zone>
//      <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/10005208/twitter-bg-tweety.jpg</profile_background_image_url>
//      <profile_background_tile>false</profile_background_tile>
//      <statuses_count>92</statuses_count>
//      <notifications>false</notifications>
//      <following>0</following>
//    </user>
//  </status>
//</statuses>

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of User</returns>
        public IList ProcessResults(XElement twitterResponse)
        {
            var responseItems = twitterResponse.Elements("status").ToList();

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == "status")
            {
                responseItems.Add(twitterResponse);
            }

            var statusList =
                from status in responseItems
                let createdAtDate =
                    DateTime.ParseExact(
                        status.Element("created_at").Value,
                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        null)
                let user = status.Element("user")
                select
                   new Favorites
                   {
                       CreatedAt = createdAtDate,
                       Favorited =
                        bool.Parse(
                            string.IsNullOrEmpty(status.Element("favorited").Value) ?
                            "true" :
                            status.Element("favorited").Value),
                       ID = int.Parse(status.Element("id").Value),
                       InReplyToStatusID = status.Element("in_reply_to_status_id").Value,
                       InReplyToUserID = status.Element("in_reply_to_user_id").Value,
                       Source = status.Element("source").Value,
                       Text = status.Element("text").Value,
                       Truncated = bool.Parse(status.Element("truncated").Value),
                       InReplyToScreenName =
                        status.Element("in_reply_to_screen_name") == null ?
                            string.Empty :
                            status.Element("in_reply_to_screen_name").Value,
                       User = new User().CreateUser(user)
                   };

            return statusList.ToList();
           //return statusList.AsQueryable<Favorites>();
        }
    }
}
