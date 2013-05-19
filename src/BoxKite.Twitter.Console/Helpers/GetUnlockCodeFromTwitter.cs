using System.Diagnostics;

namespace BoxKite.Twitter.Console
{
    public class GetUnlockCodeFromTwitter : IGetUnlockCodeFromTwitter
    {
        public void DisplayAuthInBrowser(string u)
        {
            Process.Start(u);
        }
    }
}
