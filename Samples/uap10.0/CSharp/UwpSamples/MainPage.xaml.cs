using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpSamples
{
    /// <summary>
    /// Navigate to LINQ to Twitter Samples
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));
        }

        private void tweetButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TweetPage));
        }

        private void streamButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SampleStreamPage));
        }
    }
}
