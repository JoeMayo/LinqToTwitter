using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Twitter User requests.
    /// </summary>
    public class MuteRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// type of mutes request (Muted)
        /// </summary>
        public MuteType Type { get; set; }

        /// <summary>
        /// ID of user to get mutes for
        /// </summary>
        public string? ID { get; set; }


        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Mute>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(ID)
                   });

            return paramFinder.Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<MuteType>(parameters["Type"]);

            switch (Type)
            {
                case MuteType.Muted:
                    return BuildMutedUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        Request BuildMutedUrl(Dictionary<string, string> parameters)
        {
            SetUserID(parameters);

            var req = new Request($"{BaseUrl}users/{ID}/muting");
            var urlParams = req.RequestParameters;

            return req;
        }

        /// <summary>
        /// Sets parameter, but doesn't treat as a query parameter.
        /// </summary>
        /// <param name="parameters">list of parameters</param>
        void SetUserID(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(nameof(ID)))
                ID = parameters[nameof(ID)];
            else
                throw new ArgumentException($"{nameof(ID)} is required", nameof(ID));
        }

        /// <summary>
        /// Transforms Twitter response into List of User
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            List<Mute>? muteList;

            switch (Type)
            {
                case MuteType.Muted:
                    muteList = new List<Mute> { JsonDeserialize(responseJson) };
                    break;
                default:
                    muteList = new List<Mute>();
                    break;
            }

            return muteList.OfType<T>().ToList();
        }

        Mute JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            Mute? mute = JsonSerializer.Deserialize<Mute>(responseJson, options);

            if (mute?.Meta == null || mute.Meta.ResultCount == 0)
                return new Mute
                {
                    Type = Type,
                    ID = ID,
                };
            else
                return mute with
                {
                    Type = Type,
                    ID = ID
                };
        }

        List<User> HandleSingleUserResponse(JsonElement userJson)
        {
            List<User> userList = new List<User> { new User(userJson) };
            return userList;
        }

        public T? ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement userJson = JsonDocument.Parse(responseJson).RootElement;

            List<User> user = HandleSingleUserResponse(userJson);

            return user.Single().ItemCast(default(T));
        }
    }
}
