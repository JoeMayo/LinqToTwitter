using LitJson;
using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    /// <summary>
    /// Response from Twitter stream
    /// </summary>
    public class StreamContent : IStreamContent
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
            else if (inst.ContainsKey("sender"))
            {
                EntityType = StreamEntityType.DirectMessage;
                Entity = new DirectMessage(jsonObj);
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
    }
}
