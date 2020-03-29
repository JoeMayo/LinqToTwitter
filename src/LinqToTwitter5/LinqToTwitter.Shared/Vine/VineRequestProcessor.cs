using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Twitter Vine requests.
    /// </summary>
    public class VineRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// base url for vine requests
        /// </summary>
        public virtual string VineUrl { get; set; }

        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        internal VineType Type { get; set; }

        /// <summary>
        /// ID of vine to query
        /// </summary>
        internal string ID { get; set; }

        /// <summary>
        /// Url of vine to query.
        /// </summary>
        internal string Url { get; set; }

        /// <summary>
        /// Maximum width of script.
        /// </summary>
        internal int MaxWidth { get; set; }

        /// <summary>
        /// Maximum height of script.
        /// </summary>
        internal int MaxHeight { get; set; }

        /// <summary>
        /// Don't include script.
        /// </summary>
        internal bool OmitScript { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Vine>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ID",
                       "Url",
                       "MaxWidth",
                       "MaxHeight",
                       "OmitScript"
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

            Type = RequestProcessorHelper.ParseEnum<VineType>(parameters["Type"]);

            switch (Type)
            {
                case VineType.Oembed:
                    return BuildIDsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        Request BuildIDsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(VineUrl + "oembed.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                urlParams.Add(new QueryParameter("id", parameters["ID"]));
            }

            if (parameters.ContainsKey("Url"))
            {
                Url = parameters["Url"];
                urlParams.Add(new QueryParameter("url", parameters["Url"]));
            }

            if (parameters.ContainsKey("MaxWidth"))
            {
                MaxWidth = int.Parse(parameters["MaxWidth"]);
                urlParams.Add(new QueryParameter("max_width", parameters["MaxWidth"]));
            }

            if (parameters.ContainsKey("MaxHeight"))
            {
                MaxHeight = int.Parse(parameters["MaxHeight"]);
                urlParams.Add(new QueryParameter("max_height", parameters["MaxHeight"]));
            }

            if (parameters.ContainsKey("OmitScript"))
            {
                OmitScript = bool.Parse(parameters["OmitScript"]);
                urlParams.Add(new QueryParameter("omit_script", parameters["OmitScript"].ToLower()));
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

            JsonData vineJson = JsonMapper.ToObject(responseJson);

            var vineList = new List<Vine>
            {
                new Vine(vineJson)
                {
                    ID = ID,
                    Url = Url,
                    MaxWidth = MaxWidth,
                    MaxHeight = MaxHeight,
                    OmitScript = OmitScript
                }
            };

            return vineList.OfType<T>().ToList();
        }
    }
}
