using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using LinqToTwitter;
using Microsoft.Phone.Controls;

namespace WindowsPhoneDemo
{
    public partial class UserStream : PhoneApplicationPage
    {
        public UserStream()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ITwitterAuthorizer auth = SharedState.Authorizer;

            if (auth == null || !auth.IsAuthorized)
            {
                NavigationService.Navigate(new Uri("/OAuth.xaml", UriKind.Relative));
            }
            else
            {
                int count = 0;
                var twitterCtx = new TwitterContext(auth);
                var collection = new ObservableCollection<StreamItem>();
                StreamListBox.ItemsSource = collection;

                (from strm in twitterCtx.UserStream
                 where strm.Type == UserStreamType.User
                 select strm)
                .StreamingCallback(strm => Dispatcher.BeginInvoke(() =>
                {
                    if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                    {
                        WebException wex = strm.Error as WebException;
                        if (wex != null && wex.Status == WebExceptionStatus.ConnectFailure)
                        {
                            MessageBox.Show(wex.Message + " You might want to reconnect.");
                        }

                        MessageBox.Show(strm.Error.ToString());
                        return;
                    }

                    string message = 
                        string.IsNullOrWhiteSpace(strm.Content) ? "Keep-Alive" : strm.Content;
                    collection.Add(
                        new StreamItem
                        {
                            Message = DateTime.Now.ToString() + ": " + message
                        });

                    if (count++ >= 25)
                    {
                        strm.CloseStream();
                        MessageBox.Show("Stream for this demo is closing...");
                    }
                }))
                .SingleOrDefault();
            }
        }
    }
}