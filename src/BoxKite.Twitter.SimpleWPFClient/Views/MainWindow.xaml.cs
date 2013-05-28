using System;
using System.Windows;
using BoxKite.Twitter.Helpers;

namespace BoxKite.WPFSimpleClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel mainWindowViewModel;
        
        public MainWindow()
        {
            if (mainWindowViewModel == null)
                mainWindowViewModel = new MainWindowViewModel(); 
            InitializeComponent();
            // now set the DataContext, used for XAML binding, to the new viewmodel created above
            this.DataContext = mainWindowViewModel;   
        }

        // once initialized, wire the up the view model.
        // also, manage the credentials to twitter
        private async void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            mainWindowViewModel.LoggingOn = true;
            var twittercreds = ManageTwitterCredentials.GetCredentialsFromFile();
            if (!twittercreds.Valid) return;
            var twitteraccount = await App.twitterConnection.AddTwitterAccount(twittercreds);
            mainWindowViewModel.mainTwitterAccount = twitteraccount;
            mainWindowViewModel.Connect();
            mainWindowViewModel.LoggingOn = false;
        }
    }
}