using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Core.Network.Api
{
    public class DummyApi : INetworkApi
    {
        static string sessionPage { get; }
        static string confirmationPage { get; }
        static string AuthenticationFailedPage { get; }
        static string LogoutPage { get; }

        //static Lazy<HtmlParser> singletonParser = new Lazy<HtmlParser>();
        //static HtmlParser SingletonParser { get { return singletonParser.Value;  } }

        static DummyApi() 
        {
            sessionPage = File.ReadAllText(@"sessionPage.html");
            confirmationPage = File.ReadAllText(@"confirmationPage.html");
            AuthenticationFailedPage = File.ReadAllText(@"authFailedPage.html");
            LogoutPage = File.ReadAllText(@"logoutPage.html");
        }

        public async Task<string> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes")
        {
            await Task.Delay(TimeSpan.FromSeconds(1.5), token);
            return sessionPage;
        }

        public async Task<string> Login(string username, string password, CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(1.5), token);
            return sessionPage;
        }

        public async Task<string> Logout(string sessionId)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return LogoutPage;
        }

        public async Task<string> Refresh(string sessionId)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return sessionPage;
        }
    }
}
