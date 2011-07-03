using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    public class LegalDemos
    {
        public static void Run(TwitterContext twitterCtx)
        {
            //PrivacyDemo(twitterCtx);
            TosDemo(twitterCtx);
        }

        private static void PrivacyDemo(TwitterContext twitterCtx)
        {
            var privacyPolicy =
                (from legal in twitterCtx.Legal
                 where legal.Type == LegalType.Privacy
                 select legal.Text)
                .SingleOrDefault();

            Console.WriteLine("\n\n{0}\n", privacyPolicy);
        }

        private static void TosDemo(TwitterContext twitterCtx)
        {
            var tosPolicy =
                (from legal in twitterCtx.Legal
                 where legal.Type == LegalType.TOS
                 select legal.Text)
                .SingleOrDefault();

            Console.WriteLine("\n\n{0}\n", tosPolicy);
        }
    }
}
