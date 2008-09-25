using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    public interface IRequestProcessor
    {
        string BaseUrl { get; set; }
        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);
        string BuildURL(Dictionary<string, string> parameters);
        IQueryable ProcessResults(XElement twitterResponse);
    }
}
