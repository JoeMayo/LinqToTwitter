using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<LegalType>(parameters["Type"]);

            switch (Type)
            {
                case LegalType.Privacy:
                    url = BuildPrivacyUrl(parameters);
                    break;
                case LegalType.TOS:
                    url = BuildTosUrl(parameters);
                    break;
                default:
                    break;
            }

            return url;
        }

        /// <summary>
        /// builds an url for obtaining Twitter privacy policy
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildPrivacyUrl(Dictionary<string, string> parameters)
        {
            return BaseUrl + "privacy.xml";
        }

        /// <summary>
        /// builds an url for obtaining Twitter Terms of Service
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildTosUrl(Dictionary<string, string> parameters)
        {
            return BaseUrl + "tos.xml";
        }
        /// <summary>
        /// Returns an object for interacting with stream
        /// </summary>
        /// <param name="notUsed">Response from Twitter</param>
        /// <returns>List with a single Legal</returns>
        public List<T> ProcessResults(string responseXml)
        {
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
