using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    public class StreamingRequestProcessor<T> : IRequestProcessor<T>
    {
        public string BaseUrl { get; set; }

        public ITwitterExecute TwitterExecutor { get; set; }

        /// <summary>
        /// Stream method
        /// </summary>
        public StreamingType Type { get; set; }

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
        /// Comma-separated list of keywords to get tweets for
        /// </summary>
        internal string Track { get; set; }

        /// <summary>
        /// Get tweets in the comma-separated list of lat/lon's
        /// </summary>
        internal string Locations { get; set; }

        /// <summary>
        /// Tell Twitter to send stall warnings
        /// </summary>
        internal bool StallWarnings { get; set; }

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
                       "Count",
                       "Delimited",
                       "Follow",
                       "Track",
                       "Locations",
                       "StallWarnings"
                   }).Parameters;

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
            }

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

            if (parameters.ContainsKey("Locations"))
            {
                Locations = parameters["Locations"];
            }

            if (parameters.ContainsKey("StallWarnings"))
            {
                StallWarnings = bool.Parse(parameters["StallWarnings"]);
            }

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
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
                case StreamingType.Links:
                    return BuildLinksUrl(parameters);
                case StreamingType.Retweet:
                    return BuildRetweetUrl(parameters);
                case StreamingType.Sample:
                    return BuildSampleUrl(parameters);
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// builds an url for filtering stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildFilterUrl(Dictionary<string, string> parameters)
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

            return req;
        }

        /// <summary>
        /// builds an url for getting all results from the Twitter stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildFirehoseUrl(Dictionary<string, string> parameters)
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

            if (parameters.ContainsKey("StallWarnings"))
            {
                urlParams.Add(new QueryParameter("stall_warnings", parameters["StallWarnings"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// builds an url for getting all results from the Twitter stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildLinksUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "statuses/links.json");
            var urlParams = req.RequestParameters;
      
            if (parameters.ContainsKey("Count"))
            {
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("Delimited"))
            {
                urlParams.Add(new QueryParameter("delimited", parameters["Delimited"]));
            }

            return req;
        }

        /// <summary>
        /// builds an url for getting all results from the Twitter stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildRetweetUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "statuses/retweet.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Delimited"))
            {
                urlParams.Add(new QueryParameter("delimited", parameters["Delimited"]));
            }

            return req;
        }

        /// <summary>
        /// builds an url for getting a sample from the stream
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildSampleUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("Count"))
                throw new ArgumentException("Count is forbidden in Sample streams.", "Count");

            var req = new Request(BaseUrl + "statuses/sample.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Delimited"))
            {
                urlParams.Add(new QueryParameter("delimited", parameters["Delimited"]));
            }

            if (parameters.ContainsKey("StallWarnings"))
            {
                urlParams.Add(new QueryParameter("stall_warnings", parameters["StallWarnings"].ToLower()));
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
                    Type = Type,
                    Count = Count,
                    Delimited = Delimited,
                    Follow = Follow,
                    Locations = Locations,
                    Track = Track,
                    TwitterExecutor = TwitterExecutor
                }
            };

            return streamingList.OfType<T>().ToList();
        }
    }
}
