// (c) 2012// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
// UNLESS NOTED ALTERNATIVE SOURCE

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models;

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
            // Do a simple singleton
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
            // this sets the "loggingon" state; XAML is bound to this bool & will update
            mainWindowViewModel.LoggingOn = true;
            mainWindowViewModel.LoggedOn = false;
            mainWindowViewModel.needsCredentialsCommands = false;

            // get previously saved (securely) credentials from file
            var twittercreds = ManageTwitterCredentials.GetCredentialsFromFile();
            if (twittercreds.Valid)
            {
                // add the account to the connection; is also validates the credentials with twitter
                var twitteraccount = await App.twitterConnection.AddTwitterAccount(twittercreds);
                mainWindowViewModel.mainTwitterAccount = twitteraccount;

                // now wire up all the data, and we are logged on!
                mainWindowViewModel.Connect();
                mainWindowViewModel.LoggingOn = false;
                mainWindowViewModel.LoggedOn = true;
            }
            else
            {
                // we need to star the authorisation/authenication flow
                mainWindowViewModel.LoggingOn = false;
                mainWindowViewModel.LoggedOn = false;
                mainWindowViewModel.needsCredentialsCommands = true;
                mainWindowViewModel.stage1 = true;
            }
        }

        // In a perfect world, these would be on the ViewModel. 
        private void DisplayedTweet_MouseUp(object sender, MouseEventArgs me)
        {
            var gridItem = sender as Grid;
            if (gridItem == null) return;
            var attachedTweet = (Tweet)gridItem.Tag;
            if (attachedTweet == null) return;
            if (attachedTweet.User == null) return;
            Process.Start(String.Format("https://twitter.com/{0}/status/{1}",attachedTweet.User.ScreenName,attachedTweet.Id));
        }

        private void BoxKiteLogo_MouseUp(object sender, MouseEventArgs me)
        {
            Process.Start("https://github.com/nickhodge/BoxKite.Twitter/wiki");
        }
    }
}