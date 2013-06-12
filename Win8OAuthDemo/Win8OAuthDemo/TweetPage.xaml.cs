using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LinqToTwitter;
using MetroOAuthDemo.Common;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace MetroOAuthDemo
{
    public sealed partial class TweetPage : LayoutAwarePage
    {
        WinRtAuthorizer auth;

        public TweetPage()
        {
            this.InitializeComponent();

            SearchTextBox.Text = "LINQ to Twitter";

            auth = new WinRtAuthorizer
            {
                Credentials = new LocalDataCredentials
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                UseCompression = true,
                Callback = new Uri("http://linqtotwitter.codeplex.com/")
            };
        }

        async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "Linq2TwitterCredentials.txt";
            //var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            //if (files.Any(storFile => storFile.Name == fileName))
            //{
            //    var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            //    await file.DeleteAsync();
            //}

            //var credentials = new LocalDataCredentials(fileName);
            //await credentials.ClearAsync();

            var auth = new WinRtAuthorizer
            {
                Credentials = new LocalDataCredentials(fileName)
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                UseCompression = true,
                Callback = new Uri("http://linqtotwitter.codeplex.com/")
            };

            if (auth == null || !auth.IsAuthorized)
            {
                await auth.AuthorizeAsync();
            }

            var twitterCtx = new TwitterContext(auth);

            string status = "twitter sending test: " + DateTime.Now.ToString();
            Debug.WriteLine("------------ status: {0}", status);
            var ppp = new Dictionary<string, string>
			{
				{ "status", status }
			};
            string queryString = "/statuses/update.json";
            string result = twitterCtx.ExecuteRaw(queryString, ppp);
            Debug.WriteLine("------------ result: {0}", result);

            var searchResponse =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == SearchTextBox.Text
                 select search)
                .SingleOrDefault();

            string message =
                string.Format(
                    "Search returned {0} statuses",
                    searchResponse.Statuses.Count);

            await new MessageDialog(message, "Search Complete").ShowAsync();
        }
    }
}
