using LinqToTwitter.Provider;
using System;
using System.Text.Json;
using System.Xml.Serialization;

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
            (Entity, EntityType) = ParseJson(content);
        }

        (Tweet? entity, StreamEntityType entityType) ParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return (null, StreamEntityType.Unknown);

            try
            {
                return (JsonSerializer.Deserialize<Tweet>(json), StreamEntityType.Tweet);
            }
            catch (Exception ex)
            {
                string parseError = 
                    $"Error parsing twitter message. Please create a new issue on the LINQ to Twitter " +
                    $"site at https://github.com/JoeMayo/LinqToTwitter/issues with this info. \n\n" +
                    $"Message Type: {EntityType}, Message Text:\n {json} \nException Details: {ex} \n";

                ErrorMessage = parseError;

                if (TwitterExecute.Log != null)
                    TwitterExecute.Log.WriteLine(parseError);

                return (null, StreamEntityType.Unknown);
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
        public Tweet? Entity { get; private set; }

        /// <summary>
        /// If HasError is true, check the ErrorMessage for more info
        /// </summary>
        public bool HasError { get => ErrorMessage != null; }

        /// <summary>
        /// Will have an error message if a problem occurred.
        /// </summary>
        public string? ErrorMessage { get; set; }

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
