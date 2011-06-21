using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToTwitter;

namespace LinqToTwitter
{
    public class Request
    {
        public string Endpoint { get; set; }
        public IList<QueryParameter> RequestParameters { get; internal set; }

        public string QueryString
        {
            get
            {
                return Utilities.BuildQueryString(RequestParameters);
            }
        }

        public string FullUrl
        {
            get
            {
                var queryString = this.QueryString;

                if (queryString.Length > 0)
                    return Endpoint + "?" + QueryString;
                else return Endpoint;
            }
        }

        public Request(string endpoint)
        {
            this.Endpoint = endpoint;
            this.RequestParameters = new List<QueryParameter>();
        }
    }

    public interface IRequestProcessor<T>
    {
        string BaseUrl { get; set; }
        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);
        Request BuildURL(Dictionary<string, string> expressionParameters);
        List<T> ProcessResults(string twitterResponse);
    }
}
