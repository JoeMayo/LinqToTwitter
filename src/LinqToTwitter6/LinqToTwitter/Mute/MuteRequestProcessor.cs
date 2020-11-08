using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Twitter User requests.
    /// </summary>
    public class MuteRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        public MuteType Type { get; set; }

        /// <summary>
        /// Indicator for which page to get next
        /// </summary>
        /// <remarks>
        /// This is not a page number, but is an indicator to
        /// Twitter on which page you need back. Your choices
        /// are Previous and Next, which you can find in the
        /// CursorResponse property when your response comes back.
        /// </remarks>
        public long Cursor { get; set; }

        /// <summary>
        /// Add entities to results
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Remove status from results
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Mute>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Cursor",
                       "IncludeEntities",
                       "SkipStatus"
                   });

            return paramFinder.Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<MuteType>(parameters["Type"]);

            switch (Type)
            {
                case MuteType.IDs:
                    return BuildIDsUrl(parameters);
                case MuteType.List:
                    return BuildListUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        Request BuildIDsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "mutes/users/ids.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        Request BuildListUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "mutes/users/list.json");
            var urlParams = req.RequestParameters;

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
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", parameters["SkipStatus"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Transforms Twitter response into List of User
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            List<Mute> muteList = null;

            JsonElement mutesJson = JsonDocument.Parse(responseJson).RootElement;

            switch (Type)
            {
                case MuteType.IDs:
                    muteList = HandleIdsResponse(mutesJson);
                    break;
                case MuteType.List:
                    muteList = HandleMultipleUserResponse(mutesJson);
                    break;
                default:
                    muteList = new List<Mute>();
                    break;
            }

            foreach(var mute in muteList)
            {
                mute.Type = Type;
                mute.Cursor = Cursor;
                mute.IncludeEntities = IncludeEntities;
                mute.SkipStatus = SkipStatus;
            }

            return muteList.OfType<T>().ToList();
        }

        List<Mute> HandleIdsResponse(JsonElement idsJson)
        {
            var ids = idsJson.GetProperty("ids");
            var muteList = new List<Mute>
            {
                new Mute
                {
                    IDList = 
                        (from id in ids.EnumerateArray()
                         select id.GetUInt64())
                        .ToList(),
                    Users = new List<User>(),
                    CursorMovement = new Cursors(idsJson)
                }
            };
            return muteList;
        }
  
        List<Mute> HandleMultipleUserResponse(JsonElement userJson)
        {
            var users = userJson.GetProperty("users");
            List<Mute> muteList = new List<Mute>
            {
                new Mute
                {
                    IDList = new List<ulong>(),
                    Users =
                        (from user in users.EnumerateArray()
                         select new User(user))
                        .ToList(),
                    CursorMovement = new Cursors(userJson)
                }
            };

            return muteList;
        }

        List<User> HandleSingleUserResponse(JsonElement userJson)
        {
            List<User> userList = new List<User> { new User(userJson) };
            return userList;
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement userJson = JsonDocument.Parse(responseJson).RootElement;

            List<User> user = HandleSingleUserResponse(userJson);

            return user.Single().ItemCast(default(T));
        }
    }
}
