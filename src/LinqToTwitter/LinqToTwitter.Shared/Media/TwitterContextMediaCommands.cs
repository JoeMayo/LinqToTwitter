using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Uploads a media (e.g. media) to be attached to a subsequent tweet.
        /// </summary>
        /// <param name="media">Media to upload</param>
        /// <param name="mediaType">Type of media. e.g. image/jpg, image/png, or video/mp4.</param>
        /// <param name="mediaCategory">
        /// Media category - possible values are tweet_image, tweet_gif, tweet_video, and amplify_video. 
        /// See this post on the Twitter forums: https://twittercommunity.com/t/media-category-values/64781/6
        /// </param>
        /// <param name="cancelToken">Allows you to cancel async operation</param>
        /// <returns>Status containing new reply</returns>
        public virtual async Task<Media> UploadMediaAsync(byte[] media, string mediaType, string mediaCategory, CancellationToken cancelToken = default(CancellationToken))
        {
            return await UploadMediaAsync(media, mediaType, null, mediaCategory, cancelToken);
        }

        /// <summary>
        /// Uploads a media (e.g. media) to be attached to a subsequent tweet.
        /// </summary>
        /// <param name="media">Media to upload</param>
        /// <param name="mediaType">Type of media. e.g. image/jpg, image/png, or video/mp4.</param>
        /// <param name="additionalOwners">User IDs of accounts that can used the returned media IDs</param>
        /// <param name="mediaCategory">
        /// Media category - possible values are tweet_image, tweet_gif, tweet_video, and amplify_video. 
        /// See this post on the Twitter forums: https://twittercommunity.com/t/media-category-values/64781/6
        /// </param>
        /// <param name="cancelToken">Allows you to cancel async operation</param>
        /// <returns>Status containing new reply</returns>
        public virtual async Task<Media> UploadMediaAsync(byte[] media, string mediaType, IEnumerable<ulong> additionalOwners, string mediaCategory, CancellationToken cancelToken = default(CancellationToken))
        {
            if (media == null || media.Length == 0)
                throw new ArgumentNullException("image", "You must provide a byte[] of image data.");

            string updateUrl = UploadUrl + "media/upload.json";
            string name = "media";
            string randomUnusedFileName = new Random().Next(100, 999).ToString();

            var parameters = new Dictionary<string, string>();

            if (additionalOwners != null && additionalOwners.Any())
                parameters.Add("additional_owners", string.Join(",", additionalOwners));

            var reqProc = new StatusRequestProcessor<Status>();

            RawResult =
                await TwitterExecutor.PostMediaAsync(
                    updateUrl,
                    new Dictionary<string, string>(),
                    media,
                    name,
                    randomUnusedFileName,
                    mediaType,
                    mediaCategory,
                    cancelToken)
                   .ConfigureAwait(false);

            Status status = reqProc.ProcessActionResult(RawResult, StatusAction.MediaUpload);
            return status.Media;
        }

        /// <summary>
        /// Adds alt text to media to support accessibility. Upload the media and call this to attach alt text.
        /// </summary>
        /// <param name="mediaID">ID of media.</param>
        /// <param name="altText">Media description.</param>
        /// <param name="cancelToken">Allows you to cancel async operation</param>
        /// <returns>Status containing new reply</returns>
        public virtual async Task CreateMediaMetadataAsync(ulong mediaID, string altText, CancellationToken cancelToken = default(CancellationToken))
        {
            if (mediaID == 0)
                throw new ArgumentNullException(nameof(mediaID), $"Invalid {nameof(mediaID)} parameter: {mediaID}");

            if (string.IsNullOrWhiteSpace(altText))
                throw new ArgumentNullException(nameof(altText), $"The {nameof(altText)} parameter is empty or null and must have a value.");

            string updateUrl = UploadUrl + "media/metadata/create.json";

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    updateUrl,
                    new MediaMetadata { MediaID = mediaID, AltText = new AltText { Text = altText } },
                    cancelToken)
                   .ConfigureAwait(false);
        }
    }
}
