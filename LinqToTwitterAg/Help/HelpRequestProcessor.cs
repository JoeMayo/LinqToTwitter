using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// used for processing help messages - we only use the request processing part
    /// </summary>
    public class HelpRequestProcessor<T> : IRequestProcessor<T>
    {
        #region IRequestProcessor Members

        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Type of Help request (Test, Configuration, or Languages)
        /// </summary>
        public HelpType Type { get; set; }

        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return new ParameterFinder<Help>(
               lambdaExpression.Body,
               new List<string> { 
                   "Type"
               })
               .Parameters;
        }

        public Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<HelpType>(parameters["Type"]);

            switch (Type)
            {
                case HelpType.Test:
                    return new Request(BaseUrl + "help/test.xml");
                case HelpType.Configuration:
                    return new Request(BaseUrl + "help/configuration.xml");
                case HelpType.Languages:
                    return new Request(BaseUrl + "help/languages.xml");
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// return response from help request
        /// </summary>
        /// <param name="responseXml">response from twitter</param>
        /// <returns>true</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);

            List<Help> helpList = new List<Help>
            {
                Help.Create(twitterResponse)
            };

            helpList.First().Type = this.Type;

            return helpList.OfType<T>().ToList();
        }

        #endregion
    }
}
