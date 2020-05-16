using LinqToTwitter.Provider;
using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace LinqToTwitter.Streaming
{
    /// <summary>
    /// Response from Twitter stream
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class StreamContent : IStreamContent, IDisposable
    {
        private readonly ITwitterExecute exec;

        public StreamContent(ITwitterExecute exec, string content)
        {
            this.exec = exec;
            Content = content;
            (Entity, EntityType) = ParseJson(content);
        }

        (object entity, StreamEntityType entityType) ParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return (new object(), StreamEntityType.Unknown);

            var jsonDoc = JsonDocument.Parse(json);
            var inst = jsonDoc.RootElement;

            try
            {
                if (inst.TryGetProperty("control", out JsonElement control))
                {
                    return (new Control(control), StreamEntityType.Control);
                }
                else if (inst.TryGetProperty("delete", out JsonElement delete))
                {
                    return (new Delete(delete), StreamEntityType.Delete);
                }
                else if (inst.TryGetProperty("disconnect", out JsonElement disconnect))
                {
                    return (new Disconnect(disconnect), StreamEntityType.Disconnect);
                }
                else if (inst.TryGetProperty("event", out JsonElement evnt))
                {
                    return (new Event(evnt), StreamEntityType.Event);
                }
                else if (inst.TryGetProperty("for_user", out JsonElement for_user))
                {
                    return (new ForUser(for_user), StreamEntityType.ForUser);
                }
                else if (inst.TryGetProperty("friends", out JsonElement friends) && inst.EnumerateArray().Count() == 1)
                {
                    return (new FriendsList(friends), StreamEntityType.FriendsList);
                }
                else if (inst.TryGetProperty("geo_scrub", out JsonElement geo_scrub))
                {
                    return (new GeoScrub(geo_scrub), StreamEntityType.GeoScrub);
                }
                else if (inst.TryGetProperty("limit", out JsonElement limit))
                {
                    return (new Limit(limit), StreamEntityType.Limit);
                }
                else if (inst.TryGetProperty("warning", out JsonElement warning) && inst.TryGetProperty("percent_full", out JsonElement percent_full))
                {
                    return (new Stall(warning), StreamEntityType.Stall);
                }
                else if (inst.TryGetProperty("status_withheld", out JsonElement status_withheld))
                {
                    return (new StatusWithheld(status_withheld), StreamEntityType.StatusWithheld);
                }
                else if (inst.TryGetProperty("warning", out JsonElement warningUserID) && inst.TryGetProperty("user_id", out JsonElement userID))
                {
                    return (new TooManyFollows(warningUserID),  StreamEntityType.TooManyFollows);
                }
                else if (inst.TryGetProperty("retweeted", out JsonElement retweeted))
                {
                    throw new NotImplementedException("Status not implemented yet.");
                    // TODO: Not yet implemented - This should be the correct return type
                    //return (new Status(retweeted), StreamEntityType.Status);
                }
                else if (inst.TryGetProperty("user_withheld", out JsonElement user_withheld))
                {
                    return (new UserWithheld(user_withheld), StreamEntityType.UserWithheld);
                }
                else
                {
                    return (new object(), StreamEntityType.Unknown);
                }
            }
            catch (Exception ex)
            {
                string parseError = 
                    $"Error parsing twitter message. Please create a new issue on the LINQ to Twitter " +
                    $"site at https://github.com/JoeMayo/LinqToTwitter/issues with this info. \n\n" +
                    $"Message Type: {EntityType}, Message Text:\n {json} \nException Details: {ex} \n";

                EntityType = StreamEntityType.ParseError;
                Entity = parseError;

                if (TwitterExecute.Log != null)
                    TwitterExecute.Log.WriteLine(parseError);

                return (new object(), StreamEntityType.Unknown);
            }
        }

        /// <summary>
        /// Stream object, which is a Twitter message of various
        /// formats or empty string for keep-alive message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Type of Stream Message
        /// </summary>
        public StreamEntityType EntityType { get; private set; }

        /// <summary>
        /// LINQ to Twitter entity
        /// </summary>
        public object Entity { get; private set; }

        /// <summary>
        /// Closes Twitter stream.
        /// </summary>
        public virtual void CloseStream()
        {
            exec.CloseStream();
        }

        public void Dispose()
        {
            CloseStream();
        }
    }
}
