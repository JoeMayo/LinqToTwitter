using System;
using System.Diagnostics;

using Xamarin.Forms;
using XamarinFormsPortable.Models;

namespace XamarinFormsPortable.Views
{
    public partial class TweetView : ContentPage
    {
        readonly TweetingViewModel tweetVM = new TweetingViewModel();

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
