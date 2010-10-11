using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Collections;

namespace LinqToTwitter
{
    public class ListRequestProcessor<T> : IRequestProcessor<T>
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of list to query
        /// </summary>
        private ListType Type { get; set; }

        /// <summary>
        /// Helps page results
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// List ID or Slug
        /// </summary>
        public string ListID { get; set; }

        /// <summary>
        /// Max ID to retrieve for statuses
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// Number of statuses per page
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Page number for statuses
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// ScreenName of user for query
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Statuses since status ID
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var parameters =
               new ParameterFinder<List>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ScreenName",
                       "Cursor",
                       "ID",
                       "ListID",
                       "MaxID",
                       "PerPage",
                       "Page",
                       "SinceID"
                   })
                   .Parameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
            }

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
            }

            if (parameters.ContainsKey("PerPage"))
            {
                PerPage = int.Parse(parameters["PerPage"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
            }

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

            if (!parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentException("You must specify the user's ScreenName for your query.", "ScreenName");
            }

            ScreenName = parameters["ScreenName"];

            // all List api requests are based on a user's screen name
            BaseUrl += ScreenName + @"/";

            Type = RequestProcessorHelper.ParseQueryEnumType<ListType>(parameters["Type"]);

            switch (Type)
            {
                case ListType.All:
                    url = BuildAllUrl(parameters);
                    break;
                case ListType.Lists:
                    url = BuildListsUrl(parameters);
                    break;
                case ListType.List:
                    url = BuildListUrl(parameters);
                    break;
                case ListType.Statuses:
                    url = BuildStatusesUrl(parameters);
                    break;
                case ListType.Memberships:
                    url = BuildMembershipsUrl(parameters);
                    break;
                case ListType.Subscriptions:
                    url = BuildSubscriptionsUrl(parameters);
                    break;
                case ListType.Members:
                    url = BuildMembersUrl(parameters);
                    break;
                case ListType.IsMember:
                    url = BuildIsMemberUrl(parameters);
                    break;
                case ListType.Subscribers:
                    url = BuildSubscribersUrl(parameters);
                    break;
                case ListType.IsSubscribed:
                    url = BuildIsSubcribedUrl(parameters);
                    break;
                default:
                    break;
            }

            return url;
        }

        /// <summary>
        /// Builds URL to retrieve all lists a user is subscribed to
        /// </summary>
        /// <param name="parameters">ScreenName or UserID</param>
        /// <returns>Url of requesting user's subscribed lists</returns>
        private string BuildAllUrl(Dictionary<string, string> parameters)
        {
            var urlParams = new List<string>();

            string url = BaseUrl.Replace(ScreenName + "/", "") + @"lists/all.xml";

            if (parameters.ContainsKey("UserID"))
            {
                ID = parameters["UserID"];
                urlParams.Add("user_id=" + parameters["UserID"]);
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add("screen_name=" + parameters["ScreenName"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }


        /// <summary>
        /// Builds URL to retrieve all of a user's lists
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        private string BuildListsUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl + "lists.xml?";

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                url += "cursor=" + parameters["Cursor"] + "&";
            }

            url = url.Substring(0, url.Length - 1);
            return url;
        }

        /// <summary>
        /// Builds URL to retrieve info on a specific List
        /// </summary>
        /// <param name="parameters">Contains ID for List</param>
        /// <returns>URL for List query</returns>
        private string BuildListUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl + @"lists.xml";

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }
            else
            {
                throw new ArgumentException("ID (user ID) is required for List query.");
            }

            return url;
        }

        /// <summary>
        /// Build url for getting statuses for a list
        /// </summary>
        /// <param name="parameters">Contains ListID and optionally MaxID, SinceID, PerPage, and Page</param>
        /// <returns>URL for statuses query</returns>
        private string BuildStatusesUrl(Dictionary<string, string> parameters)
        {
            // From patch #4628 (MikeLang on codeplex.com):
            //  The previous implementation wasn't putting the '?' separator 
            //  between the query string and the rest of the url.
            var urlParams = new List<string>();

            string url = BaseUrl + @"lists/";

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                url += parameters["ListID"] + "/statuses.xml";
            }
            else
            {
                throw new ArgumentException("ListID is required for Members query.");
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add("max_id=" + parameters["MaxID"]);
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add("since_id=" + parameters["SinceID"]);
            }

            if (parameters.ContainsKey("PerPage"))
            {
                PerPage = int.Parse(parameters["PerPage"]);
                urlParams.Add("per_page=" + parameters["PerPage"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// Build url for getting list memberships
        /// </summary>
        /// <param name="parameters">None required</param>
        /// <returns>URL for memberships query</returns>
        private string BuildMembershipsUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl + @"lists/memberships.xml?";

            if (parameters.ContainsKey("Cursor") && !string.IsNullOrEmpty(parameters["Cursor"]))
            {
                Cursor = parameters["Cursor"];
                url += "cursor=" + parameters["Cursor"] + "&";
            }

            url = url.Substring(0, url.Length - 1);
            return url;
        }

        /// <summary>
        /// Build url for getting list subscriptions
        /// </summary>
        /// <param name="parameters">None required</param>
        /// <returns>URL for subscriptions query</returns>
        private string BuildSubscriptionsUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl + @"lists/subscriptions.xml?";

            if (parameters.ContainsKey("Cursor") && !string.IsNullOrEmpty(parameters["Cursor"]))
            {
                Cursor = parameters["Cursor"];
                url += "cursor=" + parameters["Cursor"] + "&";
            }

            url = url.Substring(0, url.Length - 1);
            return url;
        }

        /// <summary>
        /// Build url for getting a list of members for a list
        /// </summary>
        /// <param name="parameters">Contains ListID and optionally Cursor</param>
        /// <returns>URL for members query</returns>
        private string BuildMembersUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl;

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                url += parameters["ListID"] + "/members.xml?";
            }
            else
            {
                throw new ArgumentException("ListID is required for Members query.");
            }

            if (parameters.ContainsKey("Cursor") && !string.IsNullOrEmpty(parameters["Cursor"]))
            {
                Cursor = parameters["Cursor"];
                url += "cursor=" + parameters["Cursor"] + "&";
            }

            url = url.Substring(0, url.Length - 1);
            return url;
        }

        /// <summary>
        /// Build url that determines if a user is a member of a list
        /// </summary>
        /// <param name="parameters">Contains ID and ListID</param>
        /// <returns>URL for list members query</returns>
        private string BuildIsMemberUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl;

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                url += parameters["ListID"] + "/members.xml";
            }
            else
            {
                throw new ArgumentException("ListID is required for IsMember query.");
            }

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }
            else
            {
                throw new ArgumentException("ID (user ID) is required for IsMember query.");
            }

            return url;
        }

        /// <summary>
        /// Builds an URL to retrieve subscribers of a list
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>URL for list subscribers query</returns>
        private string BuildSubscribersUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl;

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                url += parameters["ListID"] + "/subscribers.xml?";
            }
            else
            {
                throw new ArgumentException("ListID is required for Subscribers query.");
            }

            if (parameters.ContainsKey("Cursor") && !string.IsNullOrEmpty(parameters["Cursor"]))
            {
                Cursor = parameters["Cursor"];
                url += "cursor=" + parameters["Cursor"] + "&";
            }

            url = url.Substring(0, url.Length - 1);
            return url;
        }

        /// <summary>
        /// Build URL to see if user is subscribed to a list
        /// </summary>
        /// <param name="parameters">Should contain ID and ListID</param>
        /// <returns>URL for IsSubscribed query</returns>
        private string BuildIsSubcribedUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl;

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                url += parameters["ListID"] + "/subscribers.xml";
            }
            else
            {
                throw new ArgumentException("ListID is required for IsSubscribed query.");
            }

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }
            else
            {
                throw new ArgumentException("ID (user ID) is required for IsSubscribed query.");
            }

            return url;
        }

        /// <summary>
        /// transforms XML into IList of List
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>IList of List</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
            List<List> lists = new List<List>();

            if (twitterResponse.Name == "lists_list" || twitterResponse.Name == "lists")
            {
                IEnumerable<XElement> listElements =
                    twitterResponse.Name == "lists_list" ?
                        twitterResponse.Element("lists").Elements("list") :
                        twitterResponse.Elements("list");

                lists =
                    (from list in listElements
                     select new List
                     {
                         Type = Type,
                         Cursor = Cursor,
                         ID = ID,
                         ListID = list.Element("id").Value,
                         PerPage = PerPage,
                         Page = Page,
                         ScreenName = ScreenName,
                         SinceID = SinceID,
                         Name = list.Element("name").Value,
                         FullName = list.Element("full_name").Value,
                         Slug = list.Element("slug").Value,
                         Description = list.Element("description").Value,
                         SubscriberCount = int.Parse(list.Element("subscriber_count").Value),
                         MemberCount = int.Parse(list.Element("member_count").Value),
                         Uri = list.Element("uri").Value,
                         Mode = list.Element("mode").Value,
                         Users = new List<User>
                         {
                             new User().CreateUser(list.Element("user"))
                         },
                         CursorMovement = new Cursors
                         {
                             Next =
                                 twitterResponse.Element("next_cursor") == null ?
                                     string.Empty :
                                     twitterResponse.Element("next_cursor").Value,
                             Previous =
                                 twitterResponse.Element("previous_cursor") == null ?
                                     string.Empty :
                                     twitterResponse.Element("previous_cursor").Value
                         }
                     })
                     .ToList();
            }
            else if (twitterResponse.Name == "list")
            {
                lists.Add(
                    new List
                    {
                        Type = Type,
                        Cursor = Cursor,
                        ID = ID,
                        ListID = twitterResponse.Element("id").Value,
                        PerPage = PerPage,
                        Page = Page,
                        ScreenName = ScreenName,
                        SinceID = SinceID,
                        Name = twitterResponse.Element("name").Value,
                        FullName = twitterResponse.Element("full_name").Value,
                        Slug = twitterResponse.Element("slug").Value,
                        Description = twitterResponse.Element("description").Value,
                        SubscriberCount = int.Parse(twitterResponse.Element("subscriber_count").Value),
                        MemberCount = int.Parse(twitterResponse.Element("member_count").Value),
                        Uri = twitterResponse.Element("uri").Value,
                        Mode = twitterResponse.Element("mode").Value,
                        Users = new List<User>
                         {
                             new User().CreateUser(twitterResponse.Element("user"))
                         },
                        CursorMovement = new Cursors
                        {
                            Next =
                                twitterResponse.Element("next_cursor") == null ?
                                    string.Empty :
                                    twitterResponse.Element("next_cursor").Value,
                            Previous =
                                twitterResponse.Element("previous_cursor") == null ?
                                    string.Empty :
                                    twitterResponse.Element("previous_cursor").Value
                        }
                    });
            }
            else if (twitterResponse.Name == "users_list")
            {
                lists.Add(
                    new List
                    {
                        Type = Type,
                        Cursor = Cursor,
                        ID = ID,
                        ListID = ListID,
                        PerPage = PerPage,
                        Page = Page,
                        ScreenName = ScreenName,
                        SinceID = SinceID,
                        Users = 
                            (from user in twitterResponse.Element("users").Elements("user")
                             select new User().CreateUser(user))
                             .ToList(),
                        CursorMovement = new Cursors
                            {
                                Next = (twitterResponse.Element("next_cursor") == null)
                                        ? string.Empty
                                        : twitterResponse.Element("next_cursor").Value,
                                Previous = (twitterResponse.Element("previous_cursor") == null)
                                        ? string.Empty
                                        : twitterResponse.Element("previous_cursor").Value
                            }
                    });
            }
            else if (twitterResponse.Name == "user")
            {
                lists.Add(
                    new List 
                    {
                        Type = Type,
                        Cursor = Cursor,
                        ID = ID,
                        ListID = ListID,
                        PerPage = PerPage,
                        Page = Page,
                        ScreenName = ScreenName,
                        SinceID = SinceID,
                        Users = new List<User>
                        {
                            new User().CreateUser(twitterResponse)
                        }
                    });
            }
            else if (twitterResponse.Name == "statuses")
            {
                lists.Add(
                    new List
                    {
                        Type = Type,
                        Cursor = Cursor,
                        ID = ID,
                        ListID = ListID,
                        PerPage = PerPage,
                        Page = Page,
                        ScreenName = ScreenName,
                        SinceID = SinceID,
                        Statuses = 
                            (from status in twitterResponse.Elements("status")
                             select new Status().CreateStatus(status))
                             .ToList(),
                        CursorMovement = new Cursors
                            {
                                Next = (twitterResponse.Element("next_cursor") == null)
                                        ? string.Empty
                                        : twitterResponse.Element("next_cursor").Value,
                                Previous = (twitterResponse.Element("previous_cursor") == null)
                                        ? string.Empty
                                        : twitterResponse.Element("previous_cursor").Value
                            }
                    });
            }

            return lists.AsEnumerable().OfType<T>().ToList();
        }
    }
}
