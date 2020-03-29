using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Linq2TwitterDemos_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SharedState.Authorizer == null)
                new OAuth().Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var type = Type.GetType((sender as MenuItem).Tag as string);
            Window formInst = (Window)Activator.CreateInstance(type);

            formInst.Show();
        }
    }
}
