using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Globalization;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Direct Messages
    /// </summary>
    public class DirectMessageRequestProcessor<T> : IRequestProcessor<T>
    {
        #region IRequestProcessor Members

        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// Type of Direct Message
        /// </summary>
        private DirectMessageType Type { get; set; }

        /// <summary>
        /// since this message ID
        /// </summary>
        private ulong SinceID { get; set; }

        /// <summary>
        /// max ID to return
        /// </summary>
        private ulong MaxID { get; set; }

        /// <summary>
        /// page number to return
        /// </summary>
        private int Page { get; set; }

        /// <summary>
        /// number of items to return (works for SentBy and SentTo
        /// </summary>
        private int Count { get; set; }
        
        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<DirectMessage>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "SinceID",
                       "MaxID",
                       "Page",
                       "Count"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<DirectMessageType>(parameters["Type"]);

            switch (Type)
            {
                case DirectMessageType.SentBy:
                    url = BuildSentByUrl(parameters);
                    break;
                case DirectMessageType.SentTo:
                    url = BuildSentToUrl(parameters);
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return url;
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent to a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private string BuildSentToUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "direct_messages.xml";

            if (parameters != null)
            {
                url = BuildSentUrlParameters(parameters, url);
            }

            return url;
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent by a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private string BuildSentByUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "direct_messages/sent.xml";

            if (parameters != null)
            {
                url = BuildSentUrlParameters(parameters, url);
            }

            return url;
        }

        /// <summary>
        /// common code for building parameter list for both sent by and sent to urls
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <param name="url">url to start with</param>
        /// <returns>new url with parameters</returns>
        private string BuildSentUrlParameters(Dictionary<string, string> parameters, string url)
        {
            if (parameters == null)
            {
                return url;
            }

            var urlParams = new List<string>();

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add("since_id=" + parameters["SinceID"]);
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add("max_id=" + parameters["MaxID"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add("count=" + parameters["Count"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        //<direct_message>
        //  <id>87864628</id>
        //  <sender_id>1234567</sender_id>
        //  <text>;)</text>
        //  <recipient_id>15411837</recipient_id>
        //  <created_at>Tue Apr 07 16:47:25 +0000 2009</created_at>
        //  <sender_screen_name>senderscreenname</sender_screen_name>
        //  <recipient_screen_name>JoeMayo</recipient_screen_name>
        //  <sender>
        //    <id>1234567</id>
        //    <name>Sender Name</name>
        //    <screen_name>senderscreenname</screen_name>
        //    <location>SenderLocation</location>
        //    <description>Sender Description</description>
        //    <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/12345678/name_of_image.jpg</profile_image_url>
        //    <url>http://sendersite.com</url>
        //    <protected>false</protected>
        //    <followers_count>10406</followers_count>
        //    <profile_background_color>9ae4e8</profile_background_color>
        //    <profile_text_color>696969</profile_text_color>
        //    <profile_link_color>72412c</profile_link_color>
        //    <profile_sidebar_fill_color>b8aa9c</profile_sidebar_fill_color>
        //    <profile_sidebar_border_color>b8aa9c</profile_sidebar_border_color>
        //    <friends_count>705</friends_count>
        //    <created_at>Tue May 01 05:55:26 +0000 2007</created_at>
        //    <favourites_count>56</favourites_count>
        //    <utc_offset>-28800</utc_offset>
        //    <time_zone>Pacific Time (US &amp; Canada)</time_zone>
        //    <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/2036752/background.gif</profile_background_image_url>
        //    <profile_background_tile>true</profile_background_tile>
        //    <statuses_count>7607</statuses_count>
        //    <notifications>false</notifications>
        //    <following>true</following>
        //  </sender>
        //  <recipient>
        //    <id>15411837</id>
        //    <name>Joe Mayo</name>
        //    <screen_name>JoeMayo</screen_name>
        //    <location>Denver, CO</location>
        //    <description>Author/entrepreneur, specializing in custom .NET software development</description>
        //    <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/62569644/JoeTwitter_normal.jpg</profile_image_url>
        //    <url>http://www.csharp-station.com</url>
        //    <protected>false</protected>
        //    <followers_count>47</followers_count>
        //    <profile_background_color>0099B9</profile_background_color>
        //    <profile_text_color>3C3940</profile_text_color>
        //    <profile_link_color>0099B9</profile_link_color>
        //    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
        //    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
        //    <friends_count>22</friends_count>
        //    <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
        //    <favourites_count>0</favourites_count>
        //    <utc_offset>-25200</utc_offset>
        //    <time_zone>Mountain Time (US &amp; Canada)</time_zone>
        //    <profile_background_image_url>http://static.twitter.com/images/themes/theme4/bg.gif</profile_background_image_url>
        //    <profile_background_tile>false</profile_background_tile>
        //    <statuses_count>137</statuses_count>
        //    <notifications>false</notifications>
        //    <following>false</following>
        //  </recipient>
        //</direct_message>

        /// <summary>
        /// transforms XML into IQueryable of DirectMessage
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of DirectMessage</returns>
        public virtual List<T> ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            var responseItems = twitterResponse.Elements("direct_message").ToList();

            // if we get only a single response back,
            // make sure we get it
            if (twitterResponse.Name == "direct_message")
            {
                responseItems.Add(twitterResponse);
            }

            var user = new User();

            var dmList =
                from dm in responseItems
                let sender =
                    dm.Element("sender")
                let recipient =
                    dm.Element("recipient")
                let createdAtDate = 
                    DateTime.ParseExact(
                        dm.Element("created_at").Value, 
                        "ddd MMM dd HH:mm:ss %zzzz yyyy", 
                        CultureInfo.InvariantCulture)
                select new DirectMessage
                {
                    Type = Type,
                    SinceID = SinceID,
                    MaxID = MaxID,
                    Page = Page,
                    Count = Count,
                    ID = ulong.Parse(dm.Element("id").Value),
                    SenderID = ulong.Parse(dm.Element("sender_id").Value),
                    Text = dm.Element("text").Value,
                    RecipientID = ulong.Parse(dm.Element("recipient_id").Value),
                    CreatedAt = createdAtDate,
                    SenderScreenName = dm.Element("sender_screen_name").Value,
                    RecipientScreenName = dm.Element("recipient_screen_name").Value,
                    Sender = user.CreateUser(sender),
                    Recipient = user.CreateUser(recipient)
                };

            return dmList.OfType<T>().ToList();
        }

        #endregion
    }
}
