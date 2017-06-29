using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using System.Linq;
using static Netstats.Network.Utilities.PageParsing;

namespace Netstats.Network
{
    public class NetworkApi : INetworkApi
    {
        static string requestUri = "/cgi-bin/user_session.ggi";

        HttpClient networkClient;

        public NetworkApi()
        {
            networkClient = new HttpClient();
            networkClient.BaseAddress = new Uri("https://192.168.1.30");
        }

        static NetworkApi()
        {
            //override cerificate checking because the connection is not encrypted
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;
        }

        public async Task<LoginResult> Login(string username, string password, CancellationToken token)
        {
            var data = new Dictionary<string, string>()
            {
                ["user"] = username,
                ["passwd"] = password,
            };
            var response = await PostRequest(requestUri, data, token);

            if (response == null)
                return new LoginResult(false, NetworkErrorKind.UnableToMakeReqest);

            switch (IdentifyPage(response))
            {
                case PageKind.SessionPage:
                    return new LoginResult() { SessionId = ParseSessionId(response) };

                case PageKind.ConfirmationPage:
                    response = await ConfirmLogin(username, password, token);
                    if (IdentifyPage(response) == PageKind.SessionPage)
                        return new LoginResult() { SessionId = ParseSessionId(response) };
                    else
                        return new LoginResult(false, NetworkErrorKind.UnableToConfirm);

                case PageKind.AuthenticationFailedPage:
                    return new LoginResult(false, NetworkErrorKind.AuthenticationFailed);

                default:
                    return new LoginResult(false, NetworkErrorKind.Unkown);
            }
        }

        public async Task<LogoutResult> Logout(int sessionId)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId.ToString(),
                ["logout"] = 1.ToString(),
            };
            var response = await PostRequest(requestUri, data, CancellationToken.None);

            if (response == null)
                return new LogoutResult(false, NetworkErrorKind.UnableToMakeReqest);

            switch (IdentifyPage(response))
            {
                case PageKind.loggedoutPage:
                    return new LogoutResult();

                default:
                    return new LogoutResult(false, NetworkErrorKind.Unkown);
            }
        }

        public async Task<RefreshResult> Refresh(int sessionId)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId.ToString(),
            };
            var response = await PostRequest(requestUri, data, CancellationToken.None);

            if (response == null)
                return new RefreshResult(false, NetworkErrorKind.UnableToMakeReqest);

            switch (IdentifyPage(response))
            {
                case PageKind.SessionPage:
                    var download  = GetAllSiblings(response, "td.para1", "Download:").Select(x => x.TextContent).FirstOrDefault();
                    var upload    = GetAllSiblings(response, "td.para1", "Upload:").Select(x => x.TextContent).FirstOrDefault();
                    var quotaType = GetAllSiblings(response, "td.para1", "Bandwidth Quota Schedule:").Select(x => x.TextContent).FirstOrDefault();
                    var total     = GetAllSiblings(response, "td.para1", "Group Allowed Bandwidth::").Select(x => x.TextContent).FirstOrDefault();

                    return new RefreshResult()
                    {
                        QuotaType        = (BandwidthQuotaType)Enum.Parse(typeof(BandwidthQuotaType), quotaType),
                        TotalBandwidth   = ParseToMb(total),
                        Download         = ParseToMb(download),
                        Upload           = ParseToMb(upload),
                    };

                default:
                    return new RefreshResult(false, NetworkErrorKind.UnableToRefresh);
            }
        }

        public async Task<IHtmlDocument> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes")
        {
            var data = new Dictionary<string, string>()
            {
                ["user"] = username,
                ["passwd"] = password,
                ["actualurl"] = string.Empty,
                ["userdelete"] = delete,
            };
            return await PostRequest(requestUri, data, token);
        }

        Task<IHtmlDocument> PostRequest(string request, Dictionary<string, string> data, CancellationToken cancelTtoken)
        {
            return Task.Run(async () =>
            {
                var response = await networkClient.PostAsync(request, new FormUrlEncodedContent(data), cancelTtoken);
                return SingletonHtmlParser.Parse(await response.Content.ReadAsStringAsync());
            });
        }
    }
}
