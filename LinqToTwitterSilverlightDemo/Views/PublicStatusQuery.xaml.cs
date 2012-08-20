using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using LinqToTwitter;
using System;

namespace LinqToTwitterSilverlightDemo.Views
{
    public partial class PublicStatusQuery : Page
    {
        public PublicStatusQuery()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var twitterCtx = new TwitterContext();

            (from search in twitterCtx.Search
             where search.Type == SearchType.Search &&
                   search.Query == "LINQ To Twitter"
             select search)
            .MaterializedAsyncCallback(resp => Dispatcher.BeginInvoke(() =>
            {
                if (resp.Status != TwitterErrorStatus.Success)
                {
                    Exception ex = resp.Error;
                    // handle exception
                    throw ex;
                }

                Search srch = resp.State.First();
                dataGrid1.ItemsSource = srch.Results; 
            }));
        }
    }
}
