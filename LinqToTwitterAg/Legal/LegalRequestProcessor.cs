using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    public class LegalRequestProcessor<T> : IRequestProcessor<T>
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
        public Request BuildURL(Dictionary<string, string> parameters)
        {
            const string typeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", typeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<LegalType>(parameters["Type"]);

            switch (Type)
            {
                case LegalType.Privacy:
                    return BuildPrivacyUrl();
                case LegalType.TOS:
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
            var req = new Request(BaseUrl + "privacy.xml");
            return req;
        }

        /// <summary>
        /// builds an url for obtaining Twitter Terms of Service
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildTosUrl()
        {
            var req = new Request(BaseUrl + "tos.xml");
            return req;
        }

        /// <summary>
        /// Returns an object for interacting with stream
        /// </summary>
        /// <param name="notUsed">Response from Twitter</param>
        /// <returns>List with a single Legal</returns>
        public List<T> ProcessResults(string responseXml)
        {
            if (string.IsNullOrEmpty(responseXml))
            {
                responseXml = "<legal></legal>";
            }

            var legalList = new List<Legal>
            {
                new Legal
                {
                    Type = Type,
                    Text = XElement.Parse(responseXml).Value
                }
            };

            return legalList.OfType<T>().ToList();
        }
    }
}
