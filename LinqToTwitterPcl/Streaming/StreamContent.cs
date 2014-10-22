using LitJson;
using System;

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

            if (jsonObj.InstObject.ContainsKey("created_at") != null)
            {
                EntityType = StreamEntityType.Status;
                Entity = new Status(jsonObj);
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
