#if ASYNC

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    internal partial class TwitterExecute
    {
        /// <summary>
        /// performs HTTP POST to Twitter
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="postData">parameters to post</param>
        /// <param name="getResult">callback for handling async Json response - null if synchronous</param>
        /// <returns>Json Response from Twitter - empty string if async</returns>
        internal async Task<string> PostToTwitterAsync<T>(string url, IDictionary<string, string> postData, Func<string, T> getResult)
        {
            await Task.Delay(1);

            return null;
        }

        /// <summary>
        /// performs HTTP POST image byte array upload to Twitter
        /// </summary>
        /// <param name="image">byte array containing image to upload</param>
        /// <param name="url">url to upload to</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <returns>XML results From Twitter</returns>
        internal async Task<string> PostTwitterImageAsync<T>(string url, IDictionary<string, string> postData, byte[] image, string fileName,
                                        string imageType, IRequestProcessor<T> reqProc)
        {
            await Task.Delay(1);

            return null;
        }

        /// <summary>
        /// performs HTTP POST media byte array upload to Twitter
        /// </summary>
        /// <param name="url">url to upload to</param>
        /// <param name="postData">request parameters</param>
        /// <param name="mediaItems">list of Media each media item to upload</param>
        /// <param name="reqProc">request processor for handling results</param>
        /// <returns>XML results From Twitter</returns>
        internal async Task<string> PostMediaAsync<T>(string url, IDictionary<string, string> postData, List<Media> mediaItems,
                                 IRequestProcessor<T> reqProc)
        {
            await Task.Delay(1);

            return null;
        }

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>XML Results from Twitter</returns>
        internal async Task<string> QueryTwitterAsync<T>(Request req, IRequestProcessor<T> reqProc)
        {
            await Task.Delay(1);

            return null;
        }

        internal async Task<string> QueryTwitterStreamAsync(Request req)
        {
            await Task.Delay(1);

            return null;
        }
    }
}

#endif