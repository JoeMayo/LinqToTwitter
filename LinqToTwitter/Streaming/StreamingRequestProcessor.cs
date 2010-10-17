using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public int Count { get; set; }

        /// <summary>
        /// Tweets are delimeted in the stream
        /// </summary>
        public string Delimited { get; set; }

        /// <summary>
        /// Limit results to a comma-separated set of users
        /// </summary>
        public string Follow { get; set; }

        /// <summary>
        /// Comma-separated list of keywords to get tweets for
        /// </summary>
        public string Track { get; set; }

        /// <summary>
        /// Get tweets in the comma-separated list of lat/lon's
        /// </summary>
        public string Locations { get; set; }

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
                       "Locations"
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

            return parameters;
        }

        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<StreamingType>(parameters["Type"]);

            switch (Type)
            {
                case StreamingType.Filter:
                    break;
                case StreamingType.Firehose:
                    break;
                case StreamingType.Links:
                    break;
                case StreamingType.Retweet:
                    break;
                case StreamingType.Sample:
                    url = BuildSampleUrl(parameters);
                    break;
                default:
                    break;
            }

            return url;
        }

        private string BuildSampleUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("Count"))
            {
                throw new ArgumentException("Count is forbidden in Sample streams.", "Count");
            }

            string url = BaseUrl + "statuses/sample.json";

            if (parameters.ContainsKey("Delimited"))
            {
                url += "?delimited=" + parameters["Delimited"];
            }

            return url;
        }

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
