using System;
using System.ComponentModel;
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
            this.DataContext = mainWindowViewModel;   
        }

        private async void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            mainWindowViewModel.LoggingOn = true;
            var tc = ManageTwitterCredentials.GetCredentialsFromFile();
            if (!tc.Valid) return;
            var ta = await App.twitterConnection.AddTwitterAccount(tc);
            mainWindowViewModel.mainTwitterAccount = ta;
            mainWindowViewModel.Connect();
            mainWindowViewModel.LoggingOn = false;
        }
    }
}