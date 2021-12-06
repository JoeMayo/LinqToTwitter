using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class ListRequestProcessor<T> : 
        IRequestProcessor<T>, 
        IRequestProcessorWantsJson
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// type of list to query
        /// </summary>
        public ListType Type { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Comma-separated list of list fields - <see cref="ListField"/>
        /// </summary>
        public string? ListFields { get; set; }

        /// <summary>
        /// List ID
        /// </summary>
        public string? ListID { get; set; }

        /// <summary>
        /// Max number of results
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Used to get the next page of results
        /// </summary>
        public string? PaginationToken { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object - <see cref="UserField"/>
        /// </summary>
        public string? UserFields { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string? UserID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var parameters =
               new ParameterFinder<ListQuery>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(Expansions),
                       nameof(ListFields),
                       nameof(ListID),
                       nameof(MaxResults),
                       nameof(PaginationToken),
                       nameof(UserFields),
                       nameof(UserID),
                   })
                   .Parameters;

            return parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(nameof(Type)))
                throw new ArgumentNullException(nameof(Type), "You must set Type.");

            Type = RequestProcessorHelper.ParseEnum<ListType>(parameters[nameof(Type)]);

            return Type switch
            {
                ListType.Lookup => BuildLookupUrl(parameters),
                ListType.Owned => BuildOwnedUrl(parameters),
                ListType.Member => BuildMemberUrl(parameters),
                ListType.Follow => BuildFollowUrl(parameters),
                ListType.Pin => BuildPinUrl(parameters),
                _ => throw new ArgumentException("Invalid ListType", nameof(Type)),
            };
        }

        // TODO: Move this to common so we can use it everywhere
        /// <summary>
        /// Sets parameter, but doen't treat as a query parameter
        /// </summary>
        /// <example>
        /// //
        /// // Notice how we need UserID as a parameter - we use this pattern a lot.
        /// //
        /// 
        /// SetRequredSegmentParam(parameters, nameof(UserID), val => UserID = val);
        /// 
        /// var req = new Request($"{BaseUrl}users/{UserID}/owned_lists");
        /// 
        /// </example>
        /// <param name="parameters">list of parameters</param>
        /// <param name="paramName">name of parameter containing value to set</param>
        /// <param name="setter">lambda to set property with value</param>
        void SetSegment(
            Dictionary<string, string> parameters, 
            string paramName, 
            Action<string> setter)
        {
            if (parameters.ContainsKey(paramName))
                setter(parameters[paramName]);
            else
                throw new ArgumentException($"{paramName} is required", paramName);
        }

        record ParameterSpec(string ParamName, Action<string> Setter, bool ReplaceWhitespace);

        /// <summary>
        /// Builds URL to retrieve a specified list.
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        Request BuildLookupUrl(Dictionary<string, string> parameters)
        {
            SetSegment(parameters, nameof(ListID), val => ListID = val);

            var req = new Request($"{BaseUrl}lists/{ListID}");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(ListFields)))
            {
                ListFields = parameters[nameof(ListFields)];
                urlParams.Add(new QueryParameter("list.fields", ListFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Builds URL to retrieve a specified list.
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        Request BuildOwnedUrl(Dictionary<string, string> parameters)
        {
            SetSegment(parameters, nameof(UserID), val => UserID = val);

            var req = new Request($"{BaseUrl}users/{UserID}/owned_lists");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(ListFields)))
            {
                ListFields = parameters[nameof(ListFields)];
                urlParams.Add(new QueryParameter("list.fields", ListFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                string maxResultsString = parameters[nameof(MaxResults)];
                _ = int.TryParse(maxResultsString, out var maxResults);
                MaxResults = maxResults;
                urlParams.Add(new QueryParameter("max_results", maxResultsString));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Builds URL to retrieve lists a user is a member of.
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        Request BuildMemberUrl(Dictionary<string, string> parameters)
        {
            SetSegment(parameters, nameof(UserID), val => UserID = val);

            var req = new Request($"{BaseUrl}users/{UserID}/list_memberships");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(ListFields)))
            {
                ListFields = parameters[nameof(ListFields)];
                urlParams.Add(new QueryParameter("list.fields", ListFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                string maxResultsString = parameters[nameof(MaxResults)];
                _ = int.TryParse(maxResultsString, out var maxResults);
                MaxResults = maxResults;
                urlParams.Add(new QueryParameter("max_results", maxResultsString));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Builds URL to retrieve users followed lists.
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        Request BuildFollowUrl(Dictionary<string, string> parameters)
        {
            SetSegment(parameters, nameof(UserID), val => UserID = val);

            var req = new Request($"{BaseUrl}users/{UserID}/followed_lists");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(ListFields)))
            {
                ListFields = parameters[nameof(ListFields)];
                urlParams.Add(new QueryParameter("list.fields", ListFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                string maxResultsString = parameters[nameof(MaxResults)];
                _ = int.TryParse(maxResultsString, out var maxResults);
                MaxResults = maxResults;
                urlParams.Add(new QueryParameter("max_results", maxResultsString));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Builds URL to retrieve user's pinned lists.
        /// </summary>
        /// <param name="parameters">Parameter List</param>
        /// <returns>Base URL + lists request</returns>
        Request BuildPinUrl(Dictionary<string, string> parameters)
        {
            SetSegment(parameters, nameof(UserID), val => UserID = val);

            var req = new Request($"{BaseUrl}users/{UserID}/pinned_lists");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(ListFields)))
            {
                ListFields = parameters[nameof(ListFields)];
                urlParams.Add(new QueryParameter("list.fields", ListFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                string maxResultsString = parameters[nameof(MaxResults)];
                _ = int.TryParse(maxResultsString, out var maxResults);
                MaxResults = maxResults;
                urlParams.Add(new QueryParameter("max_results", maxResultsString));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
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
            IEnumerable<ListQuery> list;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                list = new List<ListQuery> { new ListQuery() };
            }
            else
            {
                var result = JsonDeserialize(responseJson);
                list = new List<ListQuery> { result };
            }

            return list.OfType<T>().ToList();
        }

        ListQuery JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            ListQuery? list = JsonSerializer.Deserialize<ListQuery>(responseJson, options);

            if (list == null)
                return new ListQuery
                {
                    Type = Type,
                    Expansions = Expansions,
                    ListFields = ListFields,
                    ListID = ListID,
                    MaxResults = MaxResults,
                    PaginationToken = PaginationToken,
                    UserFields = UserFields,
                    UserID = UserID,
                };
            else
                return list with
                {
                    Type = Type,
                    Expansions = Expansions,
                    ListFields = ListFields,
                    ListID = ListID,
                    MaxResults = MaxResults,
                    PaginationToken = PaginationToken,
                    UserFields = UserFields,
                    UserID = UserID
                };
        }
    }
}
