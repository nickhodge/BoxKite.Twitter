using BoxKite.Twitter;
using System.Windows;

namespace BoxKite.WPFSimpleClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TwitterConnection twitterConnection;

        public App()
        {
            //twitterConnection = new TwitterConnection("needs client key from dev.twitter.com", "needs client secret from dev.twitter.com");
            twitterConnection = new TwitterConnection("3izxqWiej34yTlofisw", "uncicYQtDx5SoWth1I9xcn5vrpczUct1Oz9ydwTY4");
        }
    }
}
