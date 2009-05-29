using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LinqToTwitter
{
    /// <summary>
    /// used for processing help messages - we only use the request processing part
    /// </summary>
    public class HelpRequestProcessor : IRequestProcessor
    {
        #region IRequestProcessor Members

        /// <summary>
        /// not used
        /// </summary>
        public string BaseUrl
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// not used
        /// </summary>
        public Dictionary<string, string> GetParameters(System.Linq.Expressions.LambdaExpression lambdaExpression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// not used
        /// </summary>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// return response from help request
        /// </summary>
        /// <param name="twitterResponse">response from twitter</param>
        /// <returns>true</returns>
        public IList ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            var response = twitterResponse.Value;

            var helpList = new List<bool> { bool.Parse(response) };
            return helpList;
        }

        #endregion
    }
}
