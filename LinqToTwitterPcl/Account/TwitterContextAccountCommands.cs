using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        const string NoInputParam = "NoInput";

        /// <summary>
        /// Update Twitter colors
        /// </summary>
        /// <remarks>
        /// The # character prefix is optional.  At least one color argument must be provided.
        /// </remarks>
        /// <param name="background">background color</param>
        /// <param name="text">text color</param>
        /// <param name="link">link color</param>
        /// <param name="sidebarFill">sidebar color</param>
        /// <param name="sidebarBorder">sidebar border color</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User info with new colors</returns>
        public async Task<User> UpdateAccountColorsAsync(string background, string text, string link, string sidebarFill, string sidebarBorder, bool skipStatus)
        {
            return await UpdateAccountColorsAsync(background, text, link, sidebarFill, sidebarBorder, true, skipStatus).ConfigureAwait(false);
        }

        /// <summary>
        /// Update Twitter colors
        /// </summary>
        /// <remarks>
        /// The # character prefix is optional.  At least one color argument must be provided.
        /// </remarks>
        /// <param name="background">background color</param>
        /// <param name="text">text color</param>
        /// <param name="link">link color</param>
        /// <param name="sidebarFill">sidebar color</param>
        /// <param name="sidebarBorder">sidebar border color</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User info with new colors</returns>
        public async Task<User> UpdateAccountColorsAsync(string background, string text, string link, string sidebarFill, string sidebarBorder, bool includeEntities, bool skipStatus, CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/update_profile_colors.json";

            if (string.IsNullOrWhiteSpace(background) &&
                string.IsNullOrWhiteSpace(text) &&
                string.IsNullOrWhiteSpace(link) &&
                string.IsNullOrWhiteSpace(sidebarFill) &&
                string.IsNullOrWhiteSpace(sidebarBorder))
                throw new ArgumentException("At least one of the colors (background, text, link, sidebarFill, or sidebarBorder) must be provided as arguments, but none are specified.", NoInputParam);

            var reqProc = new UserRequestProcessor<User>();

            RawResult =
                await TwitterExecutor.PostToTwitterAsync<User>(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "profile_background_color", string.IsNullOrWhiteSpace(background) ? null : background.TrimStart('#') },
                        { "profile_text_color", string.IsNullOrWhiteSpace(text) ? null : text.TrimStart('#') },
                        { "profile_link_color", string.IsNullOrWhiteSpace(link) ? null : link.TrimStart('#') },
                        { "profile_sidebar_fill_color", string.IsNullOrWhiteSpace(sidebarFill) ? null : sidebarFill.TrimStart('#') },
                        { "profile_sidebar_border_color", string.IsNullOrWhiteSpace(sidebarBorder) ? null : sidebarBorder.TrimStart('#') },
                        { "include_entities", includeEntities.ToString().ToLower() },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, UserAction.SingleUser);
        }

        /// <summary>
        /// sends an image file to Twitter to replace user image
        /// </summary>
        /// <remarks>
        /// You can only run this method with a period of time between executions; 
        /// otherwise you get WebException errors from Twitter
        /// </remarks>
        /// <param name="image">byte array of image to upload</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public async Task<User> UpdateAccountImageAsync(byte[] image, string fileName, string imageType, bool skipStatus, CancellationToken cancelToken = default(CancellationToken))
        {
            return await UpdateAccountImageAsync(image, fileName, imageType, true, skipStatus, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends an image file to Twitter to replace user image.
        /// </summary>
        /// <param name="image">byte array of image to upload</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public async Task<User> UpdateAccountImageAsync(byte[] image, string fileName, string imageType, bool includeEntities, bool skipStatus, CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/update_profile_image.json";

            if (image == null || image.Length == 0)
                throw new ArgumentException("image is required.", "image");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is required.", "fileName");

            if (string.IsNullOrWhiteSpace(imageType))
                throw new ArgumentException("imageType is required.", "imageType");

            var reqProc = new UserRequestProcessor<User>();
            var parameters = new Dictionary<string, string>
                    {
                        { "include_entities", includeEntities.ToString().ToLower() },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    };

            string name = "image";
            string imageMimeType = "image/" + imageType;

            RawResult = await TwitterExecutor.PostMediaAsync(accountUrl, parameters, image, name, fileName, imageMimeType, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, UserAction.SingleUser);
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="image">full path to file, including file name</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="tile">Tile image across background.</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public async Task<User> UpdateAccountBackgroundImageAsync(byte[] image, string fileName, string imageType, bool tile, bool use, bool skipStatus, CancellationToken cancelToken = default(CancellationToken))
        {
            return await UpdateAccountBackgroundImageAsync(image, fileName, imageType, tile, use, true, skipStatus, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends an image file to Twitter to replace background image.
        /// </summary>
        /// <param name="image">full path to file, including file name</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="tile">Tile image across background.</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public async Task<User> UpdateAccountBackgroundImageAsync(byte[] image, string fileName, string imageType, bool tile, bool use, bool includeEntities, bool skipStatus, CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/update_profile_background_image.json";

            if (image == null || image.Length == 0)
                throw new ArgumentException("image is required.", "image");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is required.", "fileName");

            if (string.IsNullOrWhiteSpace(imageType))
                throw new ArgumentException("imageType is required.", "imageType");

            var parameters = new Dictionary<string, string>
            {
                { "include_entities", includeEntities.ToString().ToLower() },
                { "skip_status", skipStatus.ToString().ToLower() }
            };

            if (tile)
                parameters.Add("tile", true.ToString().ToLower());

            parameters.Add("use", use.ToString().ToLower());

            var reqProc = new UserRequestProcessor<User>();

            string name = "image";
            string imageMimeType = "image/" + imageType;

            RawResult = await TwitterExecutor.PostMediaAsync(accountUrl, parameters, image, name, fileName, imageMimeType, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, UserAction.SingleUser);
        }

        /// <summary>
        /// Update account profile info
        /// </summary>
        /// <param name="name">User Name</param>
        /// <param name="url">Web Address</param>
        /// <param name="location">Geographic Location</param>
        /// <param name="description">Personal Description</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new info</returns>
        public async Task<User> UpdateAccountProfileAsync(string name, string url, string location, string description, bool skipStatus, CancellationToken cancelToken = default(CancellationToken))
        {
            return await UpdateAccountProfileAsync(name, url, location, description, true, skipStatus, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Update account profile info
        /// </summary>
        /// <param name="name">User Name</param>
        /// <param name="url">Web Address</param>
        /// <param name="location">Geographic Location</param>
        /// <param name="description">Personal Description</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new info</returns>
        public async Task<User> UpdateAccountProfileAsync(string name, string url, string location, string description, bool includeEntities, bool skipStatus, CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/update_profile.json";

            if (string.IsNullOrWhiteSpace(name) &&
                string.IsNullOrWhiteSpace(url) &&
                string.IsNullOrWhiteSpace(location) &&
                string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("At least one of the text fields (name, email, url, location, or description) must be provided as arguments, but none are specified.", NoInputParam);

            if (!string.IsNullOrWhiteSpace(name) && name.Length > 20)
                throw new ArgumentException("name must be no longer than 20 characters", "name");

            if (!string.IsNullOrWhiteSpace(url) && url.Length > 100)
                throw new ArgumentException("url must be no longer than 100 characters", "url");

            if (!string.IsNullOrWhiteSpace(location) && location.Length > 30)
                throw new ArgumentException("location must be no longer than 30 characters", "location");

            if (!string.IsNullOrWhiteSpace(description) && description.Length > 160)
                throw new ArgumentException("description must be no longer than 160 characters", "description");

            var reqProc = new UserRequestProcessor<User>();

            RawResult =
                await TwitterExecutor.PostToTwitterAsync<User>(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "name", name },
                        { "url", url },
                        { "location", location },
                        { "description", description },
                        { "include_entities", includeEntities.ToString().ToLower() },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, UserAction.SingleUser);
        }

        /// <summary>
        /// Updates user's account settings
        /// </summary>
        /// <param name="trendLocationWeoid">WEOID for Trend Location the user is interested in.</param>
        /// <param name="sleepTimeEnabled">Turn on time periods when notifications won't be sent.</param>
        /// <param name="startSleepTime">Don't send notifications at this time or later this time. (hour from 00 to 23)</param>
        /// <param name="endSleepTime">Start sending notifications again after this time. (hour from 00 to 23)</param>
        /// <param name="timeZone">User's time zone.</param>
        /// <param name="lang">User's language.</param>
        /// <returns>Account information with Settings property populated.</returns>
        public async Task<Account> UpdateAccountSettingsAsync(int? trendLocationWoeid, bool? sleepTimeEnabled, int? startSleepTime, int? endSleepTime, string timeZone, string lang, CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/settings.json";

            if (trendLocationWoeid == null &&
                sleepTimeEnabled == null &&
                startSleepTime == null &&
                endSleepTime == null &&
                string.IsNullOrWhiteSpace(timeZone) &&
                string.IsNullOrWhiteSpace(lang))
                throw new ArgumentException("At least one parameter must be provided as arguments, but none are specified.", NoInputParam);

            var reqProc = new AccountRequestProcessor<Account>();
            var parameters = new Dictionary<string, string>
                    {
                        { "time_zone", timeZone },
                        { "lang", lang }
                    };

            if (trendLocationWoeid != null)
                parameters.Add("trend_location_woeid", trendLocationWoeid.ToString());
            if (sleepTimeEnabled != null)
                parameters.Add("sleep_time_enabled", sleepTimeEnabled.ToString().ToLower());
            if (startSleepTime != null)
                parameters.Add("start_sleep_time", startSleepTime.ToString());
            if (endSleepTime != null)
                parameters.Add("end_sleep_time", endSleepTime.ToString());

            RawResult =
                await TwitterExecutor.PostToTwitterAsync<Account>(
                    accountUrl,
                    parameters,
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, AccountAction.Settings);
        }

        /// <summary>
        /// Modify device information
        /// </summary>
        /// <param name="device">Which device to use.</param>
        /// <param name="includeEntitites">Set this to false to not add entitites to response. (default: true)</param>
        /// <returns></returns>
        public async Task<Account> UpdateDeliveryDeviceAsync(DeviceType device, bool? includeEntitites, CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/update_delivery_device.json";

            var reqProc = new AccountRequestProcessor<Account>();

            var parameters = new Dictionary<string, string>
                    {
                        { "device", device.ToString().ToLower() }
                    };

            if (includeEntitites != null)
                parameters.Add("include_entities", includeEntitites.ToString().ToLower());

            RawResult =
                await TwitterExecutor.PostToTwitterAsync<Account>(
                    accountUrl,
                    parameters,
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, AccountAction.Settings);
        }

        /// <summary>
        /// Sends an image to Twitter to be placed as the user's profile banner.
        /// </summary>
        /// <param name="banner">byte[] containing image data.</param>
        /// <param name="fileName">Name of file.</param>
        /// <param name="imageType">Type of file (e.g. png or jpg)</param>
        /// <returns>
        /// Account of authenticated user who's profile banner will be updated.
        /// Url of new banner will appear in ProfileBannerUrl property.
        /// </returns>
        public async Task<User> UpdateProfileBannerAsync(byte[] banner, string fileName, string imageType, CancellationToken cancelToken = default(CancellationToken))
        {
            return await UpdateProfileBannerAsync(banner, fileName, imageType, 1500, 500, 0, 0, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends an image to Twitter to be placed as the user's profile banner.
        /// </summary>
        /// <param name="banner">byte[] containing image data.</param>
        /// <param name="fileName">Name of file.</param>
        /// <param name="imageType">Type of file (e.g. png or jpg)</param>
        /// <param name="width">Pixel width to clip image.</param>
        /// <param name="height">Pixel height to clip image.</param>
        /// <param name="offsetLeft">Pixels to offset start of image from the left.</param>
        /// <param name="offsetTop">Pixels to offset start of image from the top.</param>
        /// <returns>
        /// Account of authenticated user who's profile banner will be updated.
        /// Url of new banner will appear in ProfileBannerUrl property.
        /// </returns>
        public async Task<User> UpdateProfileBannerAsync(byte[] banner, string fileName, string imageType, int width, int height, int offsetLeft, int offsetTop, CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/update_profile_banner.json";

            if (banner == null || banner.Length == 0)
                throw new ArgumentException("banner is required.", "banner");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is required.", "fileName");

            if (string.IsNullOrWhiteSpace(imageType))
                throw new ArgumentException("imageType is required.", "imageType");

            var parameters = new Dictionary<string, string>
            {
                { "width", width.ToString() },
                { "height", height.ToString() },
                { "offset_left", offsetLeft.ToString() },
                { "offset_top", offsetTop.ToString() },
                //{ "banner", "FILE_DATA" }
            };

            var reqProc = new UserRequestProcessor<User>();

            string name = "banner";
            string imageMimeType = "image/" + imageType;

            RawResult = await TwitterExecutor.PostMediaAsync(accountUrl, parameters, banner, name, fileName, imageMimeType, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, UserAction.SingleUser);
        }

        /// <summary>
        /// Removes banner from authenticated user's profile.
        /// </summary>
        /// <returns>Empty User instance.</returns>
        public async Task<User> RemoveProfileBannerAsync(CancellationToken cancelToken = default(CancellationToken))
        {
            var accountUrl = BaseUrl + "account/remove_profile_banner.json";

            var reqProc = new UserRequestProcessor<User>();

            RawResult =
                await TwitterExecutor.PostToTwitterAsync<User>(
                    accountUrl,
                    new Dictionary<string, string>(),
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, UserAction.SingleUser);
        }
    }
}
