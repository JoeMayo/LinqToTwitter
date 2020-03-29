using Linq2TwitterDemos_XForms.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Linq2TwitterDemos_XForms.Views
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
