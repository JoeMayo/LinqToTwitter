using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Control Streams Queries
    /// </summary>
    public class ControlStreamRequestProcessor<T> :
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
        /// base url of SiteStream
        /// </summary>
        public virtual string SiteStreamUrl { get; set; }

        /// <summary>
        /// Type of Direct Message
        /// </summary>
        internal ControlStreamType Type { get; set; }

        /// <summary>
        /// ID of User
        /// </summary>
        internal ulong UserID { get; set; }

        /// <summary>
        /// ID of site stream to operate on
        /// </summary>
        internal string StreamID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<ControlStream>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "UserID",
                       "StreamID"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
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

            Type = RequestProcessorHelper.ParseEnum<ControlStreamType>(parameters["Type"]);

            switch (Type)
            {
                case ControlStreamType.Followers:
                    return BuildFollowersUrl(parameters);
                case ControlStreamType.Info:
                    return BuildInfoUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        Request BuildFollowersUrl(Dictionary<string, string> parameters)
        {
            const string UserIdParam = "UserID";
            if (parameters == null || !parameters.ContainsKey(UserIdParam))
                throw new ArgumentNullException(UserIdParam, "UserID is required.");

            const string StreamIdParam = "StreamID";
            if (parameters == null || !parameters.ContainsKey(StreamIdParam))
                throw new ArgumentNullException(StreamIdParam, "StreamID is required.");

            StreamID = parameters[StreamIdParam];
            var req = new Request(SiteStreamUrl + "site/c/" + parameters[StreamIdParam] + "/friends/ids.json");
            parameters.Remove(StreamIdParam);

            var urlParams = req.RequestParameters;

            UserID = ulong.Parse(parameters[UserIdParam]);
            urlParams.Add(new QueryParameter("user_id", UserID.ToString()));

            return req;
        }

        Request BuildInfoUrl(Dictionary<string, string> parameters)
        {
            const string StreamIdParam = "StreamID";
            if (parameters == null || !parameters.ContainsKey(StreamIdParam))
                throw new ArgumentNullException(StreamIdParam, "StreamID is required.");

            StreamID = parameters[StreamIdParam];
            var req = new Request(SiteStreamUrl + "site/c/" + parameters[StreamIdParam] + "/info.json");
            parameters.Remove(StreamIdParam);

            return req;
        }

        /// <summary>
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <param name="responseJson">JSON with Twitter response</param>
        /// <returns>List of DirectMessage</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            var csJson = JsonMapper.ToObject(responseJson);

            var ctrlStream = new ControlStream
            {
                Type = Type,
                UserID = UserID,
                StreamID = StreamID
            };

            var csList = new List<ControlStream>
            {
                ctrlStream
            };

            switch (Type)
            {
                case ControlStreamType.Followers:
                    ctrlStream.Follow = new ControlStreamFollow(csJson);
                    break;
                case ControlStreamType.Info:
                    ctrlStream.Info = new ControlStreamInfo(csJson);
                    break;
                default:
                    csList = new List<ControlStream>();
                    break;
            }

            return csList.OfType<T>().ToList();
        }
  
        /// <summary>
        /// Handles command responses
        /// </summary>
        /// <param name="responseJson">Response from Twitter</param>
        /// <param name="theAction">Identifies the type of response to work with.</param>
        /// <returns></returns>
        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            var cs = new ControlStream { CommandResponse = responseJson };

            return cs.ItemCast(default(T));
        }
    }
}
