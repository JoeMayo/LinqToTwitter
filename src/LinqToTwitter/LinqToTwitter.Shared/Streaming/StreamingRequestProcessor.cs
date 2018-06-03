using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    public class StreamingRequestProcessor<T> : IRequestProcessor<T>
    {
        public string BaseUrl { get; set; }

        public string UserStreamUrl { get; set; }

        public string SiteStreamUrl { get; set; }

        public ITwitterExecute TwitterExecutor { get; set; }

        /// <summary>
        /// Stream method
        /// </summary>
        public StreamingType Type { get; set; }

        /// <summary>
        /// Normally, only replies between two users that follow each other show.
        /// Setting this to true will show replies, regardless of follow status.
        /// </summary>
        internal bool AllReplies { get; set; }

        /// <summary>
        /// Number of tweets to go back to when reconnecting
        /// </summary>
        internal int Count { get; set; }

        /// <summary>
        /// Tweets are delimeted in the stream
        /// </summary>
        internal string Delimited { get; set; }

        /// <summary>
        /// Limit results to a comma-separated set of users
        /// </summary>
        internal string Follow { get; set; }

        /// <summary>
        /// Comma-separated list of languages to filter results on
        /// </summary>
        internal string Language { get; set; }

        /// <summary>
        /// Get tweets in the comma-separated list of lat/lon's
        /// </summary>
        internal string Locations { get; set; }

        /// <summary>
        /// Tell Twitter to send stall warnings
        /// </summary>
        internal bool StallWarnings { get; set; }

        /// <summary>
        /// Comma-separated list of keywords to get tweets for
        /// </summary>
        internal string Track { get; set; }

        /// <summary>
        /// Type of entities to return, i.e. "followings" or "user".
        /// </summary>
        internal string With { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var parameters =
               new ParameterFinder<Streaming>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "AllReplies",
                       "Count",
                       "Delimited",
                       "Follow",
                       "Language",
                       "Locations",
                       "StallWarnings",
                       "Track",
                       "With"
                   }).Parameters;

            if (parameters.ContainsKey("AllReplies")) 
                AllReplies = bool.Parse(parameters["AllReplies"]);

            if (parameters.ContainsKey("Count"))
                Count = int.Parse(parameters["Count"]);

            if (parameters.ContainsKey("Delimited"))
                Delimited = parameters["Delimited"];

            if (parameters.ContainsKey("Follow"))
                Follow = parameters["Follow"];

            if (parameters.ContainsKey("Language"))
                Language = parameters["Language"];

            if (parameters.ContainsKey("Locations"))
                Locations = parameters["Locations"];

            if (parameters.ContainsKey("StallWarnings"))
                StallWarnings = bool.Parse(parameters["StallWarnings"]);

            if (parameters.ContainsKey("Track"))
                Track = parameters["Track"];

            if (parameters.ContainsKey("With"))
                With = parameters["With"];

            return parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);
 
            Type = RequestProcessorHelper.ParseQueryEnumType<StreamingType>(parameters["Type"]);

            switch (Type)
            {
                case StreamingType.Filter:
                    return BuildFilterUrl(parameters);
                case StreamingType.Firehose:
                    return BuildFirehoseUrl(parameters);
                case StreamingType.Sample:
                    return BuildSampleUrl(parameters);
                case StreamingType.Site:
                    return BuildSiteUrl(parameters);
                case StreamingType.User:
                    return BuildUserUrl(parameters);
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Builds an url for filtering stream.
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildFilterUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Follow") &&
                !parameters.ContainsKey("Locations") &&
                !parameters.ContainsKey("Track"))
            {
                throw new ArgumentException("You must specify at least one of the parameters Follow, Locations, or Track.", "FollowOrLocationsOrTrack");
            }

            var req = new Request(BaseUrl + "statuses/filter.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Count"))
            {
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("Delimited"))
            {
                urlParams.Add(new QueryParameter("delimited", parameters["Delimited"]));
            }

            if (parameters.ContainsKey("Follow"))
            {
                urlParams.Add(new QueryParameter("follow", parameters["Follow"]));
            }

            if (parameters.ContainsKey("Locations"))
            {
                urlParams.Add(new QueryParameter("locations", parameters["Locations"]));
            }

            if (parameters.ContainsKey("Track"))
            {
                urlParams.Add(new QueryParameter("track", parameters["Track"]));
            }

            if (parameters.ContainsKey("StallWarnings"))
            {
                urlParams.Add(new QueryParameter("stall_warnings", parameters["StallWarnings"].ToLower()));
            }

            if (parameters.ContainsKey("Language"))
            {
                urlParams.Add(new QueryParameter("language", parameters["Language"].Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Builds an url for getting all results from the Twitter stream.
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildFirehoseUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "statuses/firehose.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Count"))
            {
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("Delimited"))
            {
                urlParams.Add(new QueryParameter("delimited", parameters["Delimited"]));
            }

            if (parameters.ContainsKey("Language"))
            {
                urlParams.Add(new QueryParameter("language", parameters["Language"].Replace(" ", "")));
            }

            if (parameters.ContainsKey("StallWarnings"))
            {
                urlParams.Add(new QueryParameter("stall_warnings", parameters["StallWarnings"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Builds an url for getting random sample tweets from the stream.
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildSampleUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("Count"))
                throw new ArgumentException("Count is forbidden in Sample streams.", "Count");

            var req = new Request(BaseUrl + "statuses/sample.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Delimited"))
            {
                urlParams.Add(new QueryParameter("delimited", parameters["Delimited"]));
            }

            if (parameters.ContainsKey("Language"))
            {
                urlParams.Add(new QueryParameter("language", parameters["Language"].Replace(" ", "")));
            }

            if (parameters.ContainsKey("StallWarnings"))
            {
                urlParams.Add(new QueryParameter("stall_warnings", parameters["StallWarnings"].ToLower()));
            }

            return req;
        }
        
        /// <summary>
        /// Builds an url for getting info for multiple users from stream.
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildSiteUrl(Dictionary<string, string> parameters)
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

            if (parameters.ContainsKey("Language"))
            {
                Language = parameters["Language"].Replace(" ", "");
                urlParams.Add(new QueryParameter("language", Language));
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

            if (parameters.ContainsKey("AllReplies"))
            {
                AllReplies = bool.Parse(parameters["AllReplies"]);

                if (AllReplies)
                {
                    urlParams.Add(new QueryParameter("replies", "all"));
                }
            }

            if (parameters.ContainsKey("StallWarnings"))
            {
                StallWarnings = bool.Parse(parameters["StallWarnings"]);
                urlParams.Add(new QueryParameter("stall_warnings", parameters["StallWarnings"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Builds an url for getting user info from stream.
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildUserUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(UserStreamUrl + "user.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Delimited"))
            {
                Delimited = parameters["Delimited"];
                urlParams.Add(new QueryParameter("delimited", Delimited.ToLower()));
            }

            if (parameters.ContainsKey("Language"))
            {
                Language = parameters["Language"].Replace(" ", "");
                urlParams.Add(new QueryParameter("language", Language));
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

            if (parameters.ContainsKey("StallWarnings"))
            {
                StallWarnings = bool.Parse(parameters["StallWarnings"]);
                urlParams.Add(new QueryParameter("stall_warnings", parameters["StallWarnings"].ToLower()));
            }

            if (parameters.ContainsKey("Locations"))
            {
                Locations = parameters["Locations"];
                urlParams.Add(new QueryParameter("locations", Locations));
            }

            return req;
        }

        /// <summary>
        /// Returns an object for interacting with stream
        /// </summary>
        /// <param name="notUsed">Not used</param>
        /// <returns>List with a single Streaming</returns>
        public List<T> ProcessResults(string notUsed)
        {
            var streamingList = new List<Streaming>
            {
                new Streaming
                {
                    AllReplies = AllReplies,
                    Type = Type,
                    Count = Count,
                    Delimited = Delimited,
                    Follow = Follow,
                    Locations = Locations,
                    Track = Track,
                    TwitterExecutor = TwitterExecutor,
                    With = With
                }
            };

            return streamingList.OfType<T>().ToList();
        }
    }
}
