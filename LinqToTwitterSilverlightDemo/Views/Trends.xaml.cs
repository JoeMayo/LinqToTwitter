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

namespace LinqToTwitterSilverlightDemo.Views
{
    public partial class Trends : Page
    {
        public Trends()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void AvailableTrendsButton_Click(object sender, RoutedEventArgs e)
        {
            var twitterCtx = new TwitterContext();

            (from trend in twitterCtx.Trends
             where trend.Type == TrendType.Available
             select trend)
            .AsyncCallback(trends =>
                Dispatcher.BeginInvoke(() =>
                {
                    AvailableTrendsGrid.ItemsSource = trends.First().Locations;
                }))
            .SingleOrDefault();
        }

    }
}
