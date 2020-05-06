using LinqToTwitter.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter.Provider
{
    public class Request
    {
        public string Endpoint { get; set; }
        public IList<QueryParameter> RequestParameters { get; internal set; }

        public Request(string endpoint)
        {
            this.Endpoint = endpoint;
            this.RequestParameters = new List<QueryParameter>();
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

        public string QueryString
        {
            get
            {
                if (RequestParameters == null)
                    throw new ArgumentNullException("parameters");

                StringBuilder builder = new StringBuilder();
                foreach (var pair in RequestParameters.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
                {
                    builder.Append(Url.PercentEncode(pair.Name));
                    builder.Append('=');
                    builder.Append(Url.PercentEncode(pair.Value));
                    builder.Append('&');
                }

                if (builder.Length > 1)
                    builder.Length--;   // truncate trailing &

                return builder.ToString();
            }
        }
    }
}