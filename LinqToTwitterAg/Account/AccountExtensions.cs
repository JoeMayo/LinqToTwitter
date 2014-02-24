using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public static class AccountExtensions
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
        public static User UpdateAccountColors(this TwitterContext ctx, string background, string text, string link, string sidebarFill, string sidebarBorder, bool skipStatus)
        {
            return UpdateAccountColors(ctx, background, text, link, sidebarFill, sidebarBorder, true, skipStatus, null);
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
        public static User UpdateAccountColors(this TwitterContext ctx, string background, string text, string link, string sidebarFill, string sidebarBorder, bool includeEntities, bool skipStatus)
        {
            return UpdateAccountColors(ctx, background, text, link, sidebarFill, sidebarBorder, includeEntities, skipStatus, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User info with new colors</returns>
        public static User UpdateAccountColors(this TwitterContext ctx, string background, string text, string link, string sidebarFill, string sidebarBorder, bool includeEntities, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_colors.json";

            if (string.IsNullOrEmpty(background) &&
                string.IsNullOrEmpty(text) &&
                string.IsNullOrEmpty(link) &&
                string.IsNullOrEmpty(sidebarFill) &&
                string.IsNullOrEmpty(sidebarBorder))
            {
                throw new ArgumentException("At least one of the colors (background, text, link, sidebarFill, or sidebarBorder) must be provided as arguments, but none are specified.", NoInputParam);
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "profile_background_color", string.IsNullOrEmpty(background) ? (string)null : background.TrimStart('#') },
                        { "profile_text_color", string.IsNullOrEmpty(text) ? (string)null : text.TrimStart('#') },
                        { "profile_link_color", string.IsNullOrEmpty(link) ? (string)null : link.TrimStart('#') },
                        { "profile_sidebar_fill_color", string.IsNullOrEmpty(sidebarFill) ? (string)null : sidebarFill.TrimStart('#') },
                        { "profile_sidebar_border_color", string.IsNullOrEmpty(sidebarBorder) ? (string)null : sidebarBorder.TrimStart('#') },
                        { "include_entities", includeEntities.ToString().ToLower() },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    },
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

#if !NETFX_CORE
        /// <summary>
        /// sends an image file to Twitter to replace user image
        /// </summary>
        /// <remarks>
        /// You can only run this method with a period of time between executions; 
        /// otherwise you get WebException errors from Twitter
        /// </remarks>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, string imageFilePath, bool skipStatus)
        {
            return UpdateAccountImage(ctx, imageFilePath, skipStatus, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace user image
        /// </summary>
        /// <remarks>
        /// You can only run this method with a period of time between executions; 
        /// otherwise you get WebException errors from Twitter
        /// </remarks>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, string imageFilePath, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_image.json";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            var reqProc = new UserRequestProcessor<User>();
            var parameters = new Dictionary<string, string>
                    {
                        { "skip_status", skipStatus.ToString().ToLower() }
                    };

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostTwitterFile(accountUrl, parameters, imageFilePath, reqProc);

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        } 
#endif

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
        public static User UpdateAccountImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool skipStatus)
        {
            return UpdateAccountImage(ctx, image, fileName, imageType, true, skipStatus, null);
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
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool includeEntities, bool skipStatus)
        {
            return UpdateAccountImage(ctx, image, fileName, imageType, includeEntities, skipStatus, null);
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
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool includeEntities, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_image.json";

            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("image is required.", "image");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is required.", "fileName");
            }

            if (string.IsNullOrEmpty(imageType))
            {
                throw new ArgumentException("imageType is required.", "imageType");
            }

            var reqProc = new UserRequestProcessor<User>();
            var parameters = new Dictionary<string, string>
                    {
                        { "include_entities", includeEntities.ToString().ToLower() },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    };

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostTwitterImage(accountUrl, parameters, image, fileName, imageType, reqProc);

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

#if !NETFX_CORE
        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="tile">Tile image in background</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, string imageFilePath, bool tile, bool use, bool includeEntities, bool skipStatus)
        {
            return UpdateAccountBackgroundImage(ctx, imageFilePath, tile, use, includeEntities, skipStatus, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="tile">Tile image in background</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, string imageFilePath, bool tile, bool use, bool includeEntities, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_background_image.json";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            var parameters = new Dictionary<string, string>
            {
                { "include_entities", includeEntities.ToString().ToLower() },
                { "skip_status", skipStatus.ToString().ToLower() }
            };

            if (tile)
            {
                parameters.Add("tile", true.ToString().ToLower());
                parameters.Add("use", use.ToString().ToLower());
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostTwitterFile(accountUrl, parameters, imageFilePath, reqProc);

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }
#endif

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
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool tile, bool use, bool skipStatus)
        {
            return UpdateAccountBackgroundImage(ctx, image, fileName, imageType, tile, use, true, skipStatus, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="image">full path to file, including file name</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="tile">Tile image across background.</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool tile, bool use, bool includeEntities, bool skipStatus)
        {
            return UpdateAccountBackgroundImage(ctx, image, fileName, imageType, tile, use, includeEntities, skipStatus, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="image">full path to file, including file name</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="tile">Tile image across background.</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <param name="includeEntities">Set to false to not include entities. (default: true)</param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool tile, bool use, bool includeEntities, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_background_image.json";

            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("image is required.", "image");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is required.", "fileName");
            }

            if (string.IsNullOrEmpty(imageType))
            {
                throw new ArgumentException("imageType is required.", "imageType");
            }

            var parameters = new Dictionary<string, string>
            {
                { "include_entities", includeEntities.ToString().ToLower() },
                { "skip_status", skipStatus.ToString().ToLower() }
            };

            if (tile)
            {
                parameters.Add("tile", true.ToString().ToLower());
                parameters.Add("use", use.ToString().ToLower());
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostTwitterImage(accountUrl, parameters, image, fileName, imageType, reqProc);

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

        /// <summary>
        /// Allows removal of background image by setting back to the default background.  Once the background image is removed
        /// it can not be turned back on.
        /// </summary>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns></returns>
        public static User RemoveBackgroundImage(this TwitterContext ctx, bool skipStatus)
        {
            return RemoveBackgroundImage(ctx, skipStatus, null);
        }

        /// <summary>
        /// Allows removal of background image by setting back to the default background.  Once the background image is removed
        /// it can not be turned back on.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="skipStatus">Don't include status with response.</param>
        /// <returns></returns>
        public static User RemoveBackgroundImage(this TwitterContext ctx, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_background_image.json";
            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "use", "false" },
                        { "skip_status", skipStatus.ToString()}
                    },
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
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
        public static User UpdateAccountProfile(this TwitterContext ctx, string name, string url, string location, string description, bool skipStatus)
        {
            return UpdateAccountProfile(ctx, name, url, location, description, true, skipStatus, null);
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
        public static User UpdateAccountProfile(this TwitterContext ctx, string name, string url, string location, string description, bool includeEntities, bool skipStatus)
        {
            return UpdateAccountProfile(ctx, name, url, location, description, includeEntities, skipStatus, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new info</returns>
        public static User UpdateAccountProfile(this TwitterContext ctx, string name, string url, string location, string description, bool includeEntities, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile.json";

            if (string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(url) &&
                string.IsNullOrEmpty(location) &&
                string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("At least one of the text fields (name, email, url, location, or description) must be provided as arguments, but none are specified.", NoInputParam);
            }

            if (!string.IsNullOrEmpty(name) && name.Length > 20)
            {
                throw new ArgumentException("name must be no longer than 20 characters", "name");
            }

            if (!string.IsNullOrEmpty(url) && url.Length > 100)
            {
                throw new ArgumentException("url must be no longer than 100 characters", "url");
            }

            if (!string.IsNullOrEmpty(location) && location.Length > 30)
            {
                throw new ArgumentException("location must be no longer than 30 characters", "location");
            }

            if (!string.IsNullOrEmpty(description) && description.Length > 160)
            {
                throw new ArgumentException("description must be no longer than 160 characters", "description");
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
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
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

        /// <summary>
        /// Updates user's account settings
        /// </summary>
        /// <param name="trendLocationWeoid">WEOID for Trend Location the user is interested in.</param>
        /// <param name="sleepTimeEnabled">Turn on time periods when notifications won't be sent.</param>
        /// <param name="startSleepTime">Don't send notifications at this time or later this time.</param>
        /// <param name="endSleepTime">Start sending notifications again after this time.</param>
        /// <param name="timeZone">User's time zone.</param>
        /// <param name="lang">User's language.</param>
        /// <returns>Account information with Settings property populated.</returns>
        public static Account UpdateAccountSettings(this TwitterContext ctx, int? trendLocationWoeid, bool? sleepTimeEnabled, int? startSleepTime, int? endSleepTime, string timeZone, string lang)
        {
            return UpdateAccountSettings(ctx, trendLocationWoeid, sleepTimeEnabled, startSleepTime, endSleepTime, timeZone, lang, null);
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
        /// <param name="callback">Async Callback.</param>
        /// <returns>Account information with Settings property populated.</returns>
        public static Account UpdateAccountSettings(this TwitterContext ctx, int? trendLocationWoeid, bool? sleepTimeEnabled, int? startSleepTime, int? endSleepTime, string timeZone, string lang, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/settings.json";

            if (trendLocationWoeid == null &&
                sleepTimeEnabled == null &&
                startSleepTime == null &&
                endSleepTime == null &&
                string.IsNullOrEmpty(timeZone) &&
                string.IsNullOrEmpty(lang))
            {
                throw new ArgumentException("At least one parameter must be provided as arguments, but none are specified.", NoInputParam);
            }

            var reqProc = new AccountRequestProcessor<Account>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "trend_location_woeid", trendLocationWoeid.ToString() },
                        { "sleep_time_enabled", sleepTimeEnabled.ToString() },
                        { "start_sleep_time", startSleepTime.ToString() },
                        { "end_sleep_time", endSleepTime.ToString() },
                        { "time_zone", timeZone },
                        { "lang", lang }
                    },
                    response => reqProc.ProcessActionResult(response, AccountAction.Settings));

            Account acct = reqProc.ProcessActionResult(resultsJson, AccountAction.Settings);
            return acct;
        }

        /// <summary>
        /// Modify device information
        /// </summary>
        /// <param name="device">Which device to use.</param>
        /// <param name="includeEntitites">Set this to false to not add entitites to response. (default: true)</param>
        /// <returns></returns>
        public static Account UpdateDeliveryDevice(this TwitterContext ctx, DeviceType device, bool includeEntitites)
        {
            return UpdateDeliveryDevice(ctx, device, includeEntitites, null);
        }

        /// <summary>
        /// Modify device information
        /// </summary>
        /// <param name="device">Which device to use.</param>
        /// <param name="includeEntitites">Set this to false to not add entitites to response. (default: true)</param>
        /// <param name="callback">Async Callback.</param>
        /// <returns></returns>
        public static Account UpdateDeliveryDevice(this TwitterContext ctx, DeviceType device, bool includeEntitites, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_delivery_device.json";

            var reqProc = new AccountRequestProcessor<Account>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "device", device.ToString().ToLower() },
                        { "include_entities", includeEntitites.ToString().ToLower() }
                    },
                    response => reqProc.ProcessActionResult(response, AccountAction.Settings));

            Account acct = reqProc.ProcessActionResult(resultsJson, AccountAction.Settings);
            return acct;
        }

        /// <summary>
        /// Sends an image to Twitter to be placed as the user's profile banner.
        /// </summary>
        /// <param name="banner">byte[] containing image data.</param>
        /// <param name="callback">Async callback routine.</param>
        /// <returns>
        /// Account of authenticated user who's profile banner will be updated.
        /// Url of new banner will appear in ProfileBannerUrl property.
        /// </returns>
        public static User UpdateProfileBanner(this TwitterContext ctx, byte[] banner, string fileName, string imageType)
        {
            return UpdateProfileBanner(ctx, banner, fileName, imageType, 1252, 626, 0, 0, null);
        }
       
        /// <summary>
        /// Sends an image to Twitter to be placed as the user's profile banner.
        /// </summary>
        /// <param name="banner">byte[] containing image data.</param>
        /// <param name="callback">Async callback routine.</param>
        /// <returns>
        /// Account of authenticated user who's profile banner will be updated.
        /// Url of new banner will appear in ProfileBannerUrl property.
        /// </returns>
        public static User UpdateProfileBanner(this TwitterContext ctx, byte[] banner, string fileName, string imageType, Action<TwitterAsyncResponse<User>> callback)
        {
            return UpdateProfileBanner(ctx, banner, fileName, imageType, 1252, 626, 0, 0, callback);
        }

        /// <summary>
        /// Sends an image to Twitter to be placed as the user's profile banner.
        /// </summary>
        /// <param name="banner">byte[] containing image data.</param>
        /// <param name="width">Pixel width to clip image.</param>
        /// <param name="height">Pixel height to clip image.</param>
        /// <param name="offsetLeft">Pixels to offset start of image from the left.</param>
        /// <param name="offsetTop">Pixels to offset start of image from the top.</param>
        /// <param name="callback">Async callback routine.</param>
        /// <returns>
        /// Account of authenticated user who's profile banner will be updated.
        /// Url of new banner will appear in ProfileBannerUrl property.
        /// </returns>
        public static User UpdateProfileBanner(this TwitterContext ctx, byte[] banner, string fileName, string imageType, int width, int height, int offsetLeft, int offsetTop)
        {
            return UpdateProfileBanner(ctx, banner, fileName, imageType, width, height, offsetLeft, offsetTop, null);
        }

        /// <summary>
        /// Sends an image to Twitter to be placed as the user's profile banner.
        /// </summary>
        /// <param name="banner">byte[] containing image data.</param>
        /// <param name="width">Pixel width to clip image.</param>
        /// <param name="height">Pixel height to clip image.</param>
        /// <param name="offsetLeft">Pixels to offset start of image from the left.</param>
        /// <param name="offsetTop">Pixels to offset start of image from the top.</param>
        /// <param name="callback">Async callback routine.</param>
        /// <returns>
        /// Account of authenticated user who's profile banner will be updated.
        /// Url of new banner will appear in ProfileBannerUrl property.
        /// </returns>
        public static User UpdateProfileBanner(this TwitterContext ctx, byte[] banner, string fileName, string imageType, int width, int height, int offsetLeft, int offsetTop, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_banner.json";

            if (banner == null || banner.Length == 0)
            {
                throw new ArgumentException("banner is required.", "banner");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is required.", "fileName");
            }

            if (string.IsNullOrEmpty(imageType))
            {
                throw new ArgumentException("imageType is required.", "imageType");
            }

            var parameters = new Dictionary<string, string>
            {
                { "width", width.ToString() },
                { "height", height.ToString() },
                { "offset_left", offsetLeft.ToString() },
                { "offset_top", offsetTop.ToString() },
                { "banner", "IMAGE_DATA" }
            };

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostTwitterImage(accountUrl, parameters, banner, fileName, imageType, reqProc);

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

        /// <summary>
        /// Removes banner from authenticated user's profile.
        /// </summary>
        /// <returns>Empty User instance.</returns>
        public static User RemoveProfileBanner(this TwitterContext ctx)
        {
            return RemoveProfileBanner(ctx, null);
        }

        /// <summary>
        /// Removes banner from authenticated user's profile.
        /// </summary>
        /// <param name="callback">Async Callback.</param>
        /// <returns>Empty User instance.</returns>
        public static User RemoveProfileBanner(this TwitterContext ctx, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/remove_profile_banner.json";

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    accountUrl,
                    new Dictionary<string, string>(),
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }
    }
}
