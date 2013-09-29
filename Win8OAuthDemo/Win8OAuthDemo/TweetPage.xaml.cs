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
        public TweetPage()
        {
            this.InitializeComponent();

            SearchTextBox.Text = "LINQ to Twitter";
        }

        async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Linq2TwitterCredentials.txt is the default isolated store file name,
            // but you can change it and pass as an argument to LocalDataCredentials
            string fileName = "Linq2TwitterCredentials.txt";

            //
            // The code below demonstrates how to remove credentials from isolated storage
            //

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

            //
            // Comment above and uncomment below to test WinRtApplicationOnlyAuthorizer
            //

            //var auth = new WinRtApplicationOnlyAuthorizer
            //{
            //    Credentials = new InMemoryCredentials
            //    {
            //        ConsumerKey = "",
            //        ConsumerSecret = ""
            //    }
            //};

            if (auth == null || !auth.IsAuthorized)
            {
                await auth.AuthorizeAsync();
            }

            var twitterCtx = new TwitterContext(auth);

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
