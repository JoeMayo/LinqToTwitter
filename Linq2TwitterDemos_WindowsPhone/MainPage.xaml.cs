using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Linq2TwitterDemos_WindowsPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            if (SharedState.Authorizer == null)
                NavigationService.Navigate(new Uri("/OAuth.xaml", UriKind.Relative));
        }

        private void DemoButton_Click(object sender, RoutedEventArgs e)
        {
            string destinationUri = (sender as Button).Tag as string;
            NavigationService.Navigate(new Uri(destinationUri, UriKind.Relative));
        }
    }
}