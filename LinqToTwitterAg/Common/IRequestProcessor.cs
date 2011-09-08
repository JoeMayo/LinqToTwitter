using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    public interface IRequestProcessor<T>
    {
        string BaseUrl { get; set; }
        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);
        Request BuildURL(Dictionary<string, string> expressionParameters);
        List<T> ProcessResults(string twitterResponse);
    }

    // temporary marker interface used to communicate that this
    // request processor wants native JSON objects.
    public interface IRequestProcessorWantsJson { }
}
