using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows favorites demos
    /// </summary>
    public class FavoritesDemos
    {
        /// <summary>
        /// Run all favorites related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            FavoritesQueryDemo(twitterCtx);
            //CreateFavoriteDemo(twitterCtx);
            //DestroyFavoriteDemo(twitterCtx);
        }

        #region Favorites Demos

        private static void DestroyFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.DestroyFavorite("1552797863");

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// Shows how to create a Favorite
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void CreateFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.CreateFavorite("1552797863");

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// shows how to request a favorites list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FavoritesQueryDemo(TwitterContext twitterCtx)
        {
            var favorites =
                from fav in twitterCtx.Favorites
                where fav.Type == FavoritesType.Favorites
                select fav;

            foreach (var fav in favorites)
            {
                Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    fav.User.Name, fav.Text);
            }
        }

        #endregion
    }
}
