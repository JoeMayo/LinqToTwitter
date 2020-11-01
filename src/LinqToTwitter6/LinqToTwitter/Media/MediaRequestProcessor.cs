using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Status requests
    /// </summary>
    public class MediaRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base URL for uploading media
        /// </summary>
        public string? UploadUrl { get; set; }

        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// type of media request, i.e. Status
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// Media command sent to Twitter. e.g. STATUS for requesting media upload status.
        /// </summary>
        public string? Command { get; set; }

        /// <summary>
        /// ID of uploaded media to work with.
        /// </summary>
        public ulong MediaID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Media>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(Command),
                       nameof(MediaID)
                   });

            Dictionary<string, string> parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">Criteria for url segments and parameters.</param>
        /// <returns>URL conforming to Twitter API.</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(nameof(Type)))
                throw new ArgumentException("You must set Type.", nameof(Type));

            Type = RequestProcessorHelper.ParseEnum<MediaType>(parameters[nameof(Type)]);

            switch (Type)
            {
                case MediaType.Status:
                    return BuildStatusUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// appends parameters that are common to both friend and user queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        Request BuildUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            //var urlParams = req.RequestParameters;

            //if (parameters.ContainsKey("ID"))
            //{
            //    ID = ulong.Parse(parameters["ID"]);
            //    urlParams.Add(new QueryParameter("id", parameters["ID"]));
            //}

            //if (parameters.ContainsKey("UserID"))
            //{
            //    UserID = ulong.Parse(parameters["UserID"]);
            //    urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            //}

            //if (parameters.ContainsKey("ScreenName"))
            //{
            //    ScreenName = parameters["ScreenName"];
            //    urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            //}

            //if (parameters.ContainsKey("SinceID"))
            //{
            //    SinceID = ulong.Parse(parameters["SinceID"]);
            //    urlParams.Add(new QueryParameter("since_id", parameters["SinceID"]));
            //}

            //if (parameters.ContainsKey("MaxID"))
            //{
            //    MaxID = ulong.Parse(parameters["MaxID"]);
            //    urlParams.Add(new QueryParameter("max_id", parameters["MaxID"]));
            //}

            //if (parameters.ContainsKey("Count"))
            //{
            //    Count = int.Parse(parameters["Count"]);
            //    urlParams.Add(new QueryParameter("count", parameters["Count"]));
            //}

            //if (parameters.ContainsKey("IncludeRetweets"))
            //{
            //    IncludeRetweets = bool.Parse(parameters["IncludeRetweets"]);
            //    urlParams.Add(new QueryParameter("include_rts", parameters["IncludeRetweets"].ToLower()));
            //}

            //if (parameters.ContainsKey("ExcludeReplies"))
            //{
            //    ExcludeReplies = bool.Parse(parameters["ExcludeReplies"]);
            //    urlParams.Add(new QueryParameter("exclude_replies", parameters["ExcludeReplies"].ToLower()));
            //}

            //if (parameters.ContainsKey("IncludeMyRetweet"))
            //{
            //    IncludeMyRetweet = bool.Parse(parameters["IncludeMyRetweet"]);
            //    urlParams.Add(new QueryParameter("include_my_retweet", parameters["IncludeMyRetweet"].ToLower()));
            //}

            //if (parameters.ContainsKey("IncludeEntities"))
            //{
            //    IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
            //    urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            //}

            //if (parameters.ContainsKey("IncludeUserEntities"))
            //{
            //    IncludeUserEntities = bool.Parse(parameters["IncludeUserEntities"]);
            //    urlParams.Add(new QueryParameter("include_user_entities", parameters["IncludeUserEntities"].ToLower()));
            //}

            //if (parameters.ContainsKey("TrimUser"))
            //{
            //    TrimUser = bool.Parse(parameters["TrimUser"]);
            //    urlParams.Add(new QueryParameter("trim_user", parameters["TrimUser"].ToLower()));
            //}

            //if (parameters.ContainsKey("IncludeContributorDetails"))
            //{
            //    IncludeContributorDetails = bool.Parse(parameters["IncludeContributorDetails"]);
            //    urlParams.Add(new QueryParameter("contributor_details", parameters["IncludeContributorDetails"].ToLower()));
            //}

            return req;
        }

        Request BuildStatusUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(nameof(MediaID)))
                throw new ArgumentNullException(nameof(MediaID), $"{nameof(MediaID)} is required");

            var req = new Request(UploadUrl + "media/upload.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            string command = 
                !parameters.ContainsKey(nameof(Command)) || string.IsNullOrWhiteSpace(parameters[nameof(Command)]) ? 
                    Media.StatusCommand : parameters[nameof(Command)];
            urlParams.Add(new QueryParameter("command", command));
            Command = command;

            MediaID = ulong.Parse(parameters[nameof(MediaID)]);
            urlParams.Add(new QueryParameter("media_id", parameters[nameof(MediaID)]));

            return req;
        }

        /// <summary>
        /// transforms Twitter response into List of Status
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of Status</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            JsonElement mediaJson = JsonDocument.Parse(responseJson).RootElement;

            var statusList = new List<Media>();
            switch (Type)
            {
                case MediaType.Status:
                default:
                    statusList.Add(new Media(mediaJson));
                    break;
            }

            foreach (var media in statusList)
            {
                media.Type = Type;
                media.MediaID = MediaID;
                media.Command = Command;
            };

            return statusList.OfType<T>().ToList();
        }

        public T? ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement statusJson = JsonDocument.Parse(responseJson).RootElement;

            Status status =
                ((StatusAction)theAction) switch
                {
                    StatusAction.SingleStatus =>
                        new Status(statusJson),
                    StatusAction.MediaUpload =>
                        new Status
                        {
                            Media = new Media(statusJson)
                        },
                    _ => new Status()
                };

            return status.ItemCast(default(T));
        }
    }
}
