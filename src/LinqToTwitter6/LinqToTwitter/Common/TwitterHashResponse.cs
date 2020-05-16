#nullable disable
namespace LinqToTwitter.Common
{
    /// <summary>
    /// Response for HTTP errors and end response
    /// </summary>
    public class TwitterHashResponse
    {
        /// <summary>
        /// URL action from request
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Error { get; set; }
    }
}
