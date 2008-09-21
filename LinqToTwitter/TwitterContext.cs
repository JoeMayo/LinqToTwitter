/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 ***********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace LinqToTwitter
{
    public class TwitterContext
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }

        public TwitterContext() : 
            this(string.Empty, string.Empty, string.Empty) { }

        public TwitterContext(string userName, string password) :
            this(userName, password, string.Empty) { }

        public TwitterContext(string userName, string password, string baseUrl)
        {
            UserName = userName;
            Password = password;
            BaseUrl = baseUrl == string.Empty ? "http://twitter.com/" : baseUrl;
        }


        public TwitterQueryable<Status> Status 
        {
            get
            {
                return new TwitterQueryable<Status>(this);
            }
        }

        internal object Execute(Expression expression, bool IsEnumerable)
        {
            Dictionary<string, string> parameters = null;

            var whereFinder = new InnermostWhereFinder();
            var whereExpression = whereFinder.GetInnermostWhere(expression);

            if (whereExpression != null)
            {
                var lambdaExpression = 
                    (LambdaExpression)
                    ((UnaryExpression)(whereExpression.Arguments[1])).Operand;

                lambdaExpression = 
                    (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

                var paramFinder =
                    new ParameterFinder<Status>(
                        lambdaExpression.Body,
                        new List<string> { "Type" });

                parameters = paramFinder.Parameters; 
            }

            var statusList = GetStatusList(parameters);

            var queryableStatusList = statusList.AsQueryable<Status>();

            return queryableStatusList;
        }

        private List<Status> GetStatusList(Dictionary<string, string> parameters)
        {
            var url = BuildUrl<Status>(parameters);
            var statusList = QueryTwitter(url);

            return statusList;
        }

        private string BuildUrl<T>(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null ||
                !parameters.ContainsKey("Type"))
            {
                url = BaseUrl + "statuses/public_timeline.xml";
                return url;
            }

            switch (typeof(T).Name)
            {
                case "Status":
                    switch (parameters["Type"])
                    {
                        case "Public":
                            url = BaseUrl + "statuses/public_timeline.xml";
                            break;
                        case "Friends":
                            url = BaseUrl + "statuses/friends_timeline.xml";
                            break;
                        default:
                            url = BaseUrl + "statuses/public_timeline.xml";
                            break;
                    }
                    break;
                default:
                    url = BaseUrl + "statuses/public_timeline.xml";
                    break;
            }

            return url;
        }

        private List<Status> QueryTwitter(string url)
        {
            var req = HttpWebRequest.Create(url);
            req.Credentials = new NetworkCredential(UserName, Password);
            var resp = req.GetResponse();
            var strm = resp.GetResponseStream();
            var strmRdr = new StreamReader(strm);
            var txtRdr = new StringReader(strmRdr.ReadToEnd());
            var statusXml = XElement.Load(txtRdr);

            var statusList =
                from status in statusXml.Elements("status")
                let dateParts = 
                    status.Element("created_at").Value.Split(' ')
                let createdAtDate = 
                    DateTime.Parse(
                        string.Format("{0} {1} {2} {3} GMT", 
                        dateParts[1], 
                        dateParts[2], 
                        dateParts[5], 
                        dateParts[3]))
                let user = status.Element("user")
                select
                   new Status
                   {
                       CreatedAt = createdAtDate,
                       Favorited = 
                        bool.Parse(
                            string.IsNullOrEmpty(status.Element("favorited").Value) ?
                            "true" :
                            status.Element("favorited").Value),
                       ID = status.Element("id").Value,
                       InReplyToStatusID = status.Element("in_reply_to_status_id").Value,
                       InReplyToUserID = status.Element("in_reply_to_user_id").Value,
                       Source = status.Element("source").Value,
                       Text = status.Element("text").Value,
                       Truncated = bool.Parse(status.Element("truncated").Value),
                       User =
                           new User
                           {
                               Description = user.Element("description").Value,
                               FollowersCount = int.Parse(user.Element("followers_count").Value),
                               ID = user.Element("id").Value,
                               Location = user.Element("location").Value,
                               Name = user.Element("name").Value,
                               ProfileImageUrl = user.Element("profile_image_url").Value,
                               Protected = bool.Parse(user.Element("protected").Value),
                               ScreenName = user.Element("screen_name").Value,
                               URL = user.Element("url").Value
                           }
                   };

            return statusList.ToList();
        }
    }
}