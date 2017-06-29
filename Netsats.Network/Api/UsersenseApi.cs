using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;

namespace Netstats.Network
{
    public class UserSenseApi : INetworkApi
    {
        static string requestUri = "/cgi-bin/user_session.ggi";

        HttpClient networkClient;

        public UserSenseApi()
        {
            networkClient = new HttpClient();
            networkClient.BaseAddress = new Uri("https://192.168.1.30");
        }

        static UserSenseApi()
        {
            //override cerificate checking because the connection is not encrypted
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;
        }

        public async Task<IHtmlDocument> Login(string username, string password, CancellationToken token)
        {
            var data = new Dictionary<string, string>()
            {
                ["user"] = username,
                ["passwd"] = password,
            };
            return await PostRequest(requestUri, data, token);
        }

        public async Task<IHtmlDocument> Logout(string sessionId)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId,
                ["logout"] = 1.ToString(),
            };
            return await PostRequest(requestUri, data, CancellationToken.None);
        }

        public async Task<IHtmlDocument> Refresh(string sessionId)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId,
            };
            return await PostRequest(requestUri, data, CancellationToken.None);
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

        async Task<IHtmlDocument> PostRequest(string request, Dictionary<string, string> data, CancellationToken cancelTtoken)
        {
            try
            {
                var response = await networkClient.PostAsync(request, new FormUrlEncodedContent(data), cancelTtoken);
                return Utilities.SingletonHtmlParser.Parse(await response.Content.ReadAsStringAsync());
            }
            catch(Exception) { return null; }
        }
    }
}
