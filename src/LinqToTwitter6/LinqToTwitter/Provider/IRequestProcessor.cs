using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToTwitter.Provider
{
    public interface IRequestProcessor<T>
    {
        string? BaseUrl { get; set; }
        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);
        Request BuildUrl(Dictionary<string, string> expressionParameters);
        List<T> ProcessResults(string twitterResponse);
    }
}
