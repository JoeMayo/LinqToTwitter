//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace LinqToTwitter
//{
//    public static class ControlStreamExtensions
//    {
//        /// <summary>
//        /// Adds one or more users to a Site Stream
//        /// </summary>
//        /// <param name="ctx">Twitter Context</param>
//        /// <param name="userIDs">List of user IDs to add to Site Stream</param>
//        /// <param name="streamID">ID of Site Stream to add users to</param>
//        /// <returns>Control Stream with CommandResponse property for Twitter's response message</returns>
//        public static ControlStream AddSiteStreamUser(this TwitterContext ctx, List<ulong> userIDs, string streamID)
//        {
//            return AddSiteStreamUser(ctx, userIDs, streamID, null);
//        }

//        /// <summary>
//        /// Adds one or more users to a Site Stream
//        /// </summary>
//        /// <param name="ctx">Twitter Context</param>
//        /// <param name="userIDs">List of user IDs to add to Site Stream</param>
//        /// <param name="streamID">ID of Site Stream to add users to</param>
//        /// <param name="callback">Async Callback</param>
//        /// <returns>Control Stream with CommandResponse property for Twitter's response message</returns>
//        public static ControlStream AddSiteStreamUser(this TwitterContext ctx, List<ulong> userIDs, string streamID, Action<TwitterAsyncResponse<ControlStream>> callback)
//        {
//            if (string.IsNullOrEmpty(streamID)) throw new ArgumentNullException("streamID", "streamID is required.");
            
//            var newUrl = ctx.SiteStreamUrl + "site/c/" + streamID + "/add_user.json";

//            string userIDString = string.Join(",", userIDs.Select(user => user.ToString()).ToArray());

//            var reqProc = new ControlStreamRequestProcessor<ControlStream>();

//            var twitExe = ctx.TwitterExecutor;

//            twitExe.AsyncCallback = callback;
//            var resultsJson =
//                twitExe.PostToTwitter(
//                    newUrl,
//                    new Dictionary<string, string>
//                    {
//                        {"user_id", userIDString}
//                    },
//                    response => reqProc.ProcessActionResult(response, ControlStreamType.Info));

//            ControlStream cs = reqProc.ProcessActionResult(resultsJson, ControlStreamType.Info);
//            return cs;
//        }
        
//        /// <summary>
//        /// Removes one or more users from a Site Stream
//        /// </summary>
//        /// <param name="ctx">Twitter Context</param>
//        /// <param name="userIDs">List of user IDs to remove from Site Stream</param>
//        /// <param name="streamID">ID of Site Stream to remove users from</param>
//        /// <returns>Control Stream with CommandResponse property for Twitter's response message</returns>
//        public static ControlStream RemoveSiteStreamUser(this TwitterContext ctx, List<ulong> userIDs, string streamID)
//        {
//            return RemoveSiteStreamUser(ctx, userIDs, streamID, null);
//        }

//        /// <summary>
//        /// Removes one or more users from a Site Stream
//        /// </summary>
//        /// <param name="ctx">Twitter Context</param>
//        /// <param name="userIDs">List of user IDs to remove from Site Stream</param>
//        /// <param name="streamID">ID of Site Stream to remove users from</param>
//        /// <param name="callback">Async Callback</param>
//        /// <returns>Control Stream with CommandResponse property for Twitter's response message</returns>
//        public static ControlStream RemoveSiteStreamUser(this TwitterContext ctx, List<ulong> userIDs, string streamID, Action<TwitterAsyncResponse<ControlStream>> callback)
//        {
//            if (string.IsNullOrEmpty(streamID)) throw new ArgumentNullException("streamID", "streamID is required.");

//            var newUrl = ctx.SiteStreamUrl + "site/c/" + streamID + "/remove_user.json";

//            string userIDString = string.Join(",", userIDs.Select(user => user.ToString()).ToArray());

//            var reqProc = new ControlStreamRequestProcessor<ControlStream>();

//            var twitExe = ctx.TwitterExecutor;

//            twitExe.AsyncCallback = callback;
//            var resultsJson =
//                twitExe.PostToTwitter(
//                    newUrl,
//                    new Dictionary<string, string>
//                    {
//                        {"user_id", userIDString}
//                    },
//                    response => reqProc.ProcessActionResult(response, ControlStreamType.Info));

//            ControlStream cs = reqProc.ProcessActionResult(resultsJson, ControlStreamType.Info);
//            return cs;
//        }
//    }
//}
