using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    public class UserStreamRequestProcessor<T> : IRequestProcessor<T>
    {
        public string BaseUrl { get; set; }

        public ITwitterExecute TwitterExecutor { get; set; }

        /// <summary>
        /// Type of user stream
        /// </summary>
        public UserStreamType Type { get; set; }

        /// <summary>
        /// Stream delimiter
        /// </summary>
        public string Delimited { get; set; }

        /// <summary>
        /// Search terms
        /// </summary>
        public string Track { get; set; }

        /// <summary>
        /// Type of entities to return, i.e. Follow, User, etc.
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
                       "Track",
                       "With",
                       "AllReplies"
                   }).Parameters;

            if (parameters.ContainsKey("Delimited"))
            {
                Delimited = parameters["Delimited"];
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
        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<UserStreamType>(parameters["Type"]);

            switch (Type)
            {
                case UserStreamType.User:
                    url = BuildUserUrl(parameters);
                    break;
                case UserStreamType.Site:
                    break;
                default:
                    throw new ArgumentException("Invalid UserStreamType", "UserStreamType");
            }

            return url;
        }

        /// <summary>
        /// builds an url for getting user info from stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildUserUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl + "user.json";

            var urlParams = new List<string>();

            if (parameters.ContainsKey("Delimited"))
            {
                urlParams.Add("delimited=" + parameters["Delimited"].ToLower());
            }

            if (parameters.ContainsKey("Track"))
            {
                urlParams.Add("track=" + Uri.EscapeUriString(parameters["Track"]));
            }

            if (parameters.ContainsKey("With"))
            {
                urlParams.Add("with=" + parameters["With"].ToLower());
            }

            if (parameters.ContainsKey("AllReplies"))
            {
                if (bool.Parse(parameters["AllReplies"]))
                {
                    urlParams.Add("replies=all"); 
                }
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
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
