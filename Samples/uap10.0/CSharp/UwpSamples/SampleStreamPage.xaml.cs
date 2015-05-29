using LinqToTwitter;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpSamples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
