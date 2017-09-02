using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Netstats.Api.Exceptions;
using Netstats.Network.Api.Interfaces;

namespace Netstats.Network.Api
{
    /// <summary>
    /// Provides a host of members for interacting with the Usersense internet service
    /// </summary>
    public class UserSenseNetworkApi : INetworkApi
    {
        static string requestUri = "/cgi-bin/user_session.ggi";

        public HttpClient NetworkClient { get; } = null;

        #region Constructor

        static UserSenseNetworkApi()
        {
            //override cerificate checking because the connection is not encrypted
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;
        }

        public UserSenseNetworkApi()
        {
            NetworkClient = new HttpClient()
            {
                BaseAddress = new Uri("https://192.168.1.30")
            };
        }

        #endregion

        #region Methods

        public async Task<string> Login(string username, string password, CancellationToken token)
        {
            var data = new Dictionary<string, string>()
            {
                ["user"] = username,
                ["passwd"] = password,
            };

            var response = await HtmlPageMixins.ParsePageAsync(await PostRequest(requestUri, data, token));

            if (response.GetPageType() != HtmlPageKind.SessionPage)
                // This is not the page we were expecting so throw an erro
                throw new Exception("An error occurred");

            // Json will contain the session id of the current session and will
            // need to be included in subsequent server call
            return JsonConvert.SerializeObject(new { Id = response.GetSessionId() }, Formatting.Indented);
        }

        public async Task<string> Login(string username, string password) => await Login(username, password, CancellationToken.None);

        public async Task<string> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes")
        {
            var data = new Dictionary<string, string>()
            {
                ["user"] = username,
                ["passwd"] = password,
                ["actualurl"] = string.Empty,
                ["userdelete"] = delete,
            };

            var response = await HtmlPageMixins.ParsePageAsync(await PostRequest(requestUri, data, token));

            if (response.GetPageType() != HtmlPageKind.SessionPage)
                // This is not the page we were expecting so throw an erro
                throw new Exception("An error occurred");

            // Json will contain the session id of the current session and will
            // need to be included in subsequent server call
            return JsonConvert.SerializeObject(new { Id = response.GetSessionId() }, Formatting.Indented);
        }

        public async Task<string> ConfirmLogin(string username, string password, string delete = "yes") => await ConfirmLogin(username, password, CancellationToken.None, delete);

        public async Task<string> Refresh(string sessionId) => await Refresh(sessionId, CancellationToken.None);

        public async Task<string> Refresh(string sessionId, CancellationToken token)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId,
            };

            var response = await HtmlPageMixins.ParsePageAsync(await PostRequest(requestUri, data, token));

            if (response.GetPageType() != HtmlPageKind.SessionPage)
                throw new Exception("An error occurred");

            // Construct a simple annon object for holding the data
            var resultGraph = new
            {
                TotalBandwidth = HtmlPageMixins.GetAllSiblings(response, "td.para1", "Group Allowed Bandwidth: ")
                                   .Select(x => x.TextContent)
                                   .FirstOrDefault(),

                UsedBandwidth = HtmlPageMixins.GetAllSiblings(response, "td.para1", "Total Bandwidth:")
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
            return JsonConvert.SerializeObject(resultGraph, Formatting.Indented);
        }

        public async Task<string> Logout(string sessionId, CancellationToken token)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId,
                ["logout"] = 1.ToString(),
            };
            await PostRequest(requestUri, data, CancellationToken.None);
            return string.Empty;
        }

        public async Task<string> Logout(string sessionId) => await Logout(sessionId, CancellationToken.None);

        #endregion


        #region Internal

        private async Task<string> PostRequest(string request, Dictionary<string, string> data, CancellationToken cancelTtoken)
        {
            if (request == null)
                throw new ArgumentNullException();
            if (data == null)
                throw new ArgumentNullException();
            try
            {
                var response = await NetworkClient.PostAsync(request, new FormUrlEncodedContent(data), cancelTtoken);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new ServerConnectionFailedException(ex);
            }
        }

        #endregion
    }
}