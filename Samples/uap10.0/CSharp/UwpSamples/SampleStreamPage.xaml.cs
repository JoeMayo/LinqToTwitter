using LinqToTwitter;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UwpSamples
{
    /// <summary>
    /// Demos usign Universal Authorizer and Stream API
    /// </summary>
    public sealed partial class SampleStreamPage : Page
    {
        public SampleStreamPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var authorizer = new UniversalAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                Callback = "http://github.com/JoeMayo/LinqToTwitter"
            };

            await authorizer.AuthorizeAsync();

            SharedState.Authorizer = authorizer;

            DataContext = new SampleStreamViewModel();
        }
    }
}
