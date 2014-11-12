using System;
using System.Xml.Serialization;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
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
            ParseJson(content);
        }

        void ParseJson(string json)
        {
            JsonData jsonObj = JsonMapper.ToObject(json);
            if (jsonObj == null || jsonObj.InstObject == null)
            {
                EntityType = StreamEntityType.Unknown;
                return;
            }
            var inst = jsonObj.InstObject;

            try
            {
                if (inst.ContainsKey("control"))
                {
                    EntityType = StreamEntityType.Control;
                    Entity = new Control(jsonObj);
                }
                else if (inst.ContainsKey("delete"))
                {
                    EntityType = StreamEntityType.Delete;
                    Entity = new Delete(jsonObj);
                }
                else if (inst.ContainsKey("direct_message"))
                {
                    EntityType = StreamEntityType.DirectMessage;
                    var dmObj = jsonObj.GetValue<JsonData>("direct_message");
                    Entity = new DirectMessage(dmObj);
                }
                else if (inst.ContainsKey("disconnect"))
                {
                    EntityType = StreamEntityType.Disconnect;
                    Entity = new Disconnect(jsonObj);
                }
                else if (inst.ContainsKey("event"))
                {
                    EntityType = StreamEntityType.Event;
                    Entity = new Event(jsonObj);
                }
                else if (inst.ContainsKey("for_user"))
                {
                    EntityType = StreamEntityType.ForUser;
                    Entity = new ForUser(jsonObj);
                }
                else if (inst.ContainsKey("friends") && inst.Count == 1)
                {
                    EntityType = StreamEntityType.FriendsList;
                    Entity = new FriendsList(jsonObj);
                }
                else if (inst.ContainsKey("geo_scrub"))
                {
                    EntityType = StreamEntityType.GeoScrub;
                    Entity = new GeoScrub(jsonObj);
                }
                else if (inst.ContainsKey("limit"))
                {
                    EntityType = StreamEntityType.Limit;
                    Entity = new Limit(jsonObj);
                }
                else if (inst.ContainsKey("warning") && inst.ContainsKey("percent_full"))
                {
                    EntityType = StreamEntityType.Stall;
                    Entity = new Stall(jsonObj);
                }
                else if (inst.ContainsKey("status_withheld"))
                {
                    EntityType = StreamEntityType.StatusWithheld;
                    Entity = new StatusWithheld(jsonObj);
                }
                else if (inst.ContainsKey("warning") && inst.ContainsKey("user_id"))
                {
                    EntityType = StreamEntityType.TooManyFollows;
                    Entity = new TooManyFollows(jsonObj);
                }
                else if (inst.ContainsKey("retweeted"))
                {
                    EntityType = StreamEntityType.Status;
                    Entity = new Status(jsonObj);
                }
                else if (inst.ContainsKey("user_withheld"))
                {
                    EntityType = StreamEntityType.UserWithheld;
                    Entity = new UserWithheld(jsonObj);
                }
                else
                {
                    EntityType = StreamEntityType.Unknown;
                }
            }
            catch (Exception ex)
            {
                string parseError = string.Format("Error parsing twitter message. Please create a new issue on the LINQ to Twitter site at https://linqtotwitter.codeplex.com/ with this info. \n\nMessage Type: {0}, Message Text:\n {1} \n", EntityType, json);
                
                EntityType = StreamEntityType.ParseError;
                Entity = parseError;

                if (TwitterExecute.Log != null)
                    TwitterExecute.Log.WriteLine(parseError);
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
