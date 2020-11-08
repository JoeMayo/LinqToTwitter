using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using System.Text.Json;

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
        public ListType Type { get; set; }

        /// <summary>
        /// Helps page results
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// List ID
        /// </summary>
        public ulong ListID { get; set; }

        /// <summary>
        /// Catchword for list
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// ID of List Owner
        /// </summary>
        public ulong OwnerID { get; set; }

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
        /// Add entities to tweets (default: true)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Add retweets, in addition to normal tweets
        /// </summary>
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Only returns lists that belong to authenticated 
        /// user or user identified by ID or ScreenName
        /// </summary>
        public bool FilterToOwnedLists { get; set; }

        /// <summary>
        /// Don't include statuses in response
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Causes Twitter to return the lists owned by the authenticated user first (Query Filter)
        /// </summary>
        public bool Reverse { get; set; }

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
                       "FilterToOwnedLists",
                       "SkipStatus",
                       "Reverse"
                   })
                   .Parameters;

            if (parameters.ContainsKey("Cursor"))
                Cursor = long.Parse(parameters["Cursor"]);

            if (parameters.ContainsKey("UserID"))
                UserID = ulong.Parse(parameters["UserID"]);

            if (parameters.ContainsKey("ScreenName"))
                ScreenName = parameters["ScreenName"];

            if (parameters.ContainsKey("ListID"))
                ListID = ulong.Parse(parameters["ListID"]);

            if (parameters.ContainsKey("Slug"))
                Slug = parameters["Slug"];

            if (parameters.ContainsKey("OwnerID"))
                OwnerID = ulong.Parse(parameters["OwnerID"]);

            if (parameters.ContainsKey("OwnerScreenName"))
                OwnerScreenName = parameters["OwnerScreenName"];

            if (parameters.ContainsKey("MaxID"))
                MaxID = ulong.Parse(parameters["MaxID"]);

            if (parameters.ContainsKey("Count"))
                Count = int.Parse(parameters["Count"]);

            if (parameters.ContainsKey("Page"))
                Page = int.Parse(parameters["Page"]);

            if (parameters.ContainsKey("SinceID"))
                SinceID = ulong.Parse(parameters["SinceID"]);

            if (parameters.ContainsKey("TrimUser"))
                TrimUser = bool.Parse(parameters["TrimUser"]);

            if (parameters.ContainsKey("IncludeEntities"))
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);

            if (parameters.ContainsKey("IncludeRetweets"))
                IncludeRetweets = bool.Parse(parameters["IncludeRetweets"]);

            if (parameters.ContainsKey("FilterToOwnedLists"))
                FilterToOwnedLists = bool.Parse(parameters["FilterToOwnedLists"]);

            if (parameters.ContainsKey("Reverse"))
                Reverse = bool.Parse(parameters["Reverse"]);

            return parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(TypeParam))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<ListType>(parameters[TypeParam]);

            switch (Type)
            {
                case ListType.List:
                    return BuildListUrl(parameters);
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
                case ListType.IsSubscriber:
                    return BuildIsSubcribedUrl(parameters);
                case ListType.Ownerships:
                    return BuildOwnershipsUrl(parameters);
                default:
                    throw new ArgumentException("Invalid ListType", TypeParam);
            }
        }

        /// <summary>
        /// Builds URL to retrieve all of a user's lists.
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        Request BuildListUrl(Dictionary<string, string> parameters)
        {
            const string UserIDOrScreenNameParam = "UserIdOrScreenName";
            if (!(parameters.ContainsKey("UserID") && UserID != 0) &&
                !(parameters.ContainsKey("ScreenName") && !string.IsNullOrWhiteSpace(parameters["ScreenName"])))
                throw new ArgumentException("Either UserID or ScreenName are required.", UserIDOrScreenNameParam);

            var req = new Request(BaseUrl + "lists/list.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("Reverse"))
            {
                Reverse = bool.Parse(parameters["Reverse"]);
                urlParams.Add(new QueryParameter("reverse", parameters["Reverse"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Builds URL to retrieve info on a specific List.
        /// </summary>
        /// <param name="parameters">Contains ID for List</param>
        /// <returns>URL for List query</returns>
        Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrWhiteSpace(parameters["ListID"])) &&
                (!parameters.ContainsKey("Slug") || string.IsNullOrWhiteSpace(parameters["Slug"])))
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrWhiteSpace(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrWhiteSpace(parameters["OwnerScreenName"])))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);

            var req = new Request(BaseUrl + @"lists/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"];
                urlParams.Add(new QueryParameter("slug", parameters["Slug"]));
            }

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = ulong.Parse(parameters["OwnerID"]);
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = ulong.Parse(parameters["ListID"]);
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            return req;
        }

        /// <summary>
        /// Build url for getting statuses for a list.
        /// </summary>
        /// <param name="parameters">Contains ListID and optionally MaxID, SinceID, Count, and Page</param>
        /// <returns>URL for statuses query</returns>
        Request BuildStatusesUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrWhiteSpace(parameters["ListID"])) &&
                (!parameters.ContainsKey("Slug") || string.IsNullOrWhiteSpace(parameters["Slug"])))
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrWhiteSpace(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrWhiteSpace(parameters["OwnerScreenName"])))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);

            var req = new Request(BaseUrl + "lists/statuses.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = ulong.Parse(parameters["OwnerID"]);
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
                ListID = ulong.Parse(parameters["ListID"]);
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
                // TODO: twitter seems to be ignoring the documented "count=", but does honor "per_page="
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
                TrimUser = bool.Parse(parameters["TrimUser"]);
                urlParams.Add(new QueryParameter("trim_user", parameters["TrimUser"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeRetweets"))
            {
                IncludeRetweets = bool.Parse(parameters["IncludeRetweets"]);
                urlParams.Add(new QueryParameter("include_rts", parameters["IncludeRetweets"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Build url for getting list memberships.
        /// </summary>
        /// <param name="parameters">NoChange required</param>
        /// <returns>URL for memberships query</returns>
        Request BuildMembershipsUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("UserID") && !string.IsNullOrWhiteSpace(parameters["UserID"])) &&
                !(parameters.ContainsKey("ScreenName") && !string.IsNullOrWhiteSpace(parameters["ScreenName"])))
                throw new ArgumentException("Either UserID or ScreenName are required.", UserIdOrScreenName);

            var req = new Request(BaseUrl + "lists/memberships.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("FilterToOwnedLists"))
            {
                if (RequestProcessorHelper.FlagTrue(parameters, "FilterToOwnedLists"))
                {
                    FilterToOwnedLists = true;
                    urlParams.Add(new QueryParameter("filter_to_owned_lists", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Build url for getting list subscriptions.
        /// </summary>
        /// <param name="parameters">NoChange required</param>
        /// <returns>URL for subscriptions query</returns>
        Request BuildSubscriptionsUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("UserID") && !string.IsNullOrWhiteSpace(parameters["UserID"])) &&
                !(parameters.ContainsKey("ScreenName") && !string.IsNullOrWhiteSpace(parameters["ScreenName"])))
                throw new ArgumentException("Either UserID or ScreenName are required.", UserIdOrScreenName);

            var req = new Request(BaseUrl + "lists/subscriptions.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        /// <summary>
        /// Build url for getting a list of members for a list
        /// </summary>
        /// <param name="parameters">Contains ListID and optionally Cursor</param>
        /// <returns>URL for members query</returns>
        Request BuildMembersUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrWhiteSpace(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrWhiteSpace(parameters["Slug"])))
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrWhiteSpace(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrWhiteSpace(parameters["OwnerScreenName"])))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);
            }

            var req = new Request(BaseUrl + "lists/members.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = ulong.Parse(parameters["OwnerID"]);
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
                ListID = ulong.Parse(parameters["ListID"]);
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                if (RequestProcessorHelper.FlagTrue(parameters, "SkipStatus"))
                {
                    SkipStatus = true;
                    urlParams.Add(new QueryParameter("skip_status", "true"));
                }
            }

			if (parameters.ContainsKey("Count"))
			{
				Count = int.Parse(parameters["Count"]);
				urlParams.Add(new QueryParameter("count", parameters["Count"]));
			}

            return req;
        }

        /// <summary>
        /// Build url that determines if a user is a member of a list.
        /// </summary>
        /// <param name="parameters">Contains ID and ListID</param>
        /// <returns>URL for list members query</returns>
        Request BuildIsMemberUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("UserID") || string.IsNullOrWhiteSpace(parameters["UserID"])) &&
               (!parameters.ContainsKey("ScreenName") || string.IsNullOrWhiteSpace(parameters["ScreenName"])))
                throw new ArgumentException("You must specify either UserID or ScreenName of the user you're checking.", UserIdOrScreenName);

            if ((!parameters.ContainsKey("ListID") || string.IsNullOrWhiteSpace(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrWhiteSpace(parameters["Slug"])))
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrWhiteSpace(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrWhiteSpace(parameters["OwnerScreenName"])))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);

            var req = new Request(BaseUrl + "lists/members/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
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
                OwnerID = ulong.Parse(parameters["OwnerID"]);
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = ulong.Parse(parameters["ListID"]);
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                if (RequestProcessorHelper.FlagTrue(parameters, "SkipStatus"))
                {
                    SkipStatus = true;
                    urlParams.Add(new QueryParameter("skip_status", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Builds an URL to retrieve subscribers of a list.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>URL for list subscribers query</returns>
        Request BuildSubscribersUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("ListID") || string.IsNullOrWhiteSpace(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrWhiteSpace(parameters["Slug"])))
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrWhiteSpace(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrWhiteSpace(parameters["OwnerScreenName"])))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);

            var req = new Request(BaseUrl + "lists/subscribers.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("OwnerID"))
            {
                OwnerID = ulong.Parse(parameters["OwnerID"]);
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
                ListID = ulong.Parse(parameters["ListID"]);
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                if (RequestProcessorHelper.FlagTrue(parameters, "SkipStatus"))
                {
                    SkipStatus = true;
                    urlParams.Add(new QueryParameter("skip_status", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Build URL to see if user is subscribed to a list.
        /// </summary>
        /// <param name="parameters">Should contain ID and ListID</param>
        /// <returns>URL for IsSubscriber query</returns>
        Request BuildIsSubcribedUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("UserID") || string.IsNullOrWhiteSpace(parameters["UserID"])) &&
               (!parameters.ContainsKey("ScreenName") || string.IsNullOrWhiteSpace(parameters["ScreenName"])))
                throw new ArgumentException("You must specify either UserID or ScreenName of the user you're checking.", UserIdOrScreenName);

            if ((!parameters.ContainsKey("ListID") || string.IsNullOrWhiteSpace(parameters["ListID"])) &&
               (!parameters.ContainsKey("Slug") || string.IsNullOrWhiteSpace(parameters["Slug"])))
                throw new ArgumentException("You must specify either ListID or Slug.", ListIdOrSlugParam);

            if (parameters.ContainsKey("Slug") &&
                !(parameters.ContainsKey("OwnerID") && !string.IsNullOrWhiteSpace(parameters["OwnerID"])) &&
                !(parameters.ContainsKey("OwnerScreenName") && !string.IsNullOrWhiteSpace(parameters["OwnerScreenName"])))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIdOrOwnerScreenName);

            var req = new Request(BaseUrl + "lists/subscribers/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
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
                OwnerID = ulong.Parse(parameters["OwnerID"]);
                urlParams.Add(new QueryParameter("owner_id", parameters["OwnerID"]));
            }

            if (parameters.ContainsKey("OwnerScreenName"))
            {
                OwnerScreenName = parameters["OwnerScreenName"];
                urlParams.Add(new QueryParameter("owner_screen_name", parameters["OwnerScreenName"]));
            }

            if (parameters.ContainsKey("ListID"))
            {
                ListID = ulong.Parse(parameters["ListID"]);
                urlParams.Add(new QueryParameter("list_id", parameters["ListID"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                if (RequestProcessorHelper.FlagTrue(parameters, "SkipStatus"))
                {
                    SkipStatus = true;
                    urlParams.Add(new QueryParameter("skip_status", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// Build URL to see if user is subscribed to a list.
        /// </summary>
        /// <param name="parameters">Should contain ID and ListID</param>
        /// <returns>URL for IsSubscriber query</returns>
        Request BuildOwnershipsUrl(Dictionary<string, string> parameters)
        {
            if ((!parameters.ContainsKey("UserID") || string.IsNullOrWhiteSpace(parameters["UserID"])) &&
               (!parameters.ContainsKey("ScreenName") || string.IsNullOrWhiteSpace(parameters["ScreenName"])))
                throw new ArgumentException("You must specify either UserID or ScreenName of the user you're checking.", UserIdOrScreenName);

            var req = new Request(BaseUrl + "lists/ownerships.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        /// <summary>
        /// Transforms Twitter response into List.
        /// </summary>
        /// <param name="responseJson">Json Twitter response</param>
        /// <returns>List of List</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            JsonElement listJson = JsonDocument.Parse(responseJson).RootElement;

            List<List> lists;
            switch (Type)
            {
                case ListType.List:
                case ListType.Memberships:
                case ListType.Subscriptions:
                case ListType.Ownerships:
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
                case ListType.IsSubscriber:
                    lists = HandleSingleUserResponse(listJson);
                    break;
                default:
                    lists = new List<List>();
                    break;
            }

            Cursors cursors = null;
            if (listJson.ValueKind == JsonValueKind.Object)
                cursors = new Cursors(listJson);

            foreach (var list in lists)
            {
                list.Type = Type;
                list.Cursor = Cursor;
                list.UserID = UserID;
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
                list.SkipStatus = SkipStatus;
                list.Reverse = Reverse;
            }

            return lists.AsEnumerable().OfType<T>().ToList();
        }
  
        private List<List> HandleSingleListResponse(JsonElement listJson)
        {
            var lists = new List<List>
            {
                new List(listJson)
            };

            return lists;
        }
  
        List<List> HandleMultipleListsResponse(JsonElement listJson)
        {
            var lists =
                (from list in listJson.EnumerateArray()
                 select new List(list))
                .ToList();

            return lists;
        }
  
        List<List> HandleSingleUserResponse(JsonElement listJson)
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

        List<List> HandleMultipleUsersResponse(JsonElement listJson)
        {
            var lists = new List<List>
            {
                new List
                {
                    Users =
                        (from user in listJson.GetProperty("users").EnumerateArray()
                         select new User(user))
                        .ToList()
                }
            };

            return lists;
        }

        private List<List> HandleStatusesResponse(JsonElement listJson)
        {
            var lists = new List<List>
            {
                new List
                {
                    Statuses =
                        (from status in listJson.EnumerateArray()
                         select new Status(status))
                        .ToList()
                }
            };

            return lists;
        }

        /// <summary>
        /// Transforms json into an action response.
        /// </summary>
        /// <param name="responseJson">json with Twitter response</param>
        /// <param name="theAction">Used to specify side-effect methods</param>
        /// <returns>Action response</returns>
        public virtual T ProcessActionResult(string responseJson, Enum theAction)
        {
            List list = null;

            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                JsonElement listJson = JsonDocument.Parse(responseJson).RootElement;

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
                    case ListAction.DestroyAll:
                        list = new List(listJson);
                        break;
                    default:
                        throw new InvalidOperationException(
                            "The default case of ProcessActionResult should never execute because a Type must be specified.");
                }
            }

            return list.ItemCast(default(T));
        }
    }
}
