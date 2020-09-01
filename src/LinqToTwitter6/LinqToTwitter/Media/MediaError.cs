using System.Text.Json;

namespace LinqToTwitter
{
    public class MediaError
    {
        public MediaError() { }
        public MediaError(JsonElement error)
        {
            Code = error.GetProperty("code").GetInt32();
            Name = error.GetProperty("name").GetString();
            Message = error.GetProperty("message").GetString();
        }

        /// <summary>
        /// Code number from Twitter
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Name of the error
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of why the error occurred
        /// </summary>
        public string Message { get; set; }
    }
}
