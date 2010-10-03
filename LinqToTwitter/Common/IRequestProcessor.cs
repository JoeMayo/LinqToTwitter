using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace LinqToTwitter
{
    public interface IRequestProcessor<T>
    {
        string BaseUrl { get; set; }
        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);
        string BuildURL(Dictionary<string, string> parameters);
        List<T> ProcessResults(string twitterResponse);
    }
}
