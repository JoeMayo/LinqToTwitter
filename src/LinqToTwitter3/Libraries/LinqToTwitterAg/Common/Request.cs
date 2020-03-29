using System.Collections.Generic;

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
}