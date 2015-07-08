using System;
using System.Diagnostics;
using Xamarin.Forms;
using XamarinFormsSamples.Models;

namespace XamarinFormsSamples.Views
{
    public partial class TweetView : ContentPage
    {
        TweetViewModel tweetVM = new TweetViewModel();

        public TweetView()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = tweetVM;
            try
            {
                await tweetVM.InitTweetViewModel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
