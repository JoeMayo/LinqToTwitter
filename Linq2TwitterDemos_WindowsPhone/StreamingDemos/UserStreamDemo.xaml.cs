using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Linq2TwitterDemos_WindowsPhone.ViewModels;
using LinqToTwitter;
using Microsoft.Phone.Controls;

namespace Linq2TwitterDemos_WindowsPhone.StreamingDemos
{
    public partial class UserStreamDemo : PhoneApplicationPage
    {
        ObservableCollection<JsonContent> jsonCollection;

        public UserStreamDemo()
        {
            InitializeComponent();

            var streamVM = new StreamViewModel();
            DataContext = streamVM;
            jsonCollection = streamVM.JsonContent;
        }

        void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Show("Starting Stream...");

            int count = 0;
            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            Task.Run(async () =>
                await
                    (from strm in twitterCtx.Streaming
                     where strm.Type == StreamingType.User
                     select strm)
                    .StartAsync(async strm =>
                    {
                        Show(strm.Content);

                        if (count++ == 5)
                            strm.CloseStream();
                    }));
        }

        void Show(string content)
        {
            Dispatcher.BeginInvoke(() =>
               jsonCollection.Add(new JsonContent { Content = content }));
        }
    }
}