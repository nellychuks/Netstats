using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Netstats.Network.Api.Interfaces;

namespace Netstats.Network.Api
{
    public class DummyNetworkApi : INetworkApi
    {
        private static string SessionPage { get; }
        private static string ConfirmationPage { get; }
        private static string AuthenticationFailedPage { get; }
        private static string LogoutPage { get; }

        static DummyNetworkApi() 
        {
            SessionPage = File.ReadAllText(@"sessionPage.html");

            ConfirmationPage = File.ReadAllText(@"confirmationPage.html");

            AuthenticationFailedPage = File.ReadAllText(@"authFailedPage.html");

            LogoutPage = File.ReadAllText(@"logoutPage.html");
        }

        public async Task<string> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes")
        {
            await Task.Delay(TimeSpan.FromSeconds(1), token);
            return SessionPage;
        }

        public async Task<string> Login(string username, string password, CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), token);
            var response = await HtmlPageMixins.ParsePageAsync(SessionPage);

            // Json will contain the session id of the current session and will
            // need to be included in subsequent server call
            return JsonConvert.SerializeObject(new { Id = response.GetSessionId() }, Formatting.Indented);
        }

        public async Task<string> Refresh(string sessionId)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            var response = await HtmlPageMixins.ParsePageAsync(SessionPage);

            // Construct a simple annon object for holding the data
            var graph = new
            {
                Total = HtmlPageMixins.GetAllSiblings(response, "td.para1", "Group Allowed Bandwidth:")
                                   .Select(x => x.TextContent)
                                   .FirstOrDefault(),

                Used = HtmlPageMixins.GetAllSiblings(response, "td.para1", "Total Bandwidth:")
                                   .Select(x => x.TextContent)
                                   .FirstOrDefault(),

                Download = HtmlPageMixins.GetAllSiblings(response, "td.para1", "Download:")
                                   .Select(x => x.TextContent)
                                   .FirstOrDefault(),

                Upload = HtmlPageMixins.GetAllSiblings(response, "td.para1", "Upload:")
                               .Select(x => x.TextContent)
                               .FirstOrDefault(),

                QuotaType = HtmlPageMixins.GetAllSiblings(response, "td.para1", "Bandwidth Quota Schedule:")
                               .Select(x => x.TextContent)
                               .FirstOrDefault(),
            };
            // Json will contain the session id of the current session and will
            // need to be included in subsequent server call
            return JsonConvert.SerializeObject(graph, Formatting.Indented);
        }


        public async Task<string> Logout(string sessionId)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return string.Empty;
        }

    }
}
