using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

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
        /// ID of DM
        /// </summary>
        private ulong ID { get; set; }

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
                       "Count",
                       "ID"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<DirectMessageType>(parameters["Type"]);

            switch (Type)
            {
                case DirectMessageType.SentBy:
                    return BuildSentByUrl(parameters);
                case DirectMessageType.SentTo:
                    return BuildSentToUrl(parameters);
                case DirectMessageType.Show:
                    return BuildShowUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        private Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("ID"))
                throw new ArgumentNullException("ID", "ID is required.");

            ID = ulong.Parse(parameters["ID"]);

            var url = BuildUrlHelper.TransformIDUrl(parameters, "direct_messages/show.xml");
            return new Request(BaseUrl + url);
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent to a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private Request BuildSentToUrl(Dictionary<string, string> parameters)
        {
            return BuildSentUrlParameters(parameters, "direct_messages.xml");
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent by a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private Request BuildSentByUrl(Dictionary<string, string> parameters)
        {
            return BuildSentUrlParameters(parameters, "direct_messages/sent.xml");
        }

        /// <summary>
        /// common code for building parameter list for both sent by and sent to urls
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <param name="url">url to start with</param>
        /// <returns>new url with parameters</returns>
        private Request BuildSentUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters == null)
                return req;

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add(new QueryParameter("since_id", SinceID.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add(new QueryParameter("max_id", MaxID.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", Page.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", Count.ToString(CultureInfo.InvariantCulture)));
            }

            return req;
        }

        /// <summary>
        /// transforms XML into IQueryable of DirectMessage
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of DirectMessage</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            if (string.IsNullOrEmpty(responseXml))
            {
                responseXml = "<direct_messages></direct_messages>";
            }

            XElement twitterResponse = XElement.Parse(responseXml);
            var responseItems = twitterResponse.Elements("direct_message").ToList();

            // if we get only a single response back,
            // make sure we get it
            if (twitterResponse.Name == "direct_message")
            {
                responseItems.Add(twitterResponse);
            }

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
                    Sender = User.CreateUser(sender),
                    Recipient = User.CreateUser(recipient)
                };

            return dmList.OfType<T>().ToList();
        }

        #endregion
    }
}
