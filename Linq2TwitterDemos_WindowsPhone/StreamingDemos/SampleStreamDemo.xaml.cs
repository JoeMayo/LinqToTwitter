using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Linq2TwitterDemos_WindowsPhone.ViewModels;
using LinqToTwitter;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Linq2TwitterDemos_WindowsPhone.StreamingDemos
{
    public partial class SampleStreamDemo : PhoneApplicationPage
    {
        ObservableCollection<JsonContent> jsonCollection;

        public SampleStreamDemo()
        {
            InitializeComponent();

            var streamVM = new StreamViewModel();
            DataContext = streamVM;
            jsonCollection = streamVM.JsonContent;
        }

        async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Show("Starting Stream...");

            // Compression doesn't work correctly for streaming in Windows Phone
            SharedState.Authorizer.SupportsCompression = false;
            int count = 0;

            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Sample
                 select strm)
                .StartAsync(async strm =>
                {
                    Show(strm.Content);

                    if (count++ >= 5)
                        strm.CloseStream();
                });
        }

        void Show(string content)
        {
            Dispatcher.BeginInvoke(() =>
               jsonCollection.Add(new JsonContent { Content = content }));
        }
    }
}