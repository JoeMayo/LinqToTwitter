using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LinqToTwitter
{
    public static class AccountExtensions
    {
        const string NoInputParam = "NoInput";

        /// <summary>
        /// Ends the session for the currently logged in user
        /// </summary>
        /// <returns>true</returns>
        public static TwitterHashResponse EndAccountSession(this TwitterContext ctx)
        {
            return EndAccountSession(ctx, null);
        }

        /// <summary>
        /// Ends the session for the currently logged in user
        /// </summary>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>true</returns>
        public static TwitterHashResponse EndAccountSession(this TwitterContext ctx, Action<TwitterAsyncResponse<Account>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/end_session.json";

            var reqProc = new AccountRequestProcessor<Account>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var results =
                exec.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            var acct = reqProc.ProcessActionResult(results, AccountAction.EndSession);

            if (acct != null)
            {
                return acct.EndSessionStatus;
            }

            throw new WebException("Unknown Twitter Response.");
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
        /// <returns>User info with new colors</returns>
        public static User UpdateAccountColors(this TwitterContext ctx, string background, string text, string link, string sidebarFill, string sidebarBorder)
        {
            return UpdateAccountColors(ctx, background, text, link, sidebarFill, sidebarBorder, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User info with new colors</returns>
        public static User UpdateAccountColors(this TwitterContext ctx, string background, string text, string link, string sidebarFill, string sidebarBorder, Action<TwitterAsyncResponse<User>> callback)
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
                exec.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "profile_background_color", background.TrimStart('#') },
                        { "profile_text_color", text.TrimStart('#') },
                        { "profile_link_color", link.TrimStart('#') },
                        { "profile_sidebar_fill_color", sidebarFill.TrimStart('#') },
                        { "profile_sidebar_border_color", sidebarBorder.TrimStart('#') }
                    },
                    reqProc);

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
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, string imageFilePath)
        {
            return UpdateAccountImage(ctx, imageFilePath, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace user image
        /// </summary>
        /// <remarks>
        /// You can only run this method with a period of time between executions; 
        /// otherwise you get WebException errors from Twitter
        /// </remarks>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, string imageFilePath, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_image.json";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostTwitterFile(accountUrl, null, imageFilePath, reqProc);

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
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, byte[] image, string fileName, string imageType)
        {
            return UpdateAccountImage(ctx, image, fileName, imageType, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, Action<TwitterAsyncResponse<User>> callback)
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

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostTwitterImage(accountUrl, null, image, fileName, imageType, reqProc);

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
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, string imageFilePath, bool tile, bool use)
        {
            return UpdateAccountBackgroundImage(ctx, imageFilePath, tile, use, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="tile">Tile image in background</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, string imageFilePath, bool tile, bool use, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = ctx.BaseUrl + "account/update_profile_background_image.json";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            Dictionary<string, string> parameters = null;

            if (tile)
            {
                parameters = new Dictionary<string, string>
                {
                    { "tile", true.ToString().ToLower() },
                    { "use", use.ToString().ToLower() }
                };
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
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool tile, bool use)
        {
            return UpdateAccountBackgroundImage(ctx, image, fileName, imageType, tile, use, null);
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
        /// <returns>User with new image info</returns>
        public static User UpdateAccountBackgroundImage(this TwitterContext ctx, byte[] image, string fileName, string imageType, bool tile, bool use, Action<TwitterAsyncResponse<User>> callback)
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

            Dictionary<string, string> parameters = null;

            if (tile)
            {
                parameters = new Dictionary<string, string>
                {
                    { "tile", true.ToString().ToLower() },
                    { "use", use.ToString().ToLower() }
                };
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
        /// Update account profile info
        /// </summary>
        /// <param name="name">User Name</param>
        /// <param name="url">Web Address</param>
        /// <param name="location">Geographic Location</param>
        /// <param name="description">Personal Description</param>
        /// <returns>User with new info</returns>
        public static User UpdateAccountProfile(this TwitterContext ctx, string name, string url, string location, string description)
        {
            return UpdateAccountProfile(ctx, name, url, location, description, null);
        }

        /// <summary>
        /// Update account profile info
        /// </summary>
        /// <param name="name">User Name</param>
        /// <param name="url">Web Address</param>
        /// <param name="location">Geographic Location</param>
        /// <param name="description">Personal Description</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new info</returns>
        public static User UpdateAccountProfile(this TwitterContext ctx, string name, string url, string location, string description, Action<TwitterAsyncResponse<User>> callback)
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
                exec.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "name", name },
                        { "url", url },
                        { "location", location },
                        { "description", description }
                    },
                    reqProc);

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
                exec.ExecuteTwitter(
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
                    reqProc);

            Account acct = reqProc.ProcessActionResult(resultsJson, AccountAction.Settings);
            return acct;
        }
    }
}
