using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows user demos
    /// </summary>
    public class UserDemos
    {
        /// <summary>
        /// Run all user related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //UserShowWithIDQueryDemo(twitterCtx);
            //UserShowWithScreenNameQueryDemo(twitterCtx);
            //UserShowForAuthenticatedUser(twitterCtx);
            UsersLookupDemo(twitterCtx);
            //UserSearchDemo(twitterCtx);
            //UserSuggestedCategoriesListQueryDemo(twitterCtx);
            //UserSuggestedCategoriesListWithLangQueryDemo(twitterCtx);
            //UsersInSuggestedCategoryQueryDemo(twitterCtx);
            //UserShowLoggedInUserQueryDemo(twitterCtx);
            //CategoryStatusDemo(twitterCtx);
            //ContributeeDemo(twitterCtx);
            //ContributorDemo(twitterCtx);
            //BannerSizesDemo(twitterCtx);
        }

        private static void CategoryStatusDemo(TwitterContext twitterCtx)
        {
            var catUsers =
                (from user in twitterCtx.User
                 where user.Type == UserType.CategoryStatus &&
                       user.Slug == "Technology"
                 select user)
                .ToList();

            Console.WriteLine("Tweets from Suggested Users in Technology Category: \n");

            catUsers.ForEach(user => 
                Console.WriteLine(
                    "User: {0}\nTweet: {1}\n", 
                    user.Identifier.ScreenName, 
                    user.Status == null ?
                        "<Tweet not available.>" : 
                        user.Status.Text));
        }

        /// <summary>
        /// Shows how to search for a user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserSearchDemo(TwitterContext twitterCtx)
        {
            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Search &&
                       usr.Query == "Joe Mayo"
                 select usr)
                 .FirstOrDefault();

            Console.WriteLine("User Name: " + user.Identifier.ScreenName);
        }

        /// <summary>
        /// Shows how to perform a lookup of specified user details
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void UsersLookupDemo(TwitterContext twitterCtx)
        {
            var users =
                (from user in twitterCtx.User
                 where user.Type == UserType.Lookup &&
                       user.ScreenName == "JoeMayo,LinqToTweeter,NewStarCw46"
                 select user)
                 .ToList();

            users.ForEach(user => Console.WriteLine("Name: " + user.Identifier.ScreenName));
        }

        /// <summary>
        /// shows how to query for users in a suggested category
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UsersInSuggestedCategoryQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Category &&
                       tweet.Slug == "funny"
                 select tweet)
                 .ToList();

            users.ForEach(
                user => Console.WriteLine("User: " + user.Identifier.ScreenName));
        }

        /// <summary>
        /// shows how to query suggested categories
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserSuggestedCategoriesListQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Categories
                select tweet;

            var user = users.SingleOrDefault();

            user.Categories.ForEach(
                cat => Console.WriteLine("Category: " + cat.Name));
        }

        /// <summary>
        /// shows how to query suggested categories
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserSuggestedCategoriesListWithLangQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from usr in twitterCtx.User
                where usr.Type == UserType.Categories &&
                      usr.Lang == "it"
                select usr;

            var user = users.SingleOrDefault();

            user.Categories.ForEach(
                cat => Console.WriteLine("Category: " + cat.Name));
        }

        /// <summary>
        /// shows how to query authenticated user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowForAuthenticatedUser(TwitterContext twitterCtx)
        {
            string screenName = twitterCtx.AuthorizedClient.ScreenName;

            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Show &&
                       usr.ScreenName == screenName
                 select usr)
                .SingleOrDefault();

            var name = user.Identifier.ScreenName;
            var lastStatus = user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine("\nName: {0}, Last Tweet: {1}\n", name, lastStatus);
        }

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowWithScreenNameQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Show &&
                      tweet.ScreenName == "JoeMayo"
                select tweet;

            var user = users.SingleOrDefault();

            var name = user.Identifier.ScreenName;
            var lastStatus = user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine();
            Console.WriteLine("Name: {0}, Last Tweet: {1}\n", name, lastStatus);
        }

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowLoggedInUserQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from usr in twitterCtx.User
                where usr.Type == UserType.Show &&
                      usr.ScreenName == twitterCtx.UserName
                select usr;

            var user = users.SingleOrDefault();

            var name = user.Identifier.ScreenName;
            var lastStatus = user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine();
            Console.WriteLine("Name: {0}, Last Tweet: {1}\n", name, lastStatus);
        }

        /// <summary>
        /// Uses LINQ to Twitter to discover password to 1st Chirp conference
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowWithIDQueryDemo(TwitterContext twitterCtx)
        {
            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Show &&
                      usr.UserID == "6253282"
                 select usr)
                 .SingleOrDefault();

            Console.WriteLine(
                "The password to Chirp is: {0}",
                user.Identifier.ScreenName);
        }

        static void ContributeeDemo(TwitterContext twitterCtx)
        {
            var users =
                (from user in twitterCtx.User
                 where user.Type == UserType.Contributees &&
                       user.ScreenName == "biz"
                 select user)
                .ToList();

            users.ForEach(
                user => Console.WriteLine("User: " + user.Identifier.ScreenName));
        }

        static void ContributorDemo(TwitterContext twitterCtx)
        {
            var users =
                (from user in twitterCtx.User
                 where user.Type == UserType.Contributors &&
                       user.ScreenName == "twitter"
                 select user)
                .ToList();

            users.ForEach(
                user => Console.WriteLine("User: " + user.Identifier.ScreenName));
        }

        static void BannerSizesDemo(TwitterContext twitterCtx)
        {
            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.BannerSizes &&
                       usr.ScreenName == "JoeMayo"
                 select usr)
                .SingleOrDefault();

            user.BannerSizes.ForEach(size => 
                Console.WriteLine(
                    "Label: {0}, W: {1} H: {2} URL: {3}",
                    size.Label, size.Width, size.Height, size.Url));
        }
    }
}
