using AngleSharp.Dom.Html;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Netstats.Network
{
    public class DummyNetworkApi : INetworkApi
    {
        static IHtmlDocument sessionPage { get; }
        static IHtmlDocument confirmationPage { get; }
        static IHtmlDocument AuthenticationFailedPage { get; }
        static IHtmlDocument LogoutPage { get; }

        
        static DummyNetworkApi()
        {
            sessionPage = Utilities.SingletonHtmlParser.Parse(File.ReadAllText("sessionPage.html"));
            confirmationPage = Utilities.SingletonHtmlParser.Parse(File.ReadAllText("confirmationPage.html"));
            AuthenticationFailedPage = Utilities.SingletonHtmlParser.Parse(File.ReadAllText("authFailedPage.html"));
            LogoutPage = Utilities.SingletonHtmlParser.Parse(File.ReadAllText("logoutPage.html"));
        }

        public async Task<IHtmlDocument> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes")
        {
            await Task.Delay(TimeSpan.FromSeconds(2), token);
            return sessionPage;
        }

        public async Task<IHtmlDocument> Login(string username, string password, CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), token);
            return sessionPage;
        }

        public async Task<IHtmlDocument> Logout(string sessionId)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return LogoutPage;
        }

        public async Task<IHtmlDocument> Refresh(string sessionId)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return sessionPage;
        }
    }
}
