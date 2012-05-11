using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LitJson;

using LinqToTwitter.Common;

namespace LinqToTwitter
{
    public class ListRequestProcessor<T> : 
        IRequestProcessor<T>, 
        IRequestProcessorWantsJson, 
        IRequestProcessorWithAction<T>
        where T : class
    {
        const string TypeParam = "Type";
        const string ListIdOrSlugParam = "ListIdOrSlug";
        const string OwnerIdOrOwnerScreenName = "OwnerIdOrOwnerScreenName";
        const string UserIdOrScreenName = "UserIdOrScreenName";

        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of list to query
        /// </summary>
        internal ListType Type { get; set; }

        /// <summary>
        /// Helps page results
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// List ID
        /// </summary>
        public string ListID { get; set; }

        /// <summary>
        /// Catchword for list
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// ID of List Owner
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// ScreenName of List Owner
        /// </summary>
        public string OwnerScreenName { get; set; }

        /// <summary>
        /// Statuses since status ID
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// Max ID to retrieve for statuses
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// Number of statuses per page
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Page number for statuses
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// ScreenName of user for query
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Truncate all user info, except for ID
        /// </summary>
        public bool TrimUser { get; set; }

        /// <summary>
        /// Add entities to tweets
        /// </summary>
        // TODO: remove after 5/14/12
        [Obsolete("All API methods capable of including entities will return them regardless of the value provided.")]
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Add retweets, in addition to normal tweets
        /// </summary>
        // TODO: remove after 5/14/12
        [Obsolete("All API methods capable of including retweets will return them regardless of the value provided.")]
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Only returns lists that belong to authenticated 
        /// user or user identified by ID or ScreenName
        /// </summary>
        public bool FilterToOwnedLists { get; set; }

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
                       "UserID",
                       "ScreenName",
                       "Cursor",
                       "ListID",
                       "Slug",
                       "OwnerID",
                       "OwnerScreenName",
                       "MaxID",
                       "Count",
                       "Page",
                       "SinceID",
                       "TrimUser",
                       "IncludeEntities",
                       "IncludeRetweets",
                       "FilterToOwnedLists"
                   })
                   .Parameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
            }

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
            }

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = parameters["OwnerID"];
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
            }

            if (parameters.ContainsKey("TrimUser"))
            {
                TrimUser = bool.Parse(parameters["TrimUser"]);
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
            }

            if (parameters.ContainsKey("IncludeRetweets"))
            {
                IncludeRetweets = bool.Parse(parameters["IncludeRetweets"]);
            }

            if (parameters.ContainsKey("FilterToOwnedLists"))
            {
                FilterToOwnedLists = bool.Parse(parameters["FilterToOwnedLists"]);
            }

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(TypeParam))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<ListType>(parameters[TypeParam]);

            switch (Type)
            {
                case ListType.All:
                    return BuildAllUrl(parameters);
                case ListType.Lists:
                    return BuildListsUrl(parameters);
                // TODO: List is deprecated
                //case ListType.List: 
                case ListType.Show:
                    return BuildShowUrl(parameters);
                case ListType.Statuses:
                    return BuildStatusesUrl(parameters);
                case ListType.Memberships:
                    return BuildMembershipsUrl(parameters);
                case ListType.Subscriptions:
                    return BuildSubscriptionsUrl(parameters);
                case ListType.Members:
                    return BuildMembersUrl(parameters);
                case ListType.IsMember:
                    return BuildIsMemberUrl(parameters);
                case ListType.Subscribers:
                    return BuildSubscribersUrl(parameters);
                case ListType.IsSubscribed:
                    return BuildIsSubcribedUrl(parameters);
                default:
                    throw new ArgumentException("Invalid ListType", TypeParam);
            }
        }

        /// <summary>
        /// Builds URL to retrieve all lists a user is subscribed to
        /// </summary>
        /// <param name="parameters">ScreenName or UserID</param>
        /// <returns>Url of requesting user's subscribed lists</returns>
        private Request BuildAllUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "lists/all.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            return req;
        }

        /// <summary>
        /// Builds URL to retrieve all of a user's lists
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        private Request BuildListsUrl(Dictionary<string, string> parameters)
        {
            const string UserIDOrScreenNameParam = "UserIdOrScreenName";
            if (!(parameters.ContainsKey("UserID") && !string.IsNullOrEmpty(parameters["UserID"])) &&
                !(parameters.ContainsKey("ScreenName") && !string.IsNullOrEmpty(parameters["ScreenName"])))
            {
                throw new ArgumentException("Either UserID or ScreenName are required.", UserIDOrScreenNameParam);
            }

            var req = new Request(BaseUrl + "lists.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        /// <summary>
        /// Builds URL to retrieve info on a specific List
        /// </summary>
        /// <param name="parameters">Contains ID for List</param>
        /// <returns>URL for List query</returns>
        private Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrEmpty(parameters["ListID"])) &&
                (!parameters.ContainsKey("Slug") || string.IsNullOrEmpty(parameters["Slug"])))
            {
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);
            }

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrEmpty(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrEmpty(parameters["OwnerScreenName"])))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);
            }

            var req = new Request(BaseUrl + @"lists/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
                urlParams.Add(new QueryParameter("slug", parameters["Slug"]));
            }

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = parameters["OwnerID"];
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            return req;
        }

        /// <summary>
        /// Build url for getting statuses for a list
        /// </summary>
        /// <param name="parameters">Contains ListID and optionally MaxID, SinceID, Count, and Page</param>
        /// <returns>URL for statuses query</returns>
        private Request BuildStatusesUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrEmpty(parameters["ListID"])) &&
                (!parameters.ContainsKey("Slug") || string.IsNullOrEmpty(parameters["Slug"])))
            {
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);
            }

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrEmpty(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrEmpty(parameters["OwnerScreenName"])))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);
            }

            var req = new Request(BaseUrl + "lists/statuses.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = parameters["OwnerID"];
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
                urlParams.Add(new QueryParameter("slug", parameters["Slug"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add(new QueryParameter("since_id", parameters["SinceID"]));
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add(new QueryParameter("max_id", parameters["MaxID"]));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
                // twitter seems to be ignoring the documented "count=", but does honor "per_page="
                // for now, send BOTH
                urlParams.Add(new QueryParameter("per_page", parameters["Count"]));
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", parameters["Page"]));
            }

            if (parameters.ContainsKey("TrimUser"))
            {
                if (FlagTrue(parameters, "TrimUser"))
                {
                    TrimUser = true;
                    urlParams.Add(new QueryParameter("trim_user", "true"));
                }
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                if (FlagTrue(parameters, "IncludeEntities"))
                {
                    IncludeEntities = true;
                    urlParams.Add(new QueryParameter("include_entities", "true"));
                }
            }

            if (parameters.ContainsKey("IncludeRetweets"))
            {
                if (FlagTrue(parameters, "IncludeRetweets"))
                {
                    IncludeRetweets = true;
                    urlParams.Add(new QueryParameter("include_rts", "true"));
                }
            }

            return req;
        }


        /// <summary>
        /// Build url for getting list memberships
        /// </summary>
        /// <param name="parameters">NoChange required</param>
        /// <returns>URL for memberships query</returns>
        private Request BuildMembershipsUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("UserID") && !string.IsNullOrEmpty(parameters["UserID"])) &&
                !(parameters.ContainsKey("ScreenName") && !string.IsNullOrEmpty(parameters["ScreenName"])))
            {
                throw new ArgumentException("Either UserID or ScreenName are required.", UserIdOrScreenName);
            }

            var req = new Request(BaseUrl + "lists/memberships.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("FilterToOwnedLists"))
            {
                if (FlagTrue(parameters, "FilterToOwnedLists"))
                {
                    FilterToOwnedLists = true;
                    urlParams.Add(new QueryParameter("filter_to_owned_lists", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Build url for getting list subscriptions
        /// </summary>
        /// <param name="parameters">NoChange required</param>
        /// <returns>URL for subscriptions query</returns>
        private Request BuildSubscriptionsUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("UserID") && !string.IsNullOrEmpty(parameters["UserID"])) &&
                !(parameters.ContainsKey("ScreenName") && !string.IsNullOrEmpty(parameters["ScreenName"])))
            {
                throw new ArgumentException("Either UserID or ScreenName are required.", UserIdOrScreenName);
            }

            var req = new Request(BaseUrl + "lists/subscriptions.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        /// <summary>
        /// Build url for getting a list of members for a list
        /// </summary>
        /// <param name="parameters">Contains ListID and optionally Cursor</param>
        /// <returns>URL for members query</returns>
        private Request BuildMembersUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrEmpty(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrEmpty(parameters["Slug"])))
            {
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);
            }

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrEmpty(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrEmpty(parameters["OwnerScreenName"])))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);
            }

            var req = new Request(BaseUrl + "lists/members.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = parameters["OwnerID"];
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
                urlParams.Add(new QueryParameter("slug", parameters["Slug"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                if (FlagTrue(parameters, "IncludeEntities"))
                {
                    IncludeEntities = true;
                    urlParams.Add(new QueryParameter("include_entities", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Build url that determines if a user is a member of a list
        /// </summary>
        /// <param name="parameters">Contains ID and ListID</param>
        /// <returns>URL for list members query</returns>
        private Request BuildIsMemberUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("UserID") || string.IsNullOrEmpty(parameters["UserID"])) &&
               (!parameters.ContainsKey("ScreenName") || string.IsNullOrEmpty(parameters["ScreenName"])))
            {
                throw new ArgumentException("You must specify either UserID or ScreenName of the user you're checking.", UserIdOrScreenName);
            }

            if ((!parameters.ContainsKey("ListID") || string.IsNullOrEmpty(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrEmpty(parameters["Slug"])))
            {
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);
            }

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrEmpty(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrEmpty(parameters["OwnerScreenName"])))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);
            }

            var req = new Request(BaseUrl + "lists/members/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
                urlParams.Add(new QueryParameter("slug", parameters["Slug"]));
            }

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = parameters["OwnerID"];
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                if (FlagTrue(parameters, "IncludeEntities"))
                {
                    IncludeEntities = true;
                    urlParams.Add(new QueryParameter("include_entities", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Builds an URL to retrieve subscribers of a list
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>URL for list subscribers query</returns>
        private Request BuildSubscribersUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrEmpty(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrEmpty(parameters["Slug"])))
            {
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);
            }

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrEmpty(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrEmpty(parameters["OwnerScreenName"])))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);
            }

            var req = new Request(BaseUrl + "lists/subscribers.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = parameters["OwnerID"];
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
                urlParams.Add(new QueryParameter("slug", parameters["Slug"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                if (FlagTrue(parameters, "IncludeEntities"))
                {
                    IncludeEntities = true;
                    urlParams.Add(new QueryParameter("include_entities", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Build URL to see if user is subscribed to a list
        /// </summary>
        /// <param name="parameters">Should contain ID and ListID</param>
        /// <returns>URL for IsSubscribed query</returns>
        private Request BuildIsSubcribedUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("UserID") || string.IsNullOrEmpty(parameters["UserID"])) &&
               (!parameters.ContainsKey("ScreenName") || string.IsNullOrEmpty(parameters["ScreenName"])))
            {
                throw new ArgumentException("You must specify either UserID or ScreenName of the user you're checking.", UserIdOrScreenName);
            }

            if ((!parameters.ContainsKey("ListID") || string.IsNullOrEmpty(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrEmpty(parameters["Slug"])))
            {
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);
            }

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrEmpty(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrEmpty(parameters["OwnerScreenName"])))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);
            }

            var req = new Request(BaseUrl + "lists/subscribers/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
                urlParams.Add(new QueryParameter("slug", parameters["Slug"]));
            }

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = parameters["OwnerID"];
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = parameters["ListID"];
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                if (FlagTrue(parameters, "IncludeEntities"))
                {
                    IncludeEntities = true;
                    urlParams.Add(new QueryParameter("include_entities", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Transforms Twitter response into List
        /// </summary>
        /// <param name="responseJson">Json Twitter response</param>
        /// <returns>List of List</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrEmpty(responseJson)) return new List<T>();

            JsonData listJson = JsonMapper.ToObject(responseJson);

            List<List> lists;
            switch (Type)
            {
                case ListType.Lists:
                case ListType.Memberships:
                case ListType.Subscriptions:
                case ListType.All:
                    lists = HandleMultipleListsResponse(listJson);
                    break;
                case ListType.Show:
                    lists = HandleSingleListResponse(listJson);
                    break;
                case ListType.Statuses:
                    lists = HandleStatusesResponse(listJson);
                    break;
                case ListType.Members:
                case ListType.Subscribers:
                    lists = HandleMultipleUsersResponse(listJson);
                    break;
                case ListType.IsMember:
                case ListType.IsSubscribed:
                    lists = HandleSingleUserResponse(listJson);
                    break;
                default:
                    lists = new List<List>();
                    break;
            }

            var cursors = new Cursors(listJson);

            foreach (var list in lists)
            {
                list.Type = Type;
                list.Cursor = Cursor;
                list.UserID = UserID;

                if (String.IsNullOrEmpty(list.ListID) && !String.IsNullOrEmpty(ListID))
                    list.ListID = ListID;

                list.Slug = Slug;
                list.OwnerID = OwnerID;
                list.OwnerScreenName = OwnerScreenName;
                list.MaxID = MaxID;
                list.Count = Count;
                list.Page = Page;
                list.ScreenName = ScreenName;
                list.SinceID = SinceID;
                list.TrimUser = TrimUser;
                list.IncludeEntities = IncludeEntities;
                list.IncludeRetweets = IncludeRetweets;
                list.FilterToOwnedLists = FilterToOwnedLists;
                list.CursorMovement = cursors;
            };

            return lists.AsEnumerable().OfType<T>().ToList();
        }
  
        private List<List> HandleSingleListResponse(JsonData listJson)
        {
            var lists = new List<List>
            {
                new List(listJson)
            };

            return lists;
        }
  
        List<List> HandleMultipleListsResponse(JsonData listJson)
        {
            var lists =
                (from JsonData list in listJson.GetValue<JsonData>("lists")
                 select new List(list))
                .ToList();

            return lists;
        }
  
        List<List> HandleSingleUserResponse(JsonData listJson)
        {
            var lists = new List<List>
            {
                new List
                {
                    Users = new List<User> { new User(listJson) }
                }
            };

            return lists;
        }

        List<List> HandleMultipleUsersResponse(JsonData listJson)
        {
            var lists = new List<List>
            {
                new List
                {
                    Users =
                        (from JsonData user in listJson.GetValue<JsonData>("users")
                         select new User(user))
                        .ToList()
                }
            };

            return lists;
        }

        private List<List> HandleStatusesResponse(JsonData listJson)
        {
            var lists = new List<List>
            {
                new List
                {
                    Statuses =
                        (from JsonData status in listJson
                         select new Status(status))
                        .ToList()
                }
            };

            return lists;
        }

        /// <summary>
        /// transforms json into an action response
        /// </summary>
        /// <param name="responseJson">json with Twitter response</param>
        /// <param name="theAction">Used to specify side-effect methods</param>
        /// <returns>Action response</returns>
        public virtual T ProcessActionResult(string responseJson, Enum theAction)
        {
            List list = null;

            if (!string.IsNullOrEmpty(responseJson))
            {
                JsonData listJson = JsonMapper.ToObject(responseJson);

                switch ((ListAction)theAction)
                {
                    case ListAction.Create:
                    case ListAction.Update:
                    case ListAction.Delete:
                    case ListAction.AddMember:
                    case ListAction.AddMemberRange:
                    case ListAction.DeleteMember:
                    case ListAction.Subscribe:
                    case ListAction.Unsubscribe:
                        list = new List(listJson);
                        break;
                    default:
                        throw new InvalidOperationException("The default case of ProcessActionResult should never execute because a Type must be specified.");
                }
            }

            return list.ItemCast(default(T));
        }

        private static bool FlagTrue(IDictionary<string, string> parameters, string key)
        {
            bool flag;

            if (!bool.TryParse(parameters[key], out flag))
            {
                flag = false;
            }

            return flag;
        }
    }
}
