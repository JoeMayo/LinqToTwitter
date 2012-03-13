using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    public class UserStreamRequestProcessor<T> : IRequestProcessor<T>
    {
        public string BaseUrl
        {
            get
            {
                throw new NotImplementedException("You should use UserStreamUrl or SiteStreamUrl instead.");
            }
            set
            {
                throw new NotImplementedException("You should use UserStreamUrl or SiteStreamUrl instead.");
            }
        }

        public string UserStreamUrl { get; set; }

        public string SiteStreamUrl { get; set; }

        public ITwitterExecute TwitterExecutor { get; set; }

        /// <summary>
        /// Type of user stream
        /// </summary>
        public UserStreamType Type { get; set; }

        /// <summary>
        /// Stream delimiter
        /// </summary>
        /// <remarks>Should always be "length" </remarks>
        public string Delimited { get; set; }

        /// <summary>
        /// Comma-separated list (no spaces) of users to add to Site Stream
        /// </summary>
        public string Follow { get; set; }

        /// <summary>
        /// Search terms
        /// </summary>
        public string Track { get; set; }

        /// <summary>
        /// Type of entities to return, i.e. "followings" or "user".
        /// </summary>
        public string With { get; set; }

        /// <summary>
        /// Normally, only replies between two users that follow each other show.
        /// Setting this to true will show replies, regardless of follow status.
        /// </summary>
        public bool AllReplies { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var parameters =
               new ParameterFinder<UserStream>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Delimited",
                       "Follow",
                       "Track",
                       "With",
                       "AllReplies"
                   }).Parameters;

            if (parameters.ContainsKey("Delimited"))
            {
                Delimited = parameters["Delimited"];
            }

            if (parameters.ContainsKey("Follow"))
            {
                Follow = parameters["Follow"];
            }

            if (parameters.ContainsKey("Track"))
            {
                Track = parameters["Track"];
            }

            if (parameters.ContainsKey("With"))
            {
                With = parameters["With"];
            }

            if (parameters.ContainsKey("AllReplies"))
            {
                AllReplies = bool.Parse(parameters["AllReplies"]);
            }

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildURL(Dictionary<string, string> parameters)
        {
            const string typeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", typeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<UserStreamType>(parameters["Type"]);

            switch (Type)
            {
                case UserStreamType.User:
                    return BuildUserUrl(parameters);
                case UserStreamType.Site:
                    return BuildSiteUrl(parameters);
                default:
                    throw new ArgumentException("Invalid UserStreamType", "UserStreamType");
            }
        }

        /// <summary>
        /// builds an url for getting user info from stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildUserUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(UserStreamUrl + "user.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Delimited"))
            {
                Delimited = parameters["Delimited"];
                urlParams.Add(new QueryParameter("delimited", Delimited.ToLower()));
            }

            if (parameters.ContainsKey("Track"))
            {
                Track = parameters["Track"];
                urlParams.Add(new QueryParameter("track", Track));
            }

            if (parameters.ContainsKey("With"))
            {
                With = parameters["With"];
                urlParams.Add(new QueryParameter("with", With.ToLower()));
            }

            if (parameters.ContainsKey("AllReplies"))
            {
                AllReplies = bool.Parse(parameters["AllReplies"]);

                if (AllReplies)
                {
                    urlParams.Add(new QueryParameter("replies", "all"));
                }
            }

            return req;
        }

        /// <summary>
        /// builds an url for getting user info from stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildSiteUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Follow"))
            {
                throw new ArgumentNullException("Follow", "Follow is required.");
            }

            var req = new Request(SiteStreamUrl + "site.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Delimited"))
            {
                Delimited = parameters["Delimited"];
                urlParams.Add(new QueryParameter("delimited", Delimited.ToLower()));
            }

            if (parameters.ContainsKey("Follow"))
            {
                Follow = parameters["Follow"].Replace(" ", "");
                urlParams.Add(new QueryParameter("follow", Follow.ToLower()));
            }

            if (parameters.ContainsKey("Track"))
            {
                throw new ArgumentException("Track is not supported for Site Streams.", "Track");
            }

            if (parameters.ContainsKey("With"))
            {
                With = parameters["With"];
                urlParams.Add(new QueryParameter("with", With.ToLower()));
            }

            //if (parameters.ContainsKey("AllReplies"))
            //{
            //    AllReplies = bool.Parse(parameters["AllReplies"]);

            //    if (AllReplies)
            //    {
            //        urlParams.Add(new QueryParameter("replies", "all"));
            //    }
            //}

            return req;
        }

        /// <summary>
        /// Returns an object for interacting with stream
        /// </summary>
        /// <param name="notUsed">Not used</param>
        /// <returns>List with a single UserStream</returns>
        public List<T> ProcessResults(string notUsed)
        {
            var streamingList = new List<UserStream>
            {
                new UserStream
                {
                    Type = Type,
                    Delimited = Delimited,
                    Follow = Follow,
                    Track = Track,
                    With = With,
                    AllReplies = AllReplies,
                    TwitterExecutor = TwitterExecutor
                }
            };

            return streamingList.OfType<T>().ToList();
        }
    }
}
