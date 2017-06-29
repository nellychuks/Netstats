using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Netstats.Core.Api.Exceptions;

namespace Netstats.Core.Network.Api
{
    /// <summary>
    /// Provides a host of members for interacting with the Usersense internet service
    /// </summary>
    public class UserSenseApi : INetworkApi
    {
        static string requestUri = "/cgi-bin/user_session.ggi";

        HttpClient networkClient = null;

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

        /// <summary>
        /// Attempts to login to the internet service using the supplied <paramref name="username"/>
        /// and <paramref name="password"/>
        /// </summary>
        /// <param name="username">The username of the account to login with</param>
        /// <param name="password">The password of the account to login with</param>
        /// <param name="token"></param>
        /// <exception cref="ServerConnectionFailedException"
        /// <returns></returns>
        public async Task<string> Login(string username, string password, CancellationToken token)
        {
            var data = new Dictionary<string, string>()
            {
                ["user"] = username,
                ["passwd"] = password,
            };
            return await PostRequest(requestUri, data, token);
        }

        public async Task<string> Logout(string sessionId)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId,
                ["logout"] = 1.ToString(),
            };
            return await PostRequest(requestUri, data, CancellationToken.None);
        }

        public async Task<string> Refresh(string sessionId)
        {
            var data = new Dictionary<string, string>()
            {
                ["gi"] = sessionId,
            };
            return await PostRequest(requestUri, data, CancellationToken.None);
        }

 
        public async Task<string> ConfirmLogin(string username, string password, CancellationToken token, string delete = "yes")
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

        private async Task<string> PostRequest(string request, Dictionary<string, string> data, CancellationToken cancelTtoken)
        {
            if (request == null)
                throw new ArgumentNullException();
            if (data == null)
                throw new ArgumentNullException();
            try
            {
                var response = await networkClient.PostAsync(request, new FormUrlEncodedContent(data), cancelTtoken);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new ServerConnectionFailedException(ex);
            }
        }
    }
}