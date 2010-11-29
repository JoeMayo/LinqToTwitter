using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using LinqToTwitter;
using System.Threading;

namespace LinqToTwitterSilverlightDemo.Views
{
    public partial class StatusUpdate : Page
    {
        private ManualResetEvent pinEvent = new ManualResetEvent(false);

        public StatusUpdate()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //var auth = new PinAuthorizer
            //{
            //    Credentials = new InMemoryCredentials
            //    {
            //        ConsumerKey = "",
            //        ConsumerSecret = ""
            //    },
            //    UseCompression = true,
            //    GoToTwitterAuthorization = pageLink => webBrowser1.Navigate(new Uri(pageLink)),
            //    GetPin = () =>
            //    {
            //        pinEvent.WaitOne();
            //        return PinTextBox.Text;
            //    }
            //};

            //auth.Authorize();

            //var twitterCtx = new TwitterContext(auth, "https://api.twitter.com/1/", "https://search.twitter.com/");

            //twitterCtx.UpdateStatus("Silverlight OOB Test, " + DateTime.Now.ToString());
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //pinEvent.Set();
        }

    }
}
