using System;
using System.Diagnostics.CodeAnalysis;

namespace LinqToTwitter.Provider
{
    // Declare that this request processor knows how to handle action
    // responses, implies the request processor also wants native JSON objects.
    public interface IRequestProcessorWithAction<T>
        : IRequestProcessorWantsJson
    {
        [return: MaybeNull]
        T ProcessActionResult(string twitterResponse, Enum theAction);
    }
}
