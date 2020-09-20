using LinqToTwitter.Common;
using System.Text.Json;

namespace LinqToTwitter
{
    public class MediaError
    {
        public MediaError() { }
        public MediaError(JsonElement error)
        {
            if (error.IsNull()) return;

            Code = error.GetInt("code");
            Name = error.GetString("name");
            Message = error.GetString("message");
        }

        /// <summary>
        /// Code number from Twitter
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Name of the error
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Description of why the error occurred
        /// </summary>
        public string? Message { get; set; }
    }
}
