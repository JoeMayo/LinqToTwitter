using LinqToTwitter;
using System;
using System.Linq;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows help demos
    /// </summary>
    public class HelpDemos
    {
        /// <summary>
        /// Run all help related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //PerformHelpTest(twitterCtx);
            GetHelpConfiguration(twitterCtx);
            //GetHelpLanguages(twitterCtx);
        }

        #region Help Demos

        /// <summary>
        /// shows how to perform a help test
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PerformHelpTest(TwitterContext twitterCtx)
        {
            var helpResult =
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Test
                 select test)
                .SingleOrDefault();

            Console.WriteLine("Test Result: " + helpResult.OK);
        }

        /// <summary>
        /// shows how to get configuration info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetHelpConfiguration(TwitterContext twitterCtx)
        {
            var helpResult =
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Configuration
                 select test)
                .SingleOrDefault();

            Configuration cfg = helpResult.Configuration;

            Console.WriteLine("Short URL Length: " + cfg.ShortUrlLength);
            Console.WriteLine("Short URL HTTPS Length: " + cfg.ShortUrlLengthHttps);
            Console.WriteLine("Non-UserName Paths: ");
            foreach (var name in cfg.NonUserNamePaths)
            {
                Console.WriteLine("\t" + name);
            }
            Console.WriteLine("Photo Size Limit: " + cfg.PhotoSizeLimit);
            Console.WriteLine("Max Media Per Upload: " + cfg.MaxMediaPerUpload);
            Console.WriteLine("Characters Reserved Per Media: " + cfg.CharactersReservedPerMedia);
            Console.WriteLine("Photo Sizes");
            foreach (var photo in cfg.PhotoSizes)
            {
                Console.WriteLine("\t" + photo.Type);
                Console.WriteLine("\t\t" + photo.Width);
                Console.WriteLine("\t\t" + photo.Height);
                Console.WriteLine("\t\t" + photo.Resize);
            }
        }

        /// <summary>
        /// shows how to perform a help test
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetHelpLanguages(TwitterContext twitterCtx)
        {
            var helpResult =
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Languages
                 select test)
                .SingleOrDefault();

            foreach (var lang in helpResult.Languages)
            {
                Console.WriteLine("{0}({1}): {2}", lang.Name, lang.Code, lang.Status);
            }
        }

        #endregion
    }
}
