using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using XamarinFormsSamples.Models;

namespace XamarinFormsSamples.Views
{
    public partial class TweetView : ContentPage
    {
        TweetingViewModel tweetVM = new TweetingViewModel();

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
