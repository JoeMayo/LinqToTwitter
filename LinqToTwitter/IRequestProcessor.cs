using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinqToTwitter
{
    public interface IRequestProcessor
    {
        string BaseUrl { get; set; }
        string BuildURL(Dictionary<string, string> parameters);
        object ProcessResults(XElement twitterResponse);
    }
}
