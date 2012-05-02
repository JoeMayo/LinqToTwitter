using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class LegalRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        public string BaseUrl { get; set; }

        /// <summary>
        /// Stream method
        /// </summary>
        public LegalType Type { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var parameters =
               new ParameterFinder<Legal>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type"
                   }).Parameters;

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

            Type = RequestProcessorHelper.ParseQueryEnumType<LegalType>(parameters["Type"]);

            switch (Type)
            {
                case LegalType.Privacy:
                    return BuildPrivacyUrl();
                case LegalType.Tos:
                    return BuildTosUrl();
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// builds an url for obtaining Twitter privacy policy
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildPrivacyUrl()
        {
            var req = new Request(BaseUrl + "privacy.json");
            return req;
        }

        /// <summary>
        /// builds an url for obtaining Twitter Terms of Service
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildTosUrl()
        {
            var req = new Request(BaseUrl + "tos.json");
            return req;
        }

        /// <summary>
        /// Handles legal response from Twitter
        /// </summary>
        /// <param name="notUsed">Response from Twitter</param>
        /// <returns>List with a single Legal</returns>
        public List<T> ProcessResults(string responseJson)
        {
            JsonData legalJson = JsonMapper.ToObject(responseJson);

            Legal legal = new Legal
            {
                Type = Type
            };

            switch (Type)
            {
                case LegalType.Privacy:
                    legal.Text = legalJson.GetValue<string>("privacy");
                    break;
                case LegalType.Tos:
                    legal.Text = legalJson.GetValue<string>("tos");
                    break;
                default:
                    legal = new Legal();
                    break;
            }

            var legalList = new List<Legal> { legal };

            return legalList.OfType<T>().ToList();
        }
    }
}
